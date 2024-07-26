using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [AllowAnonymous]
    public class TasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ToDoPartial()
        {
            return View();
        }

        public IActionResult DonePartial()
        {
            return View();
        }

        public IActionResult AddTaskPartial()
        {
            return View();
        }

        public IActionResult EditPartial()
        {
            return View();
        }

        public IActionResult KSBPartial()
        {
            return View();
        }

        public IActionResult FiltersPartial()
        {
            return View();
        }

    }
   
}
