using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.AppStart;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.AppStart
{
    public class WhenAddingServicesToTheContainer
    {
        [Test]
        [TestCase(typeof(ICustomClaims))]
        [TestCase(typeof(IStubAuthenticationService))]
        public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
        {
            ServiceCollection serviceCollection = new();
            SetupServiceCollection(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();
            var type = provider.GetService(toResolve);

            Assert.That(type, Is.Not.Null);
        }
           

        private static void SetupServiceCollection(IServiceCollection serviceCollection)
        {
            var appConfig = GenerateAppConfig();
            var config = GenerateConfiguration();
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddServiceRegistration(appConfig);
        }

        private static IConfiguration GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = [new("StubAuth", "true")]
            };

            var provider = new MemoryConfigurationProvider(configSource);
            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }

        private static ApplicationConfiguration GenerateAppConfig()
        {
            ApplicationConfiguration appConfigSource = new();
            appConfigSource.CookieName = Constants.StubAuthCookieName;
            appConfigSource.StubAuth = "true";
            return appConfigSource;
        }
    }
}