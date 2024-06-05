using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class SupportController : Controller
    {
        [Route("support")]
        public IActionResult LandingPage()
        {
            return View();
        }
        
        [Route("support/content/{pagename?}")]
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
