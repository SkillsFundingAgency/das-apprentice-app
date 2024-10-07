using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart
{

    public static class AuthenticationStartup
    {
        public static void AddGovLoginAuthentication(
            this IServiceCollection services,
            ApplicationConfiguration config,
            IConfiguration configuration)
        {
            services.AddGovLoginAuthentication(configuration);
            services.AddTransient<IApprenticeAccountProvider, ApprenticeAccountProvider>();
            services.AddAuthorization();
            services.AddScoped<AuthenticatedUser>();
            services.AddHttpContextAccessor();
            services.AddTransient((_) => config);
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
