using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;

namespace SFA.DAS.ApprenticeApp.AppStart
{
    public class ApprenticeStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICustomClaims _customClaims;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [ExcludeFromCodeCoverage]
        public ApprenticeStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
        {
            _customClaims = customClaims;
            _httpContextAccessor = httpContextAccessor;
        }

        [ExcludeFromCodeCoverage]
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(Constants.StubAuthCookieName))
            {
                return AuthenticateResult.Fail("");
            }
            var authCookieValue = JsonConvert.DeserializeObject<StubAuthUserDetails>(_httpContextAccessor.HttpContext.Request.Cookies[Constants.StubAuthCookieName]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, authCookieValue.Email),
                new Claim(ClaimTypes.NameIdentifier, authCookieValue.Id),
                new Claim("sub", authCookieValue.Id)
            };
            var identity = new ClaimsIdentity(claims, "Apprentice-stub");
            var principal = new ClaimsPrincipal(identity);

            if (_customClaims != null)
            {
                var additionalClaims = await _customClaims.GetClaims(new TokenValidatedContext(_httpContextAccessor.HttpContext, Scheme, new OpenIdConnectOptions(), principal, new AuthenticationProperties()));
                claims.AddRange(additionalClaims);
                principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Apprentice-stub"));
            }
            var ticket = new AuthenticationTicket(principal, "Apprentice-stub");
            var result = AuthenticateResult.Success(ticket);
            return result;
        }
    }
}