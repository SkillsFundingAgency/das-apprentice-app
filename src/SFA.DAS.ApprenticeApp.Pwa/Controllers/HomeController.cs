using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOuterApiClient _client;
    private readonly IConfiguration _config;

    public HomeController
        (
        ILogger<HomeController> logger,
        IOuterApiClient client,
        IConfiguration configuration
        )
    {
        _logger = logger;
        _client = client;
        _config = configuration;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                string whiteListEmailList = _config["WhiteListEmails"];
                if (whiteListEmailList != null)
                {
                    WhiteListEmailUsers? users = JsonConvert.DeserializeObject<WhiteListEmailUsers>(whiteListEmailList);

                    var apprenticeEmail = Claims.GetClaim(HttpContext, Constants.ApprenticeNameClaimKey);

                    var match = users?.Emails?.Contains(apprenticeEmail);
                    if (match == true)
                    {
                    var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                    if (apprenticeDetails != null && apprenticeDetails.MyApprenticeship != null)
                        {
                            return RedirectToAction("Index", "Profile");
                        }
                    }
                }
            }
        }
        var vm = new HomeViewModel();
        return View(vm);
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

