using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class TasksController : Controller
    {
        private readonly ILogger<TasksController> _logger;
        private readonly IOuterApiClient _client;

        public TasksController
            (
            ILogger<TasksController> logger,
            IOuterApiClient client
            )
        {
            _logger = logger;
            _client = client;
        }
        public async Task<IActionResult> Index()
        {
            //check apprenticeshipId
            var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;
            //Default to current year for testing

            DateTime fromDate = new DateTime(2024, 1, 1);
            DateTime toDate = new DateTime(2024, 12,31);
            var tasksResult = await _client.GetApprenticeTasks(new Guid(apprenticeshipId), 0, fromDate, toDate);
            var tasksData = new TasksPageModel
            {
                TasksData = tasksResult.Tasks
            };

            return View(tasksData);
        }
    }
}