using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart;

public static class AddOpenTelemetryExtension
{
    public static void AddOpenTelemetryRegistration(this IServiceCollection services, string APPLICATIONINSIGHTS_CONNECTION_STRING)
    {
        services.AddHttpContextAccessor(); // Required for HttpContext access
        services.AddSingleton<UserIdProcessor>(); // Register UserIdProcessor in DI

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ApprenticeApp"))
                    .AddAspNetCoreInstrumentation()
                    .AddProcessor(sp => sp.GetRequiredService<UserIdProcessor>()); // Use DI to resolve UserIdProcessor
            })
            .UseAzureMonitor(options =>
            {
                options.ConnectionString = APPLICATIONINSIGHTS_CONNECTION_STRING;
            });
    }
}
