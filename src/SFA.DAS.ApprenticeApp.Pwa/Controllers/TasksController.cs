﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;

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
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                    var taskResult = await _client.GetApprenticeTasks(new Guid(apprenticeId), Constants.ToDoStatus, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 12));

                    if (taskResult == null || taskResult.Tasks.Count == 0)
                    {
                        return PartialView("_TasksNotStarted");
                    }

                    if (Request.Cookies[Constants.TaskFiltersCookieName] != null)
                    {
                        var filterTasks = Filter.FilterTaskResults(taskResult.Tasks, Request.Cookies[Constants.TaskFiltersCookieName]);

                        if (filterTasks.HasFilterRun.Equals(true))
                        {
                            taskResult.Tasks = filterTasks.FilteredTasks;
                        }
                    }

                    var vm = new TasksPageModel
                    {
                        Year = DateTime.Now.Year,
                        Tasks = taskResult.Tasks

                    };
                    return PartialView("_TasksToDo", vm);
                }
            return PartialView("_TasksNotStarted");
        }

        [HttpGet]
        [Authorize]
        public async Task<PartialViewResult> DoneTasks()
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                    var taskResult = await _client.GetApprenticeTasks(new Guid(apprenticeId), Constants.DoneStatus, new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 12));

                    if (taskResult == null || taskResult.Tasks.Count == 0)
                    {
                        return PartialView("_TasksNotStarted");
                    }

                    if (Request.Cookies[Constants.TaskFiltersCookieName] != null)
                    {
                        var filterTasks = Filter.FilterTaskResults(taskResult.Tasks, Request.Cookies[Constants.TaskFiltersCookieName]);

                        if (filterTasks.HasFilterRun.Equals(true))
                        {
                            taskResult.Tasks = filterTasks.FilteredTasks;
                        }
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
        public async Task<IActionResult> Edit(int id, int status = 0)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                
                    var taskdata = await _client.GetTaskViewData(new Guid(apprenticeId), id);

                    var guids = taskdata.KsbProgress.Select(k => k.KsbId).ToList();
                    var vm = new EditTaskPageModel
                    {
                        Task = taskdata.Task,
                        Categories = taskdata.TaskCategories.TaskCategories,
                        KsbProgressData = taskdata.KsbProgress,
                        LinkedKsbGuids = String.Join(",", guids),
                        StatusId = status
                    };
                    return View(vm);
                }

            return RedirectToAction("Index", "Tasks");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(ApprenticeTask task)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                    if (task.KsbsLinked != null)
                    {
                        if (task.KsbsLinked[0] != null)
                        {
                            string[] ksbArray = task.KsbsLinked[0].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            task.KsbsLinked = ksbArray;
                        }
                    }
                    try
                    {
                        await _client.UpdateApprenticeTask(new Guid(apprenticeId), task.TaskId, task);
                    }
                    catch
                    {
                        //temporarily handle 500 errors;
                    }

                    return RedirectToAction("Index", new { status = (int)task.Status });
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
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                if (apprenticeDetails.MyApprenticeship != null)
                {
                    var apprenticeshipId = apprenticeDetails.MyApprenticeship.ApprenticeshipId.ToString();

                    var categories = await _client.GetTaskCategories(new Guid(apprenticeId));

                    var vm = new AddTaskPageModel
                    {
                        Task = new ApprenticeTask() { ApprenticeshipId = long.Parse(apprenticeshipId) },
                        Categories = categories.TaskCategories,
                        StatusId = status
                    };

                    return View(vm);
                }
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(ApprenticeTask task)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                if (apprenticeDetails.MyApprenticeship != null)
                {
                    var apprenticeshipId = apprenticeDetails.MyApprenticeship.ApprenticeshipId;
                    if (apprenticeshipId != task.ApprenticeshipId)
                    {
                        _logger.LogWarning("Invalid apprenticeship id. Cannot add task.");
                        return RedirectToAction("Index", "Tasks");
                    }

                    if (task.Status == Domain.Models.TaskStatus.Done)
                    {
                        task.CompletionDateTime = DateTime.UtcNow;
                    }
                    if (task.CompletionDateTime == null)
                    {
                        task.CompletionDateTime = DateTime.UtcNow;
                    }

                    task.ApprenticeshipCategoryId ??= 1;

                    if (task.KsbsLinked != null)
                    {
                        if (task.KsbsLinked[0] != null)
                        {
                            string[] ksbArray = task.KsbsLinked[0].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            task.KsbsLinked = ksbArray;
                        }
                    }

                    string preMessage = $"Adding new task for apprentice with id {apprenticeId}";
                    _logger.LogInformation(preMessage);

                    try
                    {
                        await _client.AddApprenticeTask(apprenticeshipId, task);
                    }
                    catch
                    {
                        //temporarily handle 500 errors
                    }

                    string postMessage = $"Task added successfully for apprentice with id {apprenticeId}";
                    _logger.LogInformation(postMessage);
                    return RedirectToAction("Index", new { status = (int)task.Status });
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

            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                string preMessage = $"Deleting task with id {taskId}";
                _logger.LogInformation(preMessage);

                await _client.DeleteApprenticeTask(new Guid(apprenticeId), taskId);
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
                _logger.LogWarning("Task Id cannot be null or zero. Cannot change task status.");
                return RedirectToAction("Index", "Tasks");
            }

            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                await _client.UpdateTaskStatus(new Guid(apprenticeId), taskId, statusId);

                return RedirectToAction("Index");
            }
            
            return Unauthorized();
        }
    }
}