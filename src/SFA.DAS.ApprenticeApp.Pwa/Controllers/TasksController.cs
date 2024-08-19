using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ILogger<TasksController> _logger;
        private readonly IOuterApiClient _client;
        public const long ApprenticeshipId = 123;

        public TasksController
            (
            ILogger<TasksController> logger,
            IOuterApiClient client
            )
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var year = DateTime.Now.Year;
            DateTime fromDate = new DateTime(year, 1, 1);
            DateTime toDate = new DateTime(year, 12, 12); //ToDo: cg - fix date issue

            var tasksResult = await _client.GetApprenticeTasks(ApprenticeshipId, 0, fromDate, toDate);

            if (tasksResult.Tasks == null || tasksResult.Tasks.Count == 0)
            {
                return RedirectToAction("TasksNotStarted", "Tasks");
            }
            var tasksData = new TasksPageModel
            {
                Year = year,
                Tasks = tasksResult.Tasks
            };

            return View(tasksData);

        }

        public IActionResult TasksNotStarted()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateApprenticeTask(ApprenticeTask task)
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                if (task.TaskId == 0)
                {
                    string preMessage = $"Adding new task for apprentice with id {apprenticeId}";
                    _logger.LogInformation(preMessage);
                    await _client.AddApprenticeTask(ApprenticeshipId, task);
                    string postMessage = $"Task added successfully for apprentice with id {apprenticeId}";
                    _logger.LogInformation(postMessage);
                    return Ok();
                }
                else {                     
                    string preMessage = $"Updating task with id {task.TaskId} for apprentice with id {apprenticeId}";
                    _logger.LogInformation(preMessage);
                    await _client.UpdateApprenticeTask(ApprenticeshipId, task.TaskId, task);
                    string postMessage = $"Task updated successfully with id {task.TaskId} for apprentice with id {apprenticeId}";
                    _logger.LogInformation(postMessage);
                    return Ok();
                }
            }
            return Unauthorized();
        }


        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteApprenticeTask (int taskId)
        {
            if(taskId == 0)
            {
                _logger.LogWarning("Task Id cannot be null or zero. Cannot delete task.");
                return RedirectToAction("Index", "Tasks");
            }

            string  message = $"Deleting task with id {taskId}";
            _logger.LogInformation(message);

            await _client.DeleteApprenticeTask(ApprenticeshipId, taskId);
            return RedirectToAction("Index", "Tasks");
        }
    }
}