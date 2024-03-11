using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOuterApiClient _client;

    public HomeController
        (
        ILogger<HomeController> logger,
        IOuterApiClient client
        )
    {
        _logger = logger;
        _client = client;
    }

    public IActionResult Index()
    {
        var vm = new HomeViewModel();
        return View(vm);
    }

    // example
    public async Task<IActionResult> Profile()
    {
        var apprenticeDetails = await _client.GetApprenticeDetails(new Guid("fd0daf58-af19-440d-b52f-7e1d47267d3b"));
        return View(new ProfileViewModel() { Apprentice = apprenticeDetails.Apprentice, MyApprenticeship = apprenticeDetails.MyApprenticeship  });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Unauthorised()
    {
        return View(new ErrorViewModel {  RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

