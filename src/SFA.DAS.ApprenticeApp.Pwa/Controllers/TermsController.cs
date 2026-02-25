using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Services;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class TermsController : Controller
{
    private readonly ILogger<TermsController> _logger;
    private readonly IOuterApiClient _client;
    private readonly ICommitmentsService _commitmentsService;
    private readonly IApprenticeContext _apprenticeContext;

    public TermsController
        (
        ILogger<TermsController> logger,
        IOuterApiClient client,
        ICommitmentsService commitmentsService,
        IApprenticeContext apprenticeContext
        )
    {
        _logger = logger;
        _client = client;
        _commitmentsService = commitmentsService;
        _apprenticeContext = apprenticeContext;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {        
        var apprenticeId = _apprenticeContext.ApprenticeId;

        if (!Guid.TryParse(apprenticeId, out var apprenticeGuid))
        {
            _logger.LogWarning("ApprenticeId claim is missing or invalid in Terms Index.");
            return RedirectToAction("Error", "Account");
        }

        // update apprentice log in time
        await _client.UpdateApprentice(new Guid(apprenticeId), new JsonPatchDocument<Apprentice>().Replace(x => x.AppLastLoggedIn, DateTime.Now));

        if (!string.IsNullOrEmpty(apprenticeId))
        {
            var termsAccepted = Claims.GetClaim(HttpContext, Constants.TermsAcceptedClaimKey);

            if (termsAccepted != null && termsAccepted == "True")
            {
                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

                var registrationByEmail = await _client.GetRegistrationByEmail(apprenticeDetails.Apprentice.Email);

                if (registrationByEmail.Count == 1)
                {
                    var registration = registrationByEmail.FirstOrDefault();
                    var commitment = await _client.GetCommitmentsApprenticeshipById(registration.CommitmentsApprenticeshipId);

                    var viewModel = await _commitmentsService.CreateApprenticeshipAndBuildViewModelAsync(
                        registration.RegistrationId,
                        Guid.Parse(apprenticeId),
                        commitment.Uln,
                        registration.LastName,
                        registration.DateOfBirth.ToIsoDate());

                    TempData["ConfirmModel"] = JsonConvert.SerializeObject(viewModel);
                    return RedirectToAction("ConfirmApprenticeshipDetails", "Cmad");
                }

                var cmadComplete = apprenticeDetails.Apprenticeship?.Apprenticeships?.FirstOrDefault();

                if (cmadComplete == null) return RedirectToAction("ConfirmDetails", "Cmad", new { apprenticeId });

                return RedirectToAction("Index", "Welcome");
            }
            else
            {
                return View();
            }
        }

        return RedirectToAction("Error", "Account");
    }

    [Authorize]
    public async Task<IActionResult> TermsAccept()
    {
        var apprenticeId = _apprenticeContext.ApprenticeId;

        if (!string.IsNullOrEmpty(apprenticeId))
        {
            var patch = new JsonPatchDocument<Apprentice>()
                               .Replace(x => x.TermsOfUseAccepted, true);

            await _client.UpdateApprentice(new Guid(apprenticeId), patch);
            _logger.LogInformation($"Apprentice accepted the Terms. ApprenticeId: {apprenticeId}");

            var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
            var cmadComplete = apprenticeDetails.Apprenticeship?.Apprenticeships?.FirstOrDefault();

            if (cmadComplete == null) return RedirectToAction("ConfirmDetails", "Cmad", new { apprenticeId });
            
            return RedirectToAction("Index", "Welcome");
        }

        _logger.LogWarning($"ApprenticeId not found in user claims for Terms TermsAccept.");
        return RedirectToAction("Index", "Login");
    }

    [Authorize]
    public IActionResult TermsDecline()
    {
        var apprenticeId = _apprenticeContext.ApprenticeId;
        _logger.LogInformation($"Apprentice declined the Terms. ApprenticeId: {apprenticeId}");
        return RedirectToAction("SigningOut", "Account");
    }
}