using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class ProfileController : Controller
{
    private const string Message = $"Adding subscription for apprentice.";
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
                    string message = $"Apprentice redirected to Terms page as Terms not yet accepted. Apprentice Id: {apprenticeId}";
                    _logger.LogInformation(message: message);
                    return RedirectToAction("Index", "Terms");
                }
            }

            string message1 = $"Apprentice Details not found - 'apprenticeDetails' is null in Profile Index. ApprenticeId: {apprenticeId}";
            _logger.LogWarning(message: message1);
        }
        else
        {
            const string Message1 = $"ApprenticeId not found in user claims for Profile Index.";
            _logger.LogWarning(message: Message1);
        }
        return RedirectToAction("Index", "Home");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddSubscription()
    {
        var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

        if (!string.IsNullOrEmpty(apprenticeId))
        {
            var addSubscriptionRequest = new ApprenticeAddSubscriptionRequest
            {
                Endpoint = "HTTP.Endpoint",
                AuthenticationSecret = "ABC",
                PublicKey = "ABC"
            };
            _logger.LogInformation(message: Message);
            await _client.ApprenticeAddSubscription(new Guid(apprenticeId), addSubscriptionRequest);
        }
        else
        {
            const string Message1 = $"ApprenticeId not found in user claims for Profile Index.";
            _logger.LogWarning(message: Message1);
        }
        return RedirectToAction("Index", "Profile");
    }

    [Authorize]
    public async Task<IActionResult> RemoveSubscription()
    {
        var apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

        if (!string.IsNullOrEmpty(apprenticeId))
        {
            var removeSubscriptionRequest = new ApprenticeRemoveSubscriptionRequest
            {
                Endpoint = "HTTP.Endpoint"
            };
            const string Message1 = $"Removing subscription for apprentice.";
            _logger.LogInformation(message: Message1);
            await _client.ApprenticeRemoveSubscription(new Guid(apprenticeId), removeSubscriptionRequest);
        }
        else
        {
            const string Message1 = $"ApprenticeId not found in user claims for Profile Index.";
            _logger.LogWarning(message: Message1);
        }
        return RedirectToAction("Index", "Profile");
        }
    }
