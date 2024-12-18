using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.ApprenticePortal.SharedUi.GoogleAnalytics;

namespace SFA.DAS.ApprenticeApp.Web.Extensions.GoogleAnalytics
{
    public static class Extensions
    {
        public static bool GoogleAnalyticsIsEnabled(this ViewDataDictionary viewData)
            => !string.IsNullOrWhiteSpace(GetConfiguration(viewData)?.GoogleTagManagerId);

        public static string? GetGoogleTagManagerId(this ViewDataDictionary viewData)
            => GetConfiguration(viewData)?.GoogleTagManagerId;

        private static GoogleAnalyticsConfiguration? GetConfiguration(ViewDataDictionary viewData)
            => viewData.TryGetValue(ViewDataKeys.GoogleAnalyticsConfigurationKey, out var section)
                ? section as GoogleAnalyticsConfiguration
                : null;

        public static IServiceCollection EnableGoogleAnalytics(this IServiceCollection services, GoogleAnalyticsConfiguration zenDeskConfiguration)
        {
            services.Configure<MvcOptions>(options =>
                options.Filters.Add(new EnableGoogleAnalyticsAttribute(zenDeskConfiguration)));
            return services;
        }

        public static class ViewDataKeys
        {
            public const string GoogleAnalyticsConfigurationKey = "SharedUI.GoogleAnalyticsConfiguration";
        }
    }
}