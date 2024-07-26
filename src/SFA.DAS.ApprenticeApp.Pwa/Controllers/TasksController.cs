using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class TasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult _ToDoPage()
        {
            return View();
        }

    }
   
}
