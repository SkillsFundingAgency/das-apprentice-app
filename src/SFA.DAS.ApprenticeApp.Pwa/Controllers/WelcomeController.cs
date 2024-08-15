using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            var cookieName = "ApprenticeAppWelcomeSplashViewed";

            var cookie = Request.Cookies[cookieName];

            if (cookie == null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(99),
                    Path = "/"
                };
                Response.Cookies.Append(cookieName, "seen", cookieOptions);

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Tasks");

            }
        }
    }
}
