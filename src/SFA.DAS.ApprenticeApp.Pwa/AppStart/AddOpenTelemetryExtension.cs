using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart;

public static class AddOpenTelemetryExtension
{
    public static void AddOpenTelemetryRegistration(this IServiceCollection services, string applicationInsightsConnectionString)
    {
        services.AddOpenTelemetry().UseAzureMonitor(options => {
            options.ConnectionString = applicationInsightsConnectionString;
        });
    }
}