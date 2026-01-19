using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.ApprenticeApp.Pwa.ViewHelpers;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class KsbController : Controller
    {
        private readonly ILogger<KsbController> _logger;
        private readonly IOuterApiClient _client;
        private readonly IApprenticeContext _apprenticeContext;

        public KsbController(
            ILogger<KsbController> logger,
            IOuterApiClient client,
            IApprenticeContext apprenticeContext
            )
        {
            _logger = logger;
            _client = client;
            _apprenticeContext = apprenticeContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeKsbResult = await _client.GetApprenticeshipKsbs(new Guid(apprenticeId));
                
                if(apprenticeKsbResult == null || apprenticeKsbResult.Count == 0 || apprenticeKsbResult.Any(k => string.IsNullOrEmpty(k.Key)))
                {
                    string noKsbMessage = $"No KSBs found for {apprenticeId} in KsbController Index.";
                    _logger.LogWarning(noKsbMessage);
                    return View("NoKsbs");
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    apprenticeKsbResult = apprenticeKsbResult
                        .Where(ksb => ksb.Detail.ToLower().Contains(searchTerm.ToLower()) || ksb.Key.ToLower().Contains(searchTerm.ToLower()))
                        .ToList();
                    
                    foreach (ApprenticeKsb item in apprenticeKsbResult)
                    {
                        item.Key = Regex.Replace(item.Key, searchTerm, "<span style='background-color: yellow'>$0</span>", RegexOptions.IgnoreCase);
                        item.Detail = Regex.Replace(item.Detail, searchTerm, "<span style='background-color: yellow'>$0</span>", RegexOptions.IgnoreCase);
                    }
                }
                
                if (Request.Cookies[Constants.KsbFiltersCookieName] != null)
                    {
                        var filterKsbs = Filter.FilterKsbResults(apprenticeKsbResult, Request.Cookies[Constants.KsbFiltersCookieName]);

                        if (filterKsbs.HasFilterRun.Equals(true))
                        {
                            apprenticeKsbResult = filterKsbs.FilteredKsbs;
                        }
                    }

                    ApprenticeKsbsPageModel apprenticeKsbsPageModel = new()
                    {
                        Ksbs = apprenticeKsbResult,
                        KnowledgeCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Knowledge),
                        SkillCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Skill),
                        BehaviourCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Behaviour),
                        SearchTerm = searchTerm
                    };

                    return View(apprenticeKsbsPageModel);
            }
            else
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Ksbs Index.");
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LinkKsbs()
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeKsbResult = await _client.GetApprenticeshipKsbs(new Guid(apprenticeId));

                if (apprenticeKsbResult == null || apprenticeKsbResult.Count == 0 || apprenticeKsbResult.Any(k => string.IsNullOrEmpty(k.Key)))
                {
                    string noKsbMessage = $"No KSBs found for {apprenticeId} in KsbController LinkKsbs.";
                    _logger.LogWarning(noKsbMessage);
                    return View("_LinkNoKsbs");
                }

                ApprenticeKsbsPageModel apprenticeKsbsPageModel = new ApprenticeKsbsPageModel()

                    {
                        Ksbs = apprenticeKsbResult,
                        KnowledgeCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Knowledge),
                        SkillCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Skill),
                        BehaviourCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Behaviour),
                        KsbStatuses = KsbHelpers.KSBStatuses()
                    };

                    return View("_LinkKsb", apprenticeKsbsPageModel);
            }
            else
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Ksbs LinkKsbs.");
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddUpdateKsbProgress(Guid id)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var ksbResult = await _client.GetApprenticeshipKsb(new Guid(apprenticeId), id);
                var vm = new EditKsbPageModel();
                    vm.KsbStatuses = KsbHelpers.KSBStatuses();

                    if (ksbResult != null)
                    {
                        if (ksbResult.Progress != null)
                        {
                            vm.KsbProgress = ksbResult.Progress;
                        }
                        else
                        {
                        var apprenticeDetails = _client.GetApprenticeDetails(new Guid(apprenticeId));
                        var apprenticeshipId = apprenticeDetails.Result.MyApprenticeship.ApprenticeshipId;
                        vm.KsbProgress = new ApprenticeKsbProgressData()
                            {
                                ApprenticeshipId = apprenticeshipId,
                                KsbId = id,
                                KsbKey = ksbResult.Key,
                                Note = string.Empty,
                                CurrentStatus = KSBStatus.NotStarted,
                                KsbProgressType = KsbHelpers.GetKsbType(ksbResult.Key),
                                Tasks = new List<ApprenticeTask>()
                            };
                        }
                        vm.KsbDetail = ksbResult.Detail;
                    }
                    else
                    {
                        string noKsbMessage = $"Ksb not found for AddUpdateKsbProgress in KsbController";
                        _logger.LogWarning(noKsbMessage);
                        return View("Index");
                    }

                    return View(vm);
            }

            string noApprMessage = $"Invalid apprenticeId for AddUpdateKsbProgress in KsbController";
            _logger.LogWarning(noApprMessage);
            return Unauthorized();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUpdateKsbProgress(ApprenticeKsbProgressData ksbProgressData)
        {
            if (ksbProgressData != null && ksbProgressData.ApprenticeshipId == 0)
            {
                var apprenticeId = _apprenticeContext.ApprenticeId;

                if (!string.IsNullOrEmpty(apprenticeId))
                {
                    string message = $"AddUpdateKsbProgress for KSB {ksbProgressData.KsbId} and Apprenticeship: {ksbProgressData.ApprenticeshipId}";
                    _logger.LogInformation(message);

                    try
                    {
                        await _client.AddUpdateKsbProgress(new Guid(apprenticeId), ksbProgressData);
                    }
                    catch
                    {
                        //temporarily handle any 500 errors
                    }

                    return RedirectToAction("Index", "Ksb");
                }

                _logger.LogWarning("Invalid apprentice id for HttpPost method AddUpdateKsbProgress in KsbController");
                return Unauthorized();
            }
            return View("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditKsbProgress(Guid ksbId, string ksbKey, KsbType ksbType, KSBStatus ksbStatus, string note)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                if (apprenticeDetails != null && apprenticeDetails.MyApprenticeship != null)
                {
                    var apprenticeshipId = apprenticeDetails.MyApprenticeship.ApprenticeshipId;

                    ApprenticeKsbProgressData ksbProgressData = new ApprenticeKsbProgressData()
                    {
                        ApprenticeshipId = apprenticeshipId,
                        KsbId = ksbId,
                        KsbKey = ksbKey,
                        KsbProgressType = ksbType,
                        CurrentStatus = ksbStatus,
                        Note = note
                    };

                    try
                    {
                        await _client.AddUpdateKsbProgress(new Guid(apprenticeId), ksbProgressData);
                    }
                    catch
                    {
                        //temporarily handle any 500 errors
                    }

                    return Ok();
                }
            }

            _logger.LogWarning("Invalid apprentice id for method EditKsbProgress in KsbController");
            return Unauthorized();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveTaskFromKsbProgress(int progressId, int taskId)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
               

                string preMessage = $"Removing Task {taskId} from KsbProgress {progressId}";
                _logger.LogInformation(preMessage);
                try
                {
                    await _client.RemoveTaskToKsbProgress(new Guid(apprenticeId), progressId, taskId);
                    string postMessage = $"Removed Task {taskId} from KsbProgress {progressId}";
                    _logger.LogInformation(postMessage);
                }
                catch
                {
                    //temporarily handle any 500 errors
                }

                return Ok();
            }

            _logger.LogWarning("Invalid apprentice id for method EditKsbProgress in KsbController");
            return Unauthorized();
        }
    }
}