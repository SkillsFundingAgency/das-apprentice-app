using System.Diagnostics.CodeAnalysis;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class ServicesStartup
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services,
            IWebHostEnvironment environment)
        {

            services.AddDomainHelper(environment);

            return services;
        }

        private static IServiceCollection AddDomainHelper(this IServiceCollection services, IWebHostEnvironment environment)
        {
            var domain = ".localhost";
            if (!environment.IsDevelopment())
            {
                domain = ".apprenticeships.education.gov.uk";
            }

            services.AddSingleton(new DomainHelper(domain));
            return services;
        }

        public static IServiceCollection AddOuterApi(
            this IServiceCollection services,
            Configuration.OuterApiConfiguration configuration)
        {
            services.AddHealthChecks();
            services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            services
                .AddRestEaseClient<IOuterApiClient>(configuration.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            services.AddTransient<IApimClientConfiguration>((_) => configuration);

            return services;
        }
    }

    public class OuterApiConfiguration : IApimClientConfiguration
    {
        public string ApiBaseUrl { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
    }
}