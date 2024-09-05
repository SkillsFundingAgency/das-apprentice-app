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

        public KsbController(
            ILogger<KsbController> logger,
            IOuterApiClient client
            )
        {
            _logger = logger;
            _client = client;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);
                var standardUId = Claims.GetClaim(HttpContext, Constants.StandardUIdClaimKey);

                if (!string.IsNullOrEmpty(apprenticeshipId))
                {
                    //using default value of core until we have the correct value from Approvals api
                    var apprenticeKsbResult = await _client.GetApprenticeshipKsbs(long.Parse(apprenticeshipId), standardUId, "core");

                    ApprenticeKsbsPageModel apprenticeKsbsPageModel = new ApprenticeKsbsPageModel()

                    {
                        Ksbs = apprenticeKsbResult,
                        KnowledgeCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Knowledge),
                        SkillCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Skill),
                        BehaviourCount = apprenticeKsbResult.Count(k => k.Type == KsbType.Behaviour)
                    };

                    return View(apprenticeKsbsPageModel);
                }

                string message = $"Apprentice Details not found - 'apprenticeDetails' is null in Ksbs Index. ApprenticeId: {apprenticeId}";
                _logger.LogWarning(message);
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
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);
                var standardUId = Claims.GetClaim(HttpContext, Constants.StandardUIdClaimKey);

                if (!string.IsNullOrEmpty(apprenticeshipId))
                {
                    //using default value of core until we have the correct value from Approvals api
                    var apprenticeKsbResult = await _client.GetApprenticeshipKsbs(long.Parse(apprenticeshipId), standardUId, "core");
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

                string message = $"Apprentice Details not found - 'apprenticeDetails' is null in Ksbs LinkKsbs. ApprenticeId: {apprenticeId}";
                _logger.LogWarning(message);
            }
            else
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Ksbs LinkKsbs.");
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddUpdateKsbProgress(Guid id, string key, string detail)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);
                Guid[] guids = new Guid[1];
                guids[0] = id;

                var ksbProgressResult = await _client.GetApprenticeshipKsbProgresses(long.Parse(apprenticeshipId), guids);
                var vm = new EditKsbPageModel();
                vm.KsbDetail = detail;
                vm.KsbStatuses = KsbHelpers.KSBStatuses();

                if (ksbProgressResult != null && ksbProgressResult.Any(k => k.KsbId == id))
                {
                    var ksbProgress = ksbProgressResult.FirstOrDefault(k => k.KsbId == id);
                    vm.KsbProgress = ksbProgress;
                }
                else
                {
                    vm.KsbProgress = new ApprenticeKsbProgressData()
                    {
                        ApprenticeshipId = long.Parse(apprenticeshipId),
                        KsbId = id,
                        KsbKey = key,
                        Note = string.Empty,
                        CurrentStatus = KSBStatus.NotStarted,
                        KsbProgressType = KsbHelpers.GetKsbType(key),
                        Tasks = new List<ApprenticeTask>()
                    };
                }
                return View(vm);
            }

            string message = $"Invalid apprentice id for AddUpdateKsbProgress in KsbController";
            _logger.LogWarning(message);
            return View("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUpdateKsbProgress(ApprenticeKsbProgressData ksbProgressData)
        {
            if (ksbProgressData != null && ksbProgressData.ApprenticeshipId == 0)
            {
                var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

                if (!string.IsNullOrEmpty(apprenticeId))
                {
                    var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);
                    ksbProgressData.ApprenticeshipId = long.Parse(apprenticeshipId);
                    string message = $"AddUpdateKsbProgress for KSB {ksbProgressData.KsbId} and Apprenticeship: {ksbProgressData.ApprenticeshipId}";
                    _logger.LogInformation(message);

                    try
                    {
                        await _client.AddUpdateKsbProgress(ksbProgressData.ApprenticeshipId, ksbProgressData);
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
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);

                ApprenticeKsbProgressData ksbProgressData = new ApprenticeKsbProgressData()
                {
                    ApprenticeshipId = long.Parse(apprenticeshipId),
                    KsbId = ksbId,
                    KsbKey = ksbKey,
                    KsbProgressType = ksbType,
                    CurrentStatus = ksbStatus,
                    Note = note
                };

                try
                {
                    await _client.AddUpdateKsbProgress(ksbProgressData.ApprenticeshipId, ksbProgressData);
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

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveTaskFromKsbProgress(int progressId, int taskId)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = Claims.GetClaim(HttpContext, Constants.ApprenticeshipIdClaimKey);

                string preMessage = $"Removing Task {taskId} from KsbProgress {progressId}";
                _logger.LogInformation(preMessage);
                try
                {
                    await _client.RemoveTaskToKsbProgress(long.Parse(apprenticeshipId), progressId, taskId);
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