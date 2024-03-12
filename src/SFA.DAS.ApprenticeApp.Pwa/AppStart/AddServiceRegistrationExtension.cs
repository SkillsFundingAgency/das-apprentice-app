using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Services;


namespace SFA.DAS.ApprenticeApp.Pwa.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, 
        ApplicationConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

        services.AddTransient<ICustomClaims, CustomClaims>();
        services.AddTransient<IStubAuthenticationService, StubAuthenticationService>();
        services.AddAndConfigureApprenticeAuthentication(configuration);

    }

}