using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [Authorize]
    public class WelcomeController : Controller
    {
        private readonly ILogger<WelcomeController> _logger;
        private readonly ApplicationConfiguration _appConfig;

        public WelcomeController(ILogger<WelcomeController> logger,
            ApplicationConfiguration appConfig
        )
        {
            _logger = logger;
            _appConfig = appConfig;
        }

        public IActionResult Index()
        {
            // Feature for whitelist emails | "WhiteListEmails": "{ \"Emails\" : [\"user1@gmail.com\", \"user2@gmail.com\"] }"
            string whiteListEmailList = _appConfig.WhiteListEmails;
            if (whiteListEmailList != null)
            {
                WhiteListEmailUsers? users = JsonConvert.DeserializeObject<WhiteListEmailUsers>(whiteListEmailList);

                var apprenticeEmail = Claims.GetClaim(HttpContext, Constants.ApprenticeNameClaimKey);
                var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);
                
                var match = users?.Emails?.Contains(apprenticeEmail);
                if (match == null || match == false)
                { 
                   _logger.LogInformation($"Invalid Private Beta Phase 2 user tried to log in. ApprenticeId: {apprenticeId}");
                    return RedirectToAction("Error", "Account");
                }
                else
                {
                    _logger.LogInformation($"Valid Private Beta Phase 2 user logged in. ApprenticeId: {apprenticeId}");
                }
            }
            else
            {
                // Deny entry
                _logger.LogInformation($"Environment not configured for email whitelist");
                return RedirectToAction("Error", "Account");
            }
           
            var cookie = Request.Cookies[Constants.WelcomeSplashScreenCookieName];

            if (cookie == null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(99),
                    Path = "/",
                    Secure = true,
                    HttpOnly = true
                };
                Response.Cookies.Append(Constants.WelcomeSplashScreenCookieName, "1", cookieOptions);

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Profile");

            }
        }
    }
}
