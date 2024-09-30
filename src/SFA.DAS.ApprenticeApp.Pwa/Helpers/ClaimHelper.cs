using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.RateLimiting;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public static class Claims
    {
        public static string GetClaim(HttpContext httpContext, string claimKey)
        {
            if (httpContext.User != null && httpContext.User.Claims.Any())
            {
                try
                {
                    var claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == claimKey);

                    if (claim == null)
                    {
                        return httpContext.Request.Cookies[claimKey].ToString() ?? string.Empty;
                          
                    }
                    return claim?.Value ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
