using SFA.DAS.ApprenticeApp.Pwa.Authentication;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.GovUK.Auth.AppStart;

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

        public static void AddGovLoginAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieDomain = AppDomainExtensions.GetDomain(configuration["ResourceEnvironmentName"]);
            var stubLoginRedirect = string.IsNullOrEmpty(cookieDomain) ? "" : $"https://{cookieDomain}/account-details";
            var signedOutRedirectUrl = string.IsNullOrEmpty(cookieDomain) ? "https://localhost:5003/apprentice-signed-out" : $"https://{cookieDomain}/apprentice-signed-out";
            services.AddAndConfigureGovUkAuthentication(configuration,
                typeof(ApprenticeAccountPostAuthenticationClaimsHandler), signedOutRedirectUrl, "/account-details", cookieDomain, stubLoginRedirect);

            services.AddHttpContextAccessor();
        }
    }
}
