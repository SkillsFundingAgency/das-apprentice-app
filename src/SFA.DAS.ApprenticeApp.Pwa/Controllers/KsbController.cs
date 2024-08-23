using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

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
        public async Task<IActionResult> Index()
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                if (apprenticeDetails.MyApprenticeship != null)
                {
                    //using default value of core until we have the correct value from Approvals api
                    var apprenticeKsbResult = await _client.GetApprenticeshipKsbs(apprenticeDetails.MyApprenticeship.ApprenticeshipId, apprenticeDetails.MyApprenticeship.StandardUId, "core");
                    ApprenticeKsbsPageModel apprenticeKsbsPageModel = new ApprenticeKsbsPageModel()

                    {
                        Ksbs = apprenticeKsbResult.AllKsbs,
                        KnowledgeCount = apprenticeKsbResult.AllKsbs.Count(k => k.Type == Domain.Models.KsbType.Knowledge),
                        SkillCount = apprenticeKsbResult.AllKsbs.Count(k => k.Type == Domain.Models.KsbType.Skill),
                        BehaviourCount = apprenticeKsbResult.AllKsbs.Count(k => k.Type == Domain.Models.KsbType.Behaviour),
                        KsbProgresses = apprenticeKsbResult.KsbProgresses
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
        public async Task<IActionResult> LinkKsbs()
        {
            var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                if (apprenticeDetails.MyApprenticeship != null)
                {
                    //using default value of core until we have the correct value from Approvals api
                    var apprenticeKsbResult = await _client.GetApprenticeshipKsbs(apprenticeDetails.MyApprenticeship.ApprenticeshipId, apprenticeDetails.MyApprenticeship.StandardUId, "core");
                    ApprenticeKsbsPageModel apprenticeKsbsPageModel = new ApprenticeKsbsPageModel()

                    {
                        Ksbs = apprenticeKsbResult.AllKsbs,
                        KnowledgeCount = apprenticeKsbResult.AllKsbs.Count(k => k.Type == Domain.Models.KsbType.Knowledge),
                        SkillCount = apprenticeKsbResult.AllKsbs.Count(k => k.Type == Domain.Models.KsbType.Skill),
                        BehaviourCount = apprenticeKsbResult.AllKsbs.Count(k => k.Type == Domain.Models.KsbType.Behaviour),
                        KsbStatuses = Enum.GetValues(typeof(Domain.Models.KSBStatus)).Cast<Domain.Models.KSBStatus>().ToList(),
                        
                    };

                    return View("_LinkKsb", apprenticeKsbsPageModel);
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

    }
}