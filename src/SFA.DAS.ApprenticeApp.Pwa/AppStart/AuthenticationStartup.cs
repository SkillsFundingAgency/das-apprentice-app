using SFA.DAS.ApprenticeApp.Pwa.Authentication;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Models;

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
            var loginRedirect = string.IsNullOrEmpty(cookieDomain) ? "" : $"https://{cookieDomain}/account-details";
            var signedOutRedirectUrl = string.IsNullOrEmpty(cookieDomain) ? "https://localhost:5003/home" : $"https://{cookieDomain}/home";
            var localStubLoginPath = "/account-details";
            services.AddAndConfigureGovUkAuthentication(configuration,
                new AuthRedirects
                {
                    CookieDomain = cookieDomain,
                    LoginRedirect = loginRedirect,
                    LocalStubLoginPath = localStubLoginPath,
                    SignedOutRedirectUrl = signedOutRedirectUrl
                },
                typeof(ApprenticeAccountPostAuthenticationClaimsHandler));

            services.AddHttpContextAccessor();
        }
    }
}
