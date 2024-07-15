using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Welcome";
            return View();
        }
    }
}


