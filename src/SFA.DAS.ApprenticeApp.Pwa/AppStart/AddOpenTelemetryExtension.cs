using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart;

public static class AddOpenTelemetryExtension
{
    public static void AddOpenTelemetryRegistration(this IServiceCollection services, string applicationInsightsConnectionString)
    {
        services.AddHttpContextAccessor();

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WEBSITE_SITE_NAME"))
                    .AddAspNetCoreInstrumentation();
            })
            .UseAzureMonitor(options =>
            {
                options.ConnectionString = applicationInsightsConnectionString;
            });
    }
}

