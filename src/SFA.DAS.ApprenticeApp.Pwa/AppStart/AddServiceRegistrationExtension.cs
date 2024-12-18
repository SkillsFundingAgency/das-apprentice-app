﻿using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart;

public static class AddServiceRegistrationExtension
{
       public static void AddServiceRegistration(this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration,
        ApplicationConfiguration appConfig)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        services.AddTransient<ICustomClaims, ApprenticeAccountPostAuthenticationClaimsHandler>();
        services.AddGovLoginAuthentication(appConfig, configuration);
    }
}