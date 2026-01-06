using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
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

                var taskResult = await _client.GetApprenticeTasks(new Guid(apprenticeId), Constants.ToDoStatus, new DateTime(2000, 1, 1), new DateTime(2100, 12, 12));
                
                if (taskResult == null || taskResult.Tasks.Count == 0)
                {
                    return PartialView("_TasksNotStarted");
                }

                if (Request.Cookies[Constants.TaskFiltersTodoCookieName] != null)
                {
                    var filterTasks = Filter.FilterTaskResults(taskResult.Tasks, Request.Cookies[Constants.TaskFiltersTodoCookieName]);

                    if (filterTasks.HasFilterRun.Equals(true))
                    {
                        taskResult.Tasks = filterTasks.FilteredTasks;
                    }
                }

                // sorting
                var sortingValue = Request.Cookies[Constants.TaskFilterSortCookieName];
                if (sortingValue != null)
                {
                    taskResult.Tasks = sortingValue switch
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
                int yearValue = DateTime.Now.Year;
                var yearSet = Request.Cookies[Constants.TaskFilterYearCookieName];
                if (yearSet != null)
                {
                    yearValue = int.Parse(Request.Cookies[Constants.TaskFilterYearCookieName]);
                }

                var taskResult = await _client.GetApprenticeTasks(new Guid(apprenticeId), Constants.DoneStatus, new DateTime(2000, 1, 1), new DateTime(2100, 12, 1));

                if (taskResult == null || taskResult.Tasks.Count == 0)
                {
                    return PartialView("_TasksNotStarted");
                }

                if (Request.Cookies[Constants.TaskFiltersDoneCookieName] != null)
                {
                    var filterTasks = Filter.FilterTaskResults(taskResult.Tasks, Request.Cookies[Constants.TaskFiltersDoneCookieName]);

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
                    var taskdata = await _client.GetTaskViewData(new Guid(apprenticeId), id);

                    var guids = taskdata.KsbProgress?.Select(k => k.KsbId).ToList() ?? new List<Guid>();
                    var vm = new EditTaskPageModel
                    {
                        Task = taskdata.Task,
                        Categories = taskdata.TaskCategories?.TaskCategories,
                        KsbProgressData = taskdata.KsbProgress,
                        LinkedKsbGuids = guids.Any() ? string.Join(",", guids) : string.Empty,
                        StatusId = status
                    };

                    if(taskdata.Task.TaskReminders != null && taskdata.Task.TaskReminders.Count == 1)
                    {
                        vm.Task.ReminderValue = taskdata.Task.TaskReminders.FirstOrDefault().ReminderValue;
                    }
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
                if (!string.IsNullOrEmpty(task.Title))
                {
                    task.Title = ViewHelpers.Helpers.StripHTML(task.Title);
                }

                if (!string.IsNullOrEmpty(task.Note))
                {
                    task.Note = ViewHelpers.Helpers.StripHTML(task.Note);
                }

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
                    if (task.Status == Domain.Models.TaskStatus.Done)
                    {
                        task.CompletionDateTime = task.CompletionDateTime.Value.Date + TimeSpan.Parse(HttpContext.Request.Form["time"]);
                    }
                    else
                    {
                        task.DueDate = task.DueDate.Value.Date + TimeSpan.Parse(HttpContext.Request.Form["time"]);
                    }
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
            return PartialView("_TasksNotStarted");
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

                    task.ApprenticeAccountId = new Guid(apprenticeId);
                    task.DueDate += TimeSpan.Parse(HttpContext.Request.Form["time"]);
                    task.ApprenticeshipCategoryId ??= 1;

                    if (!string.IsNullOrEmpty(task.Title))
                    {
                        task.Title = ViewHelpers.Helpers.StripHTML(task.Title);
                    }

                    if (!string.IsNullOrEmpty(task.Note))
                    {
                        task.Note = ViewHelpers.Helpers.StripHTML(task.Note);
                    }

                    if (task.Status == Domain.Models.TaskStatus.Done)
                    {
                        task.CompletionDateTime = task.DueDate;
                    }
                    if (task.CompletionDateTime == null)
                    {
                        task.CompletionDateTime = task.DueDate;
                    }

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

                return Ok();
            }
            return Ok();
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

            return Ok();
        }
    }
}