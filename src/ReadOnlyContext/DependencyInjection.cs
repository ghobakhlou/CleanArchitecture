using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ReadOnlyContext;

public static class DependencyInjection
{
    public static IServiceCollection AddReadonlyDbContext(this IServiceCollection services, IConfiguration configuration, string environment)
    {
        //Add DbContext Service
        var connectionString = string.Format(configuration["ConnectionStrings:DatabaseConnection"], configuration["ConnectionStrings:DatabaseName"]);

        services.AddDbContext<ReadonlyApplicationDbContext>(options => options.UseNpgsql(connectionString)
          .UseSnakeCaseNamingConvention()
            , ServiceLifetime.Transient);

        services.AddTransient<IReadonlyApplicationDbContext, ReadonlyApplicationDbContext>();

        return services;
    }


}
