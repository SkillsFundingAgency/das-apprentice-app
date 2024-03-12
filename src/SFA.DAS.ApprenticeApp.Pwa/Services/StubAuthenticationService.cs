using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.AppStart;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeApp.Pwa.Services
{
    public interface IStubAuthenticationService
    {
        void AddStubApprenticeAuth(IResponseCookies cookies, StubAuthUserDetails model, bool isEssential = false);
        Task<ClaimsPrincipal> GetStubSignInClaims(StubAuthUserDetails model);
    }

    public class StubAuthenticationService : IStubAuthenticationService
    {
        private readonly ICustomClaims _customClaims;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _environment;

        public StubAuthenticationService(IConfiguration configuration, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor)
        {
            _customClaims = customClaims;
            _httpContextAccessor = httpContextAccessor;
            _environment = configuration["ResourceEnvironmentName"]?.ToUpper();
        }

        public void AddStubApprenticeAuth(IResponseCookies cookies, StubAuthUserDetails model, bool isEssential = false)
        {
            if (_environment == "PRD")
            {
                return;
            }
            
            var authCookie = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                Path = "/",
                Domain = "localhost",
                Secure = true,
                HttpOnly = true,
                IsEssential = isEssential,
                SameSite = SameSiteMode.None
            };
            cookies.Append(Constants.StubAuthCookieName, JsonConvert.SerializeObject(model), authCookie);

            
        }

        public async Task<ClaimsPrincipal> GetStubSignInClaims(StubAuthUserDetails model)
        {
            if (_environment == "PRD")
            {
                return null;
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.NameIdentifier, model.Id),
                new Claim("sub", model.Id)  
            };


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            
            if (_customClaims != null)
            {
                var additionalClaims = await _customClaims.GetClaims(new TokenValidatedContext(_httpContextAccessor.HttpContext,new AuthenticationScheme(CookieAuthenticationDefaults.AuthenticationScheme, "Cookie", typeof(ApprenticeStubAuthHandler)), new OpenIdConnectOptions(), principal, new AuthenticationProperties() ));
                claims.AddRange(additionalClaims);
                principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            }

            return principal;
        }
    }
}