using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            const string dasCdn = "das-at-frnt-end.azureedge.net das-pp-frnt-end.azureedge.net das-mo-frnt-end.azureedge.net das-test-frnt-end.azureedge.net das-test2-frnt-end.azureedge.net das-prd-frnt-end.azureedge.net";

            if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    new StringValues(
                        $"script-src 'self' 'unsafe-inline' 'unsafe-eval' {dasCdn} https://www.googletagmanager.com https://tagmanager.google.com https://www.google-analytics.com https://ssl.google-analytics.com ; " +
                        $"style-src 'self' 'unsafe-inline' {dasCdn} https://tagmanager.google.com https://fonts.googleapis.com ; " +
                        $"img-src {dasCdn} www.googletagmanager.com https://ssl.gstatic.com https://www.gstatic.com https://www.google-analytics.com ; " +
                        $"font-src {dasCdn} https://fonts.gstatic.com ;" +
                        "connect-src https://www.google-analytics.com ;" +
                        "frame-src https://www.googletagmanager.com"));
            }
            await _next(context);
        }
    }
}
