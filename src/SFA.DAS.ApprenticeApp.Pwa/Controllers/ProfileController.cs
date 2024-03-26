using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
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

    //[Authorize]
    public async Task<IActionResult> Index()
    {
        var vm = new ProfileViewModel();

        var apprenticeDetails = await _client.GetApprenticeDetails(new Guid("fd0daf58-af19-440d-b52f-7e1d47267d3b"));

        if (apprenticeDetails != null) {
            vm = new ProfileViewModel() { Apprentice = apprenticeDetails.Apprentice, MyApprenticeship = apprenticeDetails.MyApprenticeship };
        }

        return View(vm);
    }
}