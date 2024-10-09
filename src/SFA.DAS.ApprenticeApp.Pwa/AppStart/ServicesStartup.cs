using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart
{
    public static class ServicesStartup
    {
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
}