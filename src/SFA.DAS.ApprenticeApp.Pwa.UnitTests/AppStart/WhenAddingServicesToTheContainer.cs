using AutoFixture.NUnit3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.AppStart;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Testing.AutoFixture;
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


        [Test, MoqAutoData]
        public void Then_The_OuterApi_Is_Added(
            [Frozen] Configuration.OuterApiConfiguration configuration)
        {
            ServiceCollection serviceCollection = new();
            configuration.ApiBaseUrl = "https://localhost:0000/";
            var act = ServicesStartup.AddOuterApi(serviceCollection, configuration);
            var provider = act.BuildServiceProvider();
            var type = provider.GetService(typeof(IApimClientConfiguration));

            Assert.That(type, Is.Not.Null);
        }

        private static void SetupServiceCollection(IServiceCollection serviceCollection)
        {
            var appConfig = GenerateAppConfig();
            var config = GenerateConfiguration();
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddServiceRegistration(config, appConfig);
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