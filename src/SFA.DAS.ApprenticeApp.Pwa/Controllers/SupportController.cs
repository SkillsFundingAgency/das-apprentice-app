using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult LandingPage()
        {
            return View();
        }
        public IActionResult SecondLevelPage()
        {
            return View();
        }
        public IActionResult SavedArticlesPage()
        {
            return View();
        }
    }

}
