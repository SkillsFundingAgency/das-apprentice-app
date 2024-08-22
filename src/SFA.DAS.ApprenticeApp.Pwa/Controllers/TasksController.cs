using System.Net.NetworkInformation;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<PartialViewResult> ToDoTasks()
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                var taskResult = await _client.GetApprenticeTasks(apprenticeDetails.MyApprenticeship.ApprenticeshipId, Constants.ToDoStatus, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31));

                if (taskResult == null || taskResult.Tasks.Count == 0)
                {
                    return PartialView("_TasksNotStarted");
                }

                var vm = new TasksPageModel
                {
                    Year = DateTime.Now.Year,
                    Tasks = taskResult.Tasks,
                };
                return PartialView("_TasksToDo", vm);
            }
            return PartialView("_TasksNotStarted");
        }

        [HttpGet]
        [Authorize]
        public async Task<PartialViewResult> DoneTasks()
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                var taskResult = await _client.GetApprenticeTasks(apprenticeDetails.MyApprenticeship.ApprenticeshipId, Constants.DoneStatus, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31));

                if (taskResult == null || taskResult.Tasks.Count == 0)
                {
                    return PartialView("_TasksNotStarted");
                }

                var vm = new TasksPageModel
                {
                    Year = DateTime.Now.Year,
                    Tasks = taskResult.Tasks,
                };
                return PartialView("_TasksDone", vm);
            }
            return PartialView("_TasksNotStarted");
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

                return RedirectToAction("Edit", "Tasks", task.TaskId);
            }

            return View();
        }

        public IActionResult TasksNotStarted()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Add(int status = 0)
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                var categories = await _client.GetTaskCategories(apprenticeDetails.MyApprenticeship.ApprenticeshipId);

                var vm = new AddTaskPageModel
                {
                    Task = new ApprenticeTask() { ApprenticeshipId = apprenticeDetails.MyApprenticeship.ApprenticeshipId},
                    Categories = categories.TaskCategories,
                    StatusId = status
                };

                return View(vm);
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(ApprenticeTask task)
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                if (apprenticeDetails.MyApprenticeship.ApprenticeshipId != task.ApprenticeshipId)
                {
                    _logger.LogWarning("Invalid apprenticeship id. Cannot add task.");
                    return RedirectToAction("Index", "Tasks");
                }

                    task.CompletionDateTime = DateTime.UtcNow;

                    string preMessage = $"Adding new task for apprentice with id {apprenticeId}";
                    _logger.LogInformation(preMessage);

                    await _client.AddApprenticeTask(apprenticeDetails.MyApprenticeship.ApprenticeshipId, task);

                    string postMessage = $"Task added successfully for apprentice with id {apprenticeId}";
                    _logger.LogInformation(postMessage);
                    return RedirectToAction("Index", new { status = (int)task.Status });

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



        [HttpGet]
        public async Task<IActionResult> ChangeTaskStatus(int taskId, int statusId)
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

                await _client.UpdateTaskStatus(apprenticeDetails.MyApprenticeship.ApprenticeshipId, taskId, statusId);


                return RedirectToAction("Index");
            }
            return Unauthorized();
        }
    }
}