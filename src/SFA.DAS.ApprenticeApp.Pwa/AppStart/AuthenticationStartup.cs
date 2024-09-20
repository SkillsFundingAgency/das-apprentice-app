using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart
{

    public static class AuthenticationStartup
    {
        public static IServiceCollection AddAuthentication(
            this IServiceCollection services,
            ApplicationConfiguration config,
            IWebHostEnvironment environment)
        {
            services
                .AddApplicationAuthentication(config, environment)
                .AddApplicationAuthorisation();

            services.AddTransient((_) => config);

            return services;
        }

        public static void AddGovLoginAuthentication(
            this IServiceCollection services,
            ApplicationConfiguration config,
            IConfiguration configuration)
        {

            services.AddGovLoginAuthentication(configuration);
            services.AddApplicationAuthorisation();
            services.AddTransient<IApprenticeAccountProvider, ApprenticeAccountProvider>();
            services.AddTransient((_) => config);
        }

        private static IServiceCollection AddApplicationAuthentication(
            this IServiceCollection services,
            ApplicationConfiguration config,
            IWebHostEnvironment environment)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddApprenticeAuthentication(config.Authentication.MetadataAddress, environment);
            services.AddTransient<IApprenticeAccountProvider, ApprenticeAccountProvider>();
            return services;
        }

        private static IServiceCollection AddApplicationAuthorisation(
            this IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddScoped<AuthenticatedUser>();
            services.AddScoped(s => s
                .GetRequiredService<IHttpContextAccessor>().HttpContext.User);

            return services;
        }
    }
}
