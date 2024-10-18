using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [Authorize]
    public class WelcomeController : Controller
    {
        private readonly ILogger<WelcomeController> _logger;
        private readonly IConfiguration _config;

        public WelcomeController(ILogger<WelcomeController> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _config = configuration;
        }

        public IActionResult Index()
        {
            // Feature for whitelist emails | "WhiteListEmails": "{ \"Emails\" : [\"user1@gmail.com\", \"user2@gmail.com\"] }"
            string whiteListEmailList = _config["WhiteListEmails"];
            if (whiteListEmailList != null)
            {
                WhiteListEmailUsers? users = JsonConvert.DeserializeObject<WhiteListEmailUsers>(whiteListEmailList);

                var apprenticeEmail = Claims.GetClaim(HttpContext, "name");
                var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

                var match = users?.Emails?.Contains(apprenticeEmail);
                if (match == null)
                { 
                   _logger.LogInformation($"white list user logged. ApprenticeId: {apprenticeId}");

                    // Deny entry
                    return RedirectToAction("Error", "Account");
                }
            }
            else
            {
                // Deny entry
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
