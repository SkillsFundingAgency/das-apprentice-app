using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOuterApiClient _client;
    private readonly ApplicationConfiguration _appConfig;

    public HomeController
        (
        ILogger<HomeController> logger,
        IOuterApiClient client,
        ApplicationConfiguration appConfig
        )
    {
        _logger = logger;
        _client = client;
        _appConfig = appConfig;
    }

    public async Task<IActionResult> Index()
    {
        var cookie = Request.Cookies[Constants.CookieTrackCookieName];
        if (cookie == null)
        {
            return RedirectToAction("CookieStart", "Home");
        }
        
        if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                string whiteListEmailList = _appConfig.WhiteListEmails;
                if (!string.IsNullOrEmpty(whiteListEmailList))
                {
                    WhiteListEmailUsers? users = JsonConvert.DeserializeObject<WhiteListEmailUsers>(whiteListEmailList);

                    var apprenticeEmail = Claims.GetClaim(HttpContext, Constants.ApprenticeNameClaimKey);

                    var match = users?.Emails?.Contains(apprenticeEmail);
                    if (match == true)
                    {
                        var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                        if (apprenticeDetails != null && apprenticeDetails.MyApprenticeship != null)
                        {
                            return RedirectToAction("Index", "Tasks");
                        }
                    }
                }
            }
        }
        var vm = new HomeViewModel();
        return View(vm);
    }
    
    public async Task<IActionResult> CookieStart()
    {
        // if cookie exists bypass page
        var cookie = Request.Cookies[Constants.CookieTrackCookieName];

        if (cookie == null)
        {
            var vm = new CookieStartViewModel();
            return View(vm);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost("DeclineCookies")]
    public async Task<IActionResult> DeclineCookies()
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddYears(99),
            Path = "/",
            Secure = true,
            HttpOnly = true
        };
        Response.Cookies.Append(Constants.CookieTrackCookieName, "0", cookieOptions);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("AcceptCookies")]
    public async Task<IActionResult> AcceptCookies()
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddYears(99),
            Path = "/",
            Secure = true,
            HttpOnly = true
        };
        Response.Cookies.Append(Constants.CookieTrackCookieName, "1", cookieOptions);
        
        return RedirectToAction("Index", "Home");
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