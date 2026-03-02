using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [Authorize]
    public class WelcomeController : Controller
    {
        
        public WelcomeController() { }
        
        public IActionResult Index()
        {
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
                var isNewUi = Claims.GetClaim(HttpContext, Constants.NewUiEnabledClaimKey);
                if (string.Equals(isNewUi, "true", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Ksb");
                }
                return RedirectToAction("Index", "Tasks");
            }
        }
    }
}
