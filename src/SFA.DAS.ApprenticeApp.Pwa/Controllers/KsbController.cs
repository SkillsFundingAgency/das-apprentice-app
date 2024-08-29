﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.ApprenticeApp.Pwa.ViewHelpers;

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
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;
                var standardUId = HttpContext.User?.Claims?.First(c => c.Type == Constants.StandardUIdClaimKey)?.Value;

                if (apprenticeshipId != null)
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
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;
                var standardUId = HttpContext.User?.Claims?.First(c => c.Type == Constants.StandardUIdClaimKey)?.Value;

                if (apprenticeshipId != null)
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
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;
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
                var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

                if (!string.IsNullOrEmpty(apprenticeId))
                {
                    var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;
                    ksbProgressData.ApprenticeshipId = long.Parse(apprenticeshipId);
                    string message = $"AddUpdateKsbProgress for KSB {ksbProgressData.KsbId} and Apprenticeship: {ksbProgressData.ApprenticeshipId}";
                    _logger.LogInformation(message);
                    await _client.AddUpdateKsbProgress(ksbProgressData.ApprenticeshipId, ksbProgressData);

                    return RedirectToAction("Index", "Ksb");
                }
            }
            _logger.LogWarning("Invalid apprentice id for HttpPost method AddUpdateKsbProgress in KsbController");
            return View("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditKsbProgress(Guid ksbId, string ksbKey, KsbType ksbType, KSBStatus ksbStatus, string note)
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;
            
            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeshipId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeshipIdClaimKey)?.Value;

                ApprenticeKsbProgressData ksbProgressData = new ApprenticeKsbProgressData()
                {
                    ApprenticeshipId = long.Parse(apprenticeshipId),
                    KsbId = ksbId,
                    KsbKey = ksbKey,
                    KsbProgressType = ksbType,
                    CurrentStatus = ksbStatus,
                    Note = note
                }; 
                
                await _client.AddUpdateKsbProgress(ksbProgressData.ApprenticeshipId, ksbProgressData);
                return Ok();
            }

            return Unauthorized();
        }
    }
}