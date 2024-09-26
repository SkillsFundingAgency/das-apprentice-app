using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using System.Threading.Tasks;

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
        public IActionResult Index(string sort, int year)
        {
            int yearSet = DateTime.Now.Year;
            string sortSet = "date_due";

            var yearCookie = Request.Cookies[Constants.TaskFilterYearCookieName];
            var sortCookie = Request.Cookies[Constants.TaskFilterSortCookieName];

            if (yearCookie != null)
            {
                yearSet = int.Parse(Request.Cookies[Constants.TaskFilterYearCookieName]);
            }

            if (sortCookie != null)
            {
                sortSet = Request.Cookies[Constants.TaskFilterSortCookieName];
            }

            if (!string.IsNullOrEmpty(sort))
            {
                sortSet = sort;

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(99),
                    Path = "/",
                    Secure = true,
                    HttpOnly = true
                };
                Response.Cookies.Append(Constants.TaskFilterSortCookieName, sort, cookieOptions);
            }

            if (year > 0)
            {
                yearSet = year;

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(99),
                    Path = "/",
                    Secure = true,
                    HttpOnly = true
                };
                Response.Cookies.Append(Constants.TaskFilterYearCookieName, year.ToString(), cookieOptions); 
            }

            TasksBaseModel vm = new()
            {
                Year = yearSet,
                Sort = sortSet
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<PartialViewResult> ToDoTasks()
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                int year = DateTime.Now.Year;
                var yearCookie = Request.Cookies[Constants.TaskFilterYearCookieName];
                if (yearCookie != null)
                {
                    year = int.Parse(Request.Cookies[Constants.TaskFilterYearCookieName]);
                }

                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);
                var taskResult = await _client.GetApprenticeTasks(long.Parse(apprenticeshipId), Constants.ToDoStatus, new DateTime(year, 1, 1), new DateTime(year, 12, 31));

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

                // sorting
                var sortCookie = Request.Cookies[Constants.TaskFilterSortCookieName];
                if (sortCookie != null)
                {
                    taskResult.Tasks = sortCookie switch
                    {
                        "due_date" => taskResult.Tasks.OrderBy(x => x.DueDate).ToList(),
                        "recently_added" => taskResult.Tasks.OrderByDescending(x => x.TaskId).ToList(),
                        _ => taskResult.Tasks.OrderBy(x => x.DueDate).ToList(),
                    };
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
                int year = DateTime.Now.Year;
                var yearCookie = Request.Cookies[Constants.TaskFilterYearCookieName];
                if (yearCookie != null)
                {
                    year = int.Parse(Request.Cookies[Constants.TaskFilterYearCookieName]);
                }

                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);

                var taskResult = await _client.GetApprenticeTasks(long.Parse(apprenticeshipId), Constants.DoneStatus, new DateTime(year, 1, 1), new DateTime(year, 12, 31));

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

                // sorting
                var sortCookie = Request.Cookies[Constants.TaskFilterSortCookieName];
                if (sortCookie != null)
                {
                    taskResult.Tasks = sortCookie switch
                    {
                        "due_date" => taskResult.Tasks.OrderBy(x => x.DueDate).ToList(),
                        "recently_added" => taskResult.Tasks.OrderByDescending(x => x.TaskId).ToList(),
                        _ => taskResult.Tasks.OrderBy(x => x.DueDate).ToList(),
                    };
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
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);
                var standardUId = Claims.GetClaim(HttpContext, Constants.StandardUIdClaimKey);

                if (!string.IsNullOrEmpty(apprenticeshipId) && !string.IsNullOrEmpty(standardUId))
                {
                    //using default value of core until we have the correct value from Approvals api
                    var taskdata = await _client.GetTaskViewData(long.Parse(apprenticeshipId), id, standardUId, "core");

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

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(ApprenticeTask task)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);

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
                    await _client.UpdateApprenticeTask(long.Parse(apprenticeshipId), task.TaskId, task);
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
                var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;
                var categories = await _client.GetTaskCategories(long.Parse(apprenticeshipId));

                var vm = new AddTaskPageModel
                {
                    Task = new ApprenticeTask() { ApprenticeshipId = long.Parse(apprenticeshipId) },
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
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);

                if (long.Parse(apprenticeshipId) != task.ApprenticeshipId)
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
                    await _client.AddApprenticeTask(long.Parse(apprenticeshipId), task);
                }
                catch
                {
                    //temporarily handle 500 errors
                }

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

            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;

                string preMessage = $"Deleting task with id {taskId}";
                _logger.LogInformation(preMessage);

                await _client.DeleteApprenticeTask(long.Parse(apprenticeshipId), taskId);
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
                var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;

                await _client.UpdateTaskStatus(long.Parse(apprenticeshipId), taskId, statusId);

                return RedirectToAction("Index");
            }
            
            return Unauthorized();
        }
    }
}