using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            var cookie = Request.Cookies[Constants.WelcomeSplashScreenCookieName];

            if (cookie == null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(99),
                    Path = "/"
                };
                Response.Cookies.Append(Constants.WelcomeSplashScreenCookieName, "1", cookieOptions);

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Tasks");

            }
        }
    }
}
