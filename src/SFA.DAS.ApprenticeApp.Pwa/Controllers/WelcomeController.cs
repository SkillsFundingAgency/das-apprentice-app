using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [Authorize]
    public class WelcomeController : Controller
    {
        public WelcomeController() { }

        [Route("~/Welcome/{step:int?}")]
        public IActionResult Index(int? step = null)
        {
            var cookie = Request.Cookies[Constants.WelcomeSplashScreenCookieName];

            // The cookie records that the tour has already been shown, so arriving at
            // /Welcome again moves the apprentice on. A step in the URL is an explicit
            // request for that screen, which is how Next and Back work from screen two
            // onwards - by then the cookie has been set.
            if (cookie != null && step == null)
            {
                return RedirectToAction("Index", "Ksb");
            }

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
            }

            var number = step ?? 1;

            if (!WelcomeSteps.IsValid(number))
            {
                return RedirectToAction("Index", new { step = 1 });
            }

            return View(new WelcomePageModel
            {
                Step = WelcomeSteps.All[number - 1],
                Number = number,
                Total = WelcomeSteps.Count
            });
        }
    }
}
