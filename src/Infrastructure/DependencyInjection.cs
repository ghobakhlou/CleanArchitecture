using Application;
using Application.Common.Interfaces;
using Domain.Events;
using Infrastructure.Persistence;
using Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string environment)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DatabaseConnection"))
                .UseSnakeCaseNamingConvention();
        }, ServiceLifetime.Scoped);

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<IServiceBusService, ServiceBusService>();

        AddMassTransit(services, configuration);


        return services;
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        string serviceBusConnectionString = configuration.GetConnectionString("ServiceBus");
        if (!string.IsNullOrEmpty(serviceBusConnectionString))
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // Automatically discovers and adds consumers from the assembly.
                x.AddConsumers(typeof(ApplicationModule).Assembly);
                x.AddLogging(l => l.AddSerilog());


                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(serviceBusConnectionString);

                    cfg.SetMessageTopicName<StudentEmailChangedEvent>(TopicNames.StudentEmailChanged.TopicName);

                    cfg.ConfigureEndpoints(context);

                });
            });
        }

    }



    public static void SetMessageTopicName<TMessage>(this IBusFactoryConfigurator configurator, string topicName)
        where TMessage : class
    {
        configurator.Message<BaseMessage<TMessage>>(m => { m.SetEntityName(topicName); });
    }

}
