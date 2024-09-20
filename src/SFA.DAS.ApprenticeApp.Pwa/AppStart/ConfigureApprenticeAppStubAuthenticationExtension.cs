using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.ApprenticeApp.AppStart
{
    [ExcludeFromCodeCoverage]
    internal static class ConfigureApprenticeAppStubAuthenticationExtension
    {
        public static void AddApprenticeStubAuthentication(this IServiceCollection services, string redirectUrl, string loginRedirect, string localRedirect, string cookieDomain)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = localRedirect;
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = Constants.StubAuthCookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    if (!string.IsNullOrEmpty(cookieDomain))
                    {
                        options.Cookie.Domain = cookieDomain;
                    }
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";
                    options.Events.OnSigningOut = c =>
                    {
                        c.Response.Cookies.Delete(Constants.StubAuthCookieName);
                        c.Response.Redirect("/Home/Index");
                        return Task.CompletedTask;
                    };
                    if (!string.IsNullOrEmpty(loginRedirect))
                    {
                        options.Events.OnRedirectToLogin = c =>
                        {
                            var redirectUri = new Uri(c.RedirectUri);

                            var redirectQuery = HttpUtility.UrlEncode(
                                $"{redirectUri.Scheme}://{redirectUri.Authority}{HttpUtility.UrlDecode(redirectUri.Query.Replace("?ReturnUrl=", ""))}");
                            c.Response.Redirect(loginRedirect + "?ReturnUrl=" + redirectQuery);
                            return Task.CompletedTask;
                        };
                    }

                });
            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ICustomClaims>((options, customClaims) =>
                {
                    options.Events.OnValidatePrincipal = async (ctx) =>
                    {
                        var claims = new List<Claim>();
                        claims.AddRange(ctx.Principal.Claims);

                        if (customClaims != null)
                        {
                            var additionalClaims = await customClaims.GetClaims(new TokenValidatedContext(ctx.HttpContext, new AuthenticationScheme(CookieAuthenticationDefaults.AuthenticationScheme, "Cookie", typeof(ApprenticeStubAuthHandler)), new OpenIdConnectOptions(), ctx.Principal, new AuthenticationProperties()));
                            foreach (var additionalClaim in additionalClaims)
                            {
                                if (claims.FirstOrDefault(c => c.Type == additionalClaim.Type) == null)
                                    claims.Add(additionalClaim);
                            }
                        }
                        ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                        await ctx.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ctx.Principal);
                    };
                });
        }
    }
}