using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
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
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);
           
            if (!string.IsNullOrEmpty(apprenticeId))
            {
                string message = $"Apprentice authenticated and cookies added for {apprenticeId}";
                _logger.LogInformation(message);

                var lastName = Claims.GetClaim(HttpContext, Constants.ApprenticeLastNameClaimKey);
                if (!string.IsNullOrEmpty(lastName))
                {
                    try
                    {
                        var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                        if (apprenticeDetails?.MyApprenticeship != null)
                        {
                            return RedirectToAction("Index", "Terms");
                        }
                    }
                    catch (Exception)
                    {
                        string myappmsg = $"MyApprenticeship data error or not found for {apprenticeId}";
                        _logger.LogInformation(myappmsg);
                        return RedirectToAction("EmailMismatchError", "Account");
                    }
                }
                else
                {
                    try
                    {
                        var email = Claims.GetClaim(HttpContext, Constants.ApprenticeNameClaimKey);
                        var registrationId = await _client.GetRegistrationIdByEmail(email);
                        if (registrationId != Guid.Empty)
                        {
                            string cmadmsg = $"Registration record found for {apprenticeId}";
                            _logger.LogInformation(cmadmsg);
                            return RedirectToAction("CmadError", "Account", new { registrationId });
                        }
                    }
                    catch (Exception)
                    {
                        string cmaderrormsg = $"Registration data error or not found for {apprenticeId}";
                        _logger.LogInformation(cmaderrormsg);
                        return RedirectToAction("EmailMismatchError", "Account");
                    }  
                }
            }
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
                model.Email = model.Email.ToLower();
                var claims = await _stubAuthenticationService.GetStubSignInClaims(model);
                var apprenticeId = claims?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

                if (!string.IsNullOrEmpty(apprenticeId))
                {
                    var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                    claims?.Identities.First().AddClaim(new Claim(Constants.ApprenticeshipIdClaimKey, apprenticeDetails.MyApprenticeship.ApprenticeshipId.ToString()));
                    claims?.Identities.First().AddClaim(new Claim(Constants.StandardUIdClaimKey, apprenticeDetails.MyApprenticeship.StandardUId.ToString()));
                }

                // Set extended cookie expiration here
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Persistent cookie survives browser restarts
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1), // 1 month expiration
                    AllowRefresh = true // Allow refreshing the session
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claims,
                    authProperties); // Pass the modified properties

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

            return SignOut(
                authenticationProperties,
                schemes.ToArray());
        }

        [HttpGet]
        [Authorize]
        [Route("Stub-Auth", Name = RouteNames.StubSignedIn)]
        public IActionResult StubSignedIn()
        {
            if (_config["ResourceEnvironmentName"].ToUpper() == "PRD")
            {
                return NotFound();
            }

            var viewModel = new StubAuthenticationViewModel
            {
                Email = User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value.ToLower(),
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
        public IActionResult CmadError(Guid? registrationId)
        {
            CmadErrorViewModel vm = new()
            {
                RegistrationId = registrationId ?? Guid.Empty
            };
            return View(vm);
        }      
        
        [HttpGet]
        public IActionResult EmailMismatchError()
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