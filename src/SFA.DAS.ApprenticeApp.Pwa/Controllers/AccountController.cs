using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.GovUK.Auth.Services;
using System.Security.Claims;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IStubAuthenticationService _stubAuthenticationService;
        private readonly IConfiguration _config;
        public static ApplicationConfiguration _appConfig { get; set; }
        private readonly IOuterApiClient _client;
        private readonly IOidcService _oidcService;
        private readonly AuthenticatedUser _user;
        private readonly IApprenticeAccountProvider _apprenticeAccountProvider;

        public AccountController(ILogger<AccountController> logger,
            IStubAuthenticationService stubAuthenticationService,
            ApplicationConfiguration appConfig,
            AuthenticatedUser user,
            IOidcService oidcService, IApprenticeAccountProvider apprenticeAccountProvider,
            IConfiguration configuration,
            IOuterApiClient client
        )
        {
            _logger = logger;
            _stubAuthenticationService = stubAuthenticationService;
            _appConfig = appConfig;
            _config = configuration;
            _client = client;
            _oidcService = oidcService;
            _apprenticeAccountProvider = apprenticeAccountProvider;
            _user = user;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Authenticated()
        {
            var apprenticeId = User.Identities.First().Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;
            var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

            var additionalClaims = new ClaimsIdentity();

            additionalClaims.AddClaim(new Claim(Constants.ApprenticeshipIdClaimKey, apprenticeDetails.MyApprenticeship.ApprenticeshipId.ToString()));
            additionalClaims.AddClaim(new Claim(Constants.StandardUIdClaimKey, apprenticeDetails.MyApprenticeship.StandardUId.ToString()));

            User.AddIdentity(additionalClaims);

            return RedirectToAction("Index", "Terms");
           
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

            var schemes = new List<string>
            {
                CookieAuthenticationDefaults.AuthenticationScheme
            };

            _ = bool.TryParse(_appConfig.StubAuth, out var stubAuth);
            if (!stubAuth)
            {
                schemes.Add(OpenIdConnectDefaults.AuthenticationScheme);
            }


            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

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