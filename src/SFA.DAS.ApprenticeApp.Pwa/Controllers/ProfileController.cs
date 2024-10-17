using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class ProfileController : Controller
{
    private readonly ILogger<ProfileController> _logger;
    private readonly IOuterApiClient _client;

    public ProfileController
        (
        ILogger<ProfileController> logger,
        IOuterApiClient client
        )
    {
        _logger = logger;
        _client = client;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

        if (!string.IsNullOrEmpty(apprenticeId))
        {
           var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
           return View(new ProfileViewModel() { Apprentice = apprenticeDetails.Apprentice, MyApprenticeship = apprenticeDetails.MyApprenticeship });
           
        }
        else
        {
            _logger.LogWarning("ApprenticeId not found in user claims for Profile Index.");
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddSubscription([FromBody] ApprenticeAddSubscriptionRequest request)
    {
        if(!ModelState.IsValid)
        {
            _logger.LogWarning("ProfileController: ModelState is not valid in AddSubscription.");
            return RedirectToAction("Index", "Profile");
        }
        var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

        if (!string.IsNullOrEmpty(apprenticeId) && !string.IsNullOrEmpty(request.Endpoint))
        {
            string message = $"Sending subscription details for {apprenticeId}";
            _logger.LogInformation(message);
            await _client.ApprenticeAddSubscription(new Guid(apprenticeId), request);
        }
        else
        {
            if (string.IsNullOrEmpty(apprenticeId))
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Profile Index.");
            }
            if (string.IsNullOrEmpty(request.Endpoint))
            {
                _logger.LogWarning("Endpoint not found in subscription request.");
            }
        }
        return RedirectToAction("Index", "Profile");
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> RemoveSubscription([FromBody] ApprenticeRemoveSubscriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ProfileController: ModelState is not valid in RemoveSubscription.");
            return RedirectToAction("Index", "Profile");
        }

        var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

        if (!string.IsNullOrEmpty(apprenticeId) && !string.IsNullOrEmpty(request.Endpoint))
        {
            string message = $"Removing subscription for apprentice. ApprenticeId: {apprenticeId}";
            _logger.LogInformation(message);
            await _client.ApprenticeRemoveSubscription(new Guid(apprenticeId), request);
        }
        else
        {
            if (string.IsNullOrEmpty(apprenticeId))
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Profile Index.");
            }
            if (string.IsNullOrEmpty(request.Endpoint))
            {
                string message = $"Endpoint not found in remove subscription request for apprentice. ApprenticeId: {apprenticeId}";
                _logger.LogWarning(message);
            }
        }
        return RedirectToAction("Index", "Profile");
    }
}