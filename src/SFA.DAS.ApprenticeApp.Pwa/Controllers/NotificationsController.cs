using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
