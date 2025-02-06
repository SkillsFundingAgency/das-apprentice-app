using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [Authorize]
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
