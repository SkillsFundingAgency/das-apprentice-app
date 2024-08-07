﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class TermsController : Controller
{
    private readonly ILogger<TermsController> _logger;
    private readonly IOuterApiClient _client;

    public TermsController
        (
        ILogger<TermsController> logger,
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
            var apprentice = await _client.GetApprentice(new Guid(apprenticeId));

            if (apprentice?.TermsOfUseAccepted == true)
            {
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
        var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

        if (!string.IsNullOrEmpty(apprenticeId))
        {
            var patch = new JsonPatchDocument<Apprentice>()
                               .Replace(x => x.TermsOfUseAccepted, true);

            await _client.UpdateApprentice(new Guid(apprenticeId), patch);

            _logger.LogInformation($"Apprentice accepted the Terms. ApprenticeId: {apprenticeId}");
            return RedirectToAction("Index", "Welcome");
        }

        _logger.LogWarning($"ApprenticeId not found in user claims for Terms TermsAccept.");
        return RedirectToAction("Index", "Login");
    }

    [Authorize]
    public IActionResult TermsDecline()
    {
        var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;
        _logger.LogInformation($"Apprentice declined the Terms. ApprenticeId: {apprenticeId}");
        return RedirectToAction("SigningOut", "Account");
    }
}