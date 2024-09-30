using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.GovUK.Auth.Services;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IStubAuthenticationService _stubAuthenticationService;
        private readonly IConfiguration _config;
        public static ApplicationConfiguration _appConfig { get; set; }
        private readonly IOuterApiClient _client;

        public AccountController(ILogger<AccountController> logger,
            IStubAuthenticationService stubAuthenticationService,
            ApplicationConfiguration appConfig, 
            IConfiguration configuration,
            IOuterApiClient client
        )
        {
            _logger = logger;
            _stubAuthenticationService = stubAuthenticationService;
            _appConfig = appConfig;
            _config = configuration;
            _client = client;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Authenticated()
        {
            var apprenticeId = User.Identities.First().Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;
            var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

            if (apprenticeDetails.MyApprenticeship != null) {
                var appCookie = Request.Cookies[Constants.ApprenticeshipIdClaimKey];

                if (appCookie == null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(1),
                        Path = "/",
                        Secure = true,
                        HttpOnly = true
                    };
                    Response.Cookies.Append(Constants.ApprenticeshipIdClaimKey, apprenticeDetails.MyApprenticeship.ApprenticeshipId.ToString(), cookieOptions);
                }

                var crsCookie = Request.Cookies[Constants.StandardUIdClaimKey];

                if (crsCookie == null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(1),
                        Path = "/",
                        Secure = true,
                        HttpOnly = true
                    };
                    Response.Cookies.Append(Constants.StandardUIdClaimKey, apprenticeDetails.MyApprenticeship.StandardUId.ToString(), cookieOptions);
                }

                string message = $"User authenticated and cookies added for {apprenticeId}";
                _logger.LogInformation(message);
                return RedirectToAction("Index", "Terms");
            }
            return RedirectToAction("Error", "Account");

        }

        [HttpGet]
        [Route("account-details", Name = RouteNames.StubAccountDetailsGet)]
        public IActionResult AccountDetails([FromQuery] string returnUrl)
        {
            if (_config["ResourceEnvironmentName"].ToUpper() == "PRD")
            {
                return NotFound();
            }

            return View("AccountDetails", new StubAuthenticationViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [Route("account-details", Name = RouteNames.StubAccountDetailsPost)]
        public async Task<IActionResult> AccountDetails(StubAuthenticationViewModel model)
        {
            if (_config["ResourceEnvironmentName"].ToUpper() == "PRD")
            {
                return NotFound();
            }

            try
            {
                var claims = await _stubAuthenticationService.GetStubSignInClaims(model);
                var apprenticeId = claims?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

                if (!string.IsNullOrEmpty(apprenticeId))
                {
                    var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                    claims?.Identities.First().AddClaim(new Claim(Constants.ApprenticeshipIdClaimKey, apprenticeDetails.MyApprenticeship.ApprenticeshipId.ToString()));
                    claims?.Identities.First().AddClaim(new Claim(Constants.StandardUIdClaimKey, apprenticeDetails.MyApprenticeship.StandardUId.ToString()));
                }
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
                    new AuthenticationProperties());

                return RedirectToRoute(RouteNames.StubSignedIn, new { returnUrl = model.ReturnUrl });
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Account");
            }
            
            
           
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SigningOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token", idToken);

            _ = bool.TryParse(_appConfig.StubAuth, out var stubAuth);
           
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if(!stubAuth)
            {
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }

            HttpContext.Response.Cookies.Delete(Constants.ApprenticeshipIdClaimKey);
            HttpContext.Response.Cookies.Delete(Constants.StandardUIdClaimKey);

            return RedirectToAction("Index", "Home");
        }

        

        [HttpGet]
        [Authorize]
        [Route("Stub-Auth", Name = RouteNames.StubSignedIn)]
        public IActionResult StubSignedIn()
        {
            var viewModel = new StubAuthenticationViewModel
            {
                Email = User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value,
                Id = User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value
            };
           
            return RedirectToAction("Index", "Terms");
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult YourAccount()
        {
            return View();
        }

    }
}