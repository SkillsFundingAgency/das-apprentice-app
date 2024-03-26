using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<IActionResult> Index()
    {
        var apprenticeDetails = await _client.GetApprenticeDetails(new Guid("fd0daf58-af19-440d-b52f-7e1d47267d3b"));

        if (apprenticeDetails?.Apprentice?.TermsOfUseAccepted == true)
        {
            return RedirectToAction("Index", "Profile");
        }

        return View();
    }

    public async Task<IActionResult> TermsAccept()
    {
        var patch = new JsonPatchDocument<Apprentice>()
                           .Replace(x => x.TermsOfUseAccepted, true);

        await _client.UpdateApprentice(new Guid("fd0daf58-af19-440d-b52f-7e1d47267d3b"), patch);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult TermsDecline()
    {
        return RedirectToAction("SigningOut", "Account");
    }
}

