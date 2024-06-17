﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
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
        var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

        if (!string.IsNullOrEmpty(apprenticeId))
        {
            var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));

            if (apprenticeDetails.Apprentice != null)
            {
                if (apprenticeDetails.Apprentice?.TermsOfUseAccepted != false)
                {
                    return View(new ProfileViewModel() { Apprentice = apprenticeDetails.Apprentice, MyApprenticeship = apprenticeDetails.MyApprenticeship });
                }
                else
                {
                    _logger.LogInformation($"Apprentice redirected to Terms page as Terms not yet accepted. Apprentice Id: {apprenticeId}");
                    return RedirectToAction("Index", "Terms");
                }
            }

            _logger.LogWarning($"Apprentice Details not found - 'apprenticeDetails' is null in Profile Index. ApprenticeId: {apprenticeId}");
        }
        else
        {
            _logger.LogWarning($"ApprenticeId not found in user claims for Profile Index.");
        }
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddSubscription([FromBody] ApprenticeAddSubscriptionRequest request)
    {
        var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

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
    public async Task<IActionResult> RemoveSubscription([FromBody] string endPoint)
    {
        var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

        if (!string.IsNullOrEmpty(apprenticeId) && !string.IsNullOrEmpty(endPoint))
        {
            var removeSubscriptionRequest = new ApprenticeRemoveSubscriptionRequest
            {
                Endpoint = endPoint
            };

            string message = $"Removing subscription for apprentice. ApprenticeId: {apprenticeId}";
            _logger.LogInformation(message);
            await _client.ApprenticeRemoveSubscription(new Guid(apprenticeId), removeSubscriptionRequest);
        }
        else
        {
            if (string.IsNullOrEmpty(apprenticeId))
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Profile Index.");
            }
            if (string.IsNullOrEmpty(endPoint))
            {
                string message = $"Endpoint not found in remove subscription request for apprentice. ApprenticeId: {apprenticeId}";
                _logger.LogWarning(message);
            }
        }
        return RedirectToAction("Index", "Profile");
    }
}