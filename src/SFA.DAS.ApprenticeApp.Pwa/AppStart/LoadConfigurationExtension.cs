using System.Diagnostics.CodeAnalysis;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart;

[ExcludeFromCodeCoverage]
public static class LoadConfigurationExtension
{
    public static IConfigurationRoot LoadConfiguration(this IConfiguration config, IServiceCollection services)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(config)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

        if (!config["ResourceEnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
#if DEBUG
            configBuilder.AddJsonFile("appsettings", true)
                .AddJsonFile("appsettings.Development.json", true);
#endif

            configBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = config["ConfigNames"]!.Split(",");
                options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
                options.EnvironmentName = config["ResourceEnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
        }

        var configuration = configBuilder.Build();

        var appConfig = configuration.Get<ApplicationConfiguration>();
        services.AddSingleton(appConfig!);

        return configuration;
    }
}