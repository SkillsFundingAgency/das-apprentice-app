using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using StackExchange.Redis;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddDataProtectionExtension
    {
        public static void AddDataProtection(this IServiceCollection services, ApplicationConfiguration configuration)
        {
            var config = configuration.ConnectionStrings;

            if (config != null
                && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
                && !string.IsNullOrEmpty(config.RedisConnectionString))
            {
                var redisConnectionString = config.RedisConnectionString;
                var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

                var configurationOptions = ConfigurationOptions.Parse($"{redisConnectionString},{dataProtectionKeysDatabase}");
                var redis = ConnectionMultiplexer
                    .Connect(configurationOptions);

                services.AddDataProtection()
                    .SetApplicationName("das-apprentice-app")
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
        }
    }
}
