using Api;                  // API layer: exposes endpoints (GraphQL & controllers) for client interactions.
// -----------------------------------------------------------------------------
// DESIGN DECISION: Separation of Concerns & Clean Architecture.
// The Application, Infrastructure, and ReadOnlyContext layers are clearly separated
// to isolate business logic (Application), external dependencies (Infrastructure), and
// read-optimized data access (CQRS read side).
// -----------------------------------------------------------------------------
using Application;           // Application layer containing use-case logic & MediatR handlers (CQRS).
using Infrastructure;        // Infrastructure layer for persistence, external services, etc.
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ReadOnlyContext;       // Specialized read-only DB context for CQRS query side.
using Serilog;               // Structured logging library to facilitate robust observability.
using System.Reflection;

// Build configuration settings from multiple sources: JSON, User Secrets, and Environment variables.
// -----------------------------------------------------------------------------
// DESIGN DECISION: Flexible & Secure Configuration.
// By layering configurations, we enable environment-specific overrides and secure local secrets,
// in line with best practices from DDD (explicit configuration) and secure deployments.
// -----------------------------------------------------------------------------
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
        optional: true,
        reloadOnChange: true)
    .AddUserSecrets(assembly: Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Create the WebApplication builder.
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// DESIGN DECISION: Contextual Logging & Observability.
// Using a dedicated application name and Application Insights ensures that logs are
// contextualized and can be correlated across services, which is crucial for modern
// distributed systems and microservice architectures.
// -----------------------------------------------------------------------------
const string applicationName = "CleanArchitecture";
var applicationInsightConnection = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

// Configure a temporary bootstrap logger using Serilog.
// -----------------------------------------------------------------------------
// DESIGN DECISION: Structured Logging with Serilog.
// Establishing structured logging early in the startup process ensures that any issues
// during initialization are captured. This is an example of applying functional programming
// principles by making side effects (logging) explicit and well-structured.
// -----------------------------------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", applicationName)
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(applicationInsightConnection, TelemetryConverter.Traces)
    .CreateLogger();

var environment = builder.Environment;

try
{
    // Register services across different layers of the application.
    // -------------------------------------------------------------------------
    // DESIGN DECISION: Layered Architecture & Dependency Injection.
    // We inject the Application layer (business logic via MediatR),
    // the Infrastructure layer (data access, external integrations),
    // and a dedicated ReadOnly context for CQRS query operations.
    // This ensures that each layer has a single responsibility and is easily testable.
    // -------------------------------------------------------------------------
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(configuration, environment.EnvironmentName);
    builder.Services.AddReadonlyDbContext(builder.Configuration, environment.EnvironmentName);

    // Makes the current HttpContext accessible (e.g., for user context propagation).
    builder.Services.AddHttpContextAccessor();

    // Configure the GraphQL server using Hot Chocolate.
    // -------------------------------------------------------------------------
    // DESIGN DECISION: CQRS for GraphQL API.
    // By integrating GraphQL with support for filtering, projections, and sorting,
    // we demonstrate how read and write operations are separated, aligning with CQRS principles.
    // Additionally, diagnostic listeners and schema initialization support maintainability.
    // -------------------------------------------------------------------------
    builder.Services
        .AddGraphQLServer()
        .AddDiagnosticEventListener<ExecutionEventListener>()
        .AddFiltering()
        .AddProjections()
        .AddSorting()
        .AddQueryType<Query>()
        .AddMutationType<Mutation>()
        // .AddApplicationTypes() // Uncomment to auto-register GraphQL types.
        .IgnoreRootTypes()
        .InitializeOnStartup();

    // Integrate Application Insights for performance monitoring and telemetry.
    builder.Services.AddApplicationInsightsTelemetry(opt =>
        opt.ConnectionString = applicationInsightConnection
    );

    // Override logger configuration for the full application lifecycle.
    // -------------------------------------------------------------------------
    // DESIGN DECISION: Consistent & Robust Logging Across Environments.
    // Reconfiguring the logger at the host level ensures that logging is coherent and
    // that every component of the application emits structured logs with the required context.
    // -------------------------------------------------------------------------
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", applicationName)
            .WriteTo.Console()
            .WriteTo.ApplicationInsights(applicationInsightConnection, TelemetryConverter.Traces);
    });

    // Build the web application.
    var app = builder.Build();

    // Global exception handling middleware.
    // -------------------------------------------------------------------------
    // DESIGN DECISION: Centralized Error Handling.
    // This middleware captures any unhandled exceptions, logs them, and returns a generic error
    // message to clients. This is critical for production-readiness and aligns with the principle
    // of failing gracefully in distributed systems.
    // -------------------------------------------------------------------------
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature != null)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(exceptionHandlerFeature.Error, "Unhandled exception.");
            }

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
        });
    });

    // Database migration block.
    // -------------------------------------------------------------------------
    // DESIGN DECISION: Automated Schema Evolution.
    // Running migrations at startup ensures that the database schema remains in sync with the domain model,
    // an essential aspect of maintaining consistency in a DDD-based system.
    // -------------------------------------------------------------------------
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate(); // Applies any pending migrations automatically.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    // Configure routing and endpoint mappings.
    // -------------------------------------------------------------------------
    // DESIGN DECISION: Minimal API & Endpoint Mapping.
    // Mapping GraphQL endpoints separately ensures that the application can handle
    // multiple types of client interactions, supporting both RESTful and GraphQL APIs.
    // -------------------------------------------------------------------------
    app.UseRouting();
    app.MapGraphQL();

    // Start the application.
    app.Run();
}
catch (Exception ex)
{
    // Capture any critical failures during startup.
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    // Ensure all logs are flushed and resources are released gracefully.
    Log.CloseAndFlush();
}
