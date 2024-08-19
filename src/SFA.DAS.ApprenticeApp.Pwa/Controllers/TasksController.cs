using Markdig.Parsers;
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
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                var taskTodoResult = await _client.GetApprenticeTasks(apprenticeDetails.MyApprenticeship.ApprenticeshipId, 0, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 12));
                var taskDoneResult = await _client.GetApprenticeTasks(apprenticeDetails.MyApprenticeship.ApprenticeshipId, 1, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 12));

                if (taskTodoResult == null || taskTodoResult.Tasks.Count == 0)
                {
                    return RedirectToAction("TasksNotStarted", "Tasks");
                }

                var vm = new TasksPageModel
                {
                    Year = DateTime.Now.Year,
                    TasksTodo = taskTodoResult.Tasks,
                    TasksDone = taskDoneResult.Tasks
                };
              
                return View(vm);
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                var task = await _client.GetApprenticeTaskById(apprenticeDetails.MyApprenticeship.ApprenticeshipId, id);
                var categories = await _client.GetTaskCategories(id);

                var vm = new EditTaskPageModel
                {
                    Task = task.Tasks.FirstOrDefault(),
                    Categories = categories.TaskCategories
                };

                return View(vm);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(ApprenticeTask task)
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                await _client.UpdateApprenticeTask(apprenticeDetails.MyApprenticeship.ApprenticeshipId, task.TaskId, task);

                return View();
            }

            return View();
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
                    var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                    if (task.TaskId == 0)
                {
                    string preMessage = $"Adding new task for apprentice with id {apprenticeId}";
                    _logger.LogInformation(preMessage);
                    await _client.AddApprenticeTask(apprenticeDetails.MyApprenticeship.ApprenticeshipId, task);
                    string postMessage = $"Task added successfully for apprentice with id {apprenticeId}";
                    _logger.LogInformation(postMessage);
                    return Ok();
                }
                else {                     
                    string preMessage = $"Updating task with id {task.TaskId} for apprentice with id {apprenticeId}";
                    _logger.LogInformation(preMessage);
                    await _client.UpdateApprenticeTask(apprenticeDetails.MyApprenticeship.ApprenticeshipId, task.TaskId, task);
                    string postMessage = $"Task updated successfully with id {task.TaskId} for apprentice with id {apprenticeId}";
                    _logger.LogInformation(postMessage);
                    return Ok();
                }
            }
            return Unauthorized();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteApprenticeTask(int taskId)
        {
            if (taskId == 0)
            {
                _logger.LogWarning("Task Id cannot be null or zero. Cannot delete task.");
                return RedirectToAction("Index", "Tasks");
            }

            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                string preMessage = $"Deleting task with id {taskId}";
                _logger.LogInformation(preMessage);

                await _client.DeleteApprenticeTask(apprenticeDetails.MyApprenticeship.ApprenticeshipId, taskId);
                string postMessage = $"Deleting task with id {taskId}";
                _logger.LogInformation(postMessage);

                return RedirectToAction("Index");
            }
            return Unauthorized();
        }
    }
}