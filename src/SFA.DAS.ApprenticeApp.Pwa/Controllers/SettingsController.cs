using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController
            (
            ILogger<SettingsController> logger
            )
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Settings page loaded.");
            return View();
        }
    }
}
