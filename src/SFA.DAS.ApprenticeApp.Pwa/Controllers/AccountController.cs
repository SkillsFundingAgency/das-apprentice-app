using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.GovUK.Auth.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IStubAuthenticationService _stubAuthenticationService;
        private readonly IConfiguration _config;
        public static ApplicationConfiguration _appConfig { get; set; }
        private readonly IOuterApiClient _client;
        private readonly IApprenticeContext _apprenticeContext;

        private const string NewUiOptInCookieName = "SFA.ApprenticeApp.NewUiOptIn";
        private const string CohortUserSessionKey = "CohortUser";
        private const string OptInUserSessionKey = "OptInUser";
        private const string ForceOldUISessionKey = "ForceOldUI";
        
        public AccountController(ILogger<AccountController> logger,
            IStubAuthenticationService stubAuthenticationService,
            ApplicationConfiguration appConfig,
            IConfiguration configuration,
            IOuterApiClient client,
            IApprenticeContext apprenticeContext
        )
        {
            _logger = logger;
            _stubAuthenticationService = stubAuthenticationService;
            _appConfig = appConfig;
            _config = configuration;
            _client = client;
            _apprenticeContext = apprenticeContext;
        }

        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Authenticated()
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;
            if (!string.IsNullOrEmpty(apprenticeId))
            {
                string message = $"Apprentice authenticated and cookies added for {apprenticeId}";
                _logger.LogInformation(message);

                try
                {
                    var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                    if (apprenticeDetails?.MyApprenticeship != null)
                    {
                        // Determine cohort membership
                        bool isCohort = IsUserInNewUiCohort(apprenticeDetails.MyApprenticeship.TrainingProviderId);
                        HttpContext.Session.SetString(CohortUserSessionKey, isCohort ? "true" : "false");

                        // Check opt‑in cookie
                        bool optIn = Request.Cookies[NewUiOptInCookieName] == "true";
                        HttpContext.Session.SetString(OptInUserSessionKey, optIn ? "true" : "false");

                        // Set legacy UserType for backward compatibility
                        string userType = (optIn || isCohort) ? "SpecialUser" : "RegularUser";
                        HttpContext.Session.SetString("UserType", userType);

                        return RedirectToAction("Index", "Terms");
                    }
                    else
                    {
                        string cmaderrormsg = $"MyApprenticeship data not found for {apprenticeId}";
                        _logger.LogInformation(cmaderrormsg);
                        return RedirectToAction("CmadError", "Account");
                    }
                }
                catch (Exception ex)
                {
                    string cmaderrormsg = $"MyApprenticeship data error or not found for {apprenticeId}";
                    _logger.LogInformation(cmaderrormsg);
                    return RedirectToAction("CmadError", "Account");
                }
            }
            else
            {
                return RedirectToAction("EmailMismatchError", "Account");
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
                    claims?.Identities.First().AddClaim(new Claim(Constants.ApprenticeshipTitleClaimKey, apprenticeDetails.MyApprenticeship.Title ?? ""));

                    // Enable new UI
                    claims?.Identities.First().AddClaim(new Claim(Constants.NewUiEnabledClaimKey, "true"));
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
        public async Task<IActionResult> StubSignedIn()
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
            
            // 1473
            bool isCohort = IsUserInNewUiCohort(1);
            HttpContext.Session.SetString(CohortUserSessionKey, isCohort ? "true" : "false");

            // Check opt‑in cookie
            bool optIn = Request.Cookies[NewUiOptInCookieName] == "true";
            HttpContext.Session.SetString(OptInUserSessionKey, optIn ? "true" : "false");

            // Set legacy UserType for backward compatibility
            string userType = (optIn || isCohort) ? "SpecialUser" : "RegularUser";
            HttpContext.Session.SetString("UserType", userType);
      

            return RedirectToAction("Index", "Terms");
        }

        [HttpGet]
        public IActionResult AccountNotFound()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CmadError()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EmailMismatchError()
        {
            return View();
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult OptInNewUi(string returnUrl)
        {
            // Set cookie with long expiry (e.g., 1 year)
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
            Response.Cookies.Append(NewUiOptInCookieName, "true", cookieOptions);

            // Clear any force‑old‑UI flag (in case user previously opted out)
            HttpContext.Session.Remove(ForceOldUISessionKey);

            // Set opt‑in flag in session
            HttpContext.Session.SetString(OptInUserSessionKey, "true");
            HttpContext.Session.SetString("UserType", "SpecialUser");

            // Clear welcome splash screen cookie so user sees it again
            if (Request.Cookies[Constants.WelcomeSplashScreenCookieName] != null)
            {
                Response.Cookies.Delete(Constants.WelcomeSplashScreenCookieName);
            }

            _logger.LogInformation("User opted into new UI via button.");

            // Always redirect to Welcome screen to show the welcome message
            return RedirectToAction("Index", "Welcome");
        }

        [Authorize]
        [HttpGet]
        public IActionResult OptOutNewUi(string returnUrl)
        {
            // Delete the opt-in cookie by setting an expired cookie
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
            Response.Cookies.Append(NewUiOptInCookieName, "false", cookieOptions);

            // Set a session flag to force old UI for this session
            HttpContext.Session.SetString("ForceOldUI", "true");

            // Clear any opt-in session flag
            HttpContext.Session.SetString(OptInUserSessionKey, "false");
            HttpContext.Session.SetString("UserType", "RegularUser");

            _logger.LogInformation("User opted out of new UI.");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Tasks");
        }
        
        [HttpGet]
        public async Task<IActionResult> YourAccount()
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            var apprenticeAccountModel = new ApprenticeAccountModel();
            var apprenticeKsbsPageModel = new ApprenticeKsbsPageModel();


            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeKsbResult = await _client.GetApprenticeshipKsbs(new Guid(apprenticeId));

                if (Request.Cookies[Constants.KsbFiltersCookieName] != null)
                {
                    var filterKsbs = Filter.FilterKsbResults(
                        apprenticeKsbResult,
                        Request.Cookies[Constants.KsbFiltersCookieName]);

                    if (filterKsbs.HasFilterRun)
                    {
                        apprenticeKsbResult = filterKsbs.FilteredKsbs;
                    }
                }

                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                apprenticeKsbsPageModel.Ksbs = apprenticeKsbResult;
                apprenticeKsbsPageModel.KnowledgeCount = apprenticeKsbResult?.Count(k => k.Type == KsbType.Knowledge);
                apprenticeKsbsPageModel.SkillCount = apprenticeKsbResult?.Count(k => k.Type == KsbType.Skill);
                apprenticeKsbsPageModel.BehaviourCount = apprenticeKsbResult?.Count(k => k.Type == KsbType.Behaviour);
                apprenticeKsbsPageModel.SearchTerm = null;
                apprenticeKsbsPageModel.MyApprenticeship = apprenticeDetails?.MyApprenticeship;
            }

            apprenticeAccountModel.apprenticeKsbsPageModel = apprenticeKsbsPageModel;

            return View(apprenticeAccountModel);
        }
        
        private void SetUiforCohort(long? providerId)
        {
            bool isUserInNewUiCohort = IsUserInNewUiCohort(providerId.Value);
            string userType = isUserInNewUiCohort ? "SpecialUser" : "RegularUser";
            string logMessage = isUserInNewUiCohort 
                ? $"User provider Id: {providerId} identified as SpecialUser (New UI Cohort)"
                : $"User provider Id: {providerId} identified as RegularUser";
    
            HttpContext.Session.SetString("UserType", userType);
            _logger.LogInformation(logMessage);
        }

        public bool IsUserInNewUiCohort(long providerId) => 
            new long[] {  }.Contains(providerId);        
    }

}