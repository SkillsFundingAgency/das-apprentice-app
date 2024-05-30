using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult LandingPage()
        {
            return View();
        }
        // GET: Support/Content/{pageName}
        public IActionResult SecondLevel()
        {
            // Use the pageName parameter as needed
            // For example, you could fetch content based on pageName
            //ViewBag.PageName = pageName;
            return View();
        }
    
    public IActionResult SavedArticlesPage()
        {
            return View();
        }
    }


}
