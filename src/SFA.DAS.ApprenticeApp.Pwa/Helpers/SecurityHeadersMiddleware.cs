using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public SecurityHeadersMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string dasCdn = "das-at-frnt-end.azureedge.net das-pp-frnt-end.azureedge.net das-mo-frnt-end.azureedge.net das-test-frnt-end.azureedge.net das-test2-frnt-end.azureedge.net das-prd-frnt-end.azureedge.net";
            const string googleAnalytics = "https://www.googletagmanager.com https://tagmanager.google.com https://www.google-analytics.com https://ssl.google-analytics.com";
            const string clarityMs = "https://www.clarity.ms https://clarity.microsoft.com";
            const string pstatic = "https://ssl.pstatic.com https://www.pstatic.com";

            string csp = $"script-src 'self' 'unsafe-inline' 'unsafe-eval' {dasCdn} {googleAnalytics} {clarityMs} https://code.jquery.com ; " +
                         $"style-src 'self' 'unsafe-inline' {dasCdn} https://tagmanager.google.com https://fonts.googleapis.com ; " +
                         $"img-src 'self' {dasCdn} {googleAnalytics} {pstatic} ; " +
                         $"font-src 'self' {dasCdn} https://fonts.gstatic.com ; " +
                         $"connect-src 'self' ws://localhost:* wss://localhost:* http://localhost:* https://localhost:* {dasCdn} {googleAnalytics} {clarityMs} ; " +
                         $"frame-src https://www.googletagmanager.com";

            if (_env.IsDevelopment())
            {
                csp += " http://localhost:* https://localhost:*";
            }

            if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                context.Response.Headers.Add("Content-Security-Policy", new StringValues(csp));
            }

            await _next(context);
        }
    }
}
