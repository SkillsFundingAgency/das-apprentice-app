﻿using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class TasksController : Controller
    {
        public IActionResult Tasks()
        {
            return View();
        }
    }
}