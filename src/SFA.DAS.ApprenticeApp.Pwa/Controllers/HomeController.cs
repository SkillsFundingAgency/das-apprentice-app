using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class HomeController : Controller
{
    private readonly IOuterApiClient _client;
    private readonly IApprenticeContext _apprenticeContext;

    public HomeController(IOuterApiClient client, IApprenticeContext apprenticeContext)
        {
            _client = client;
            _apprenticeContext = apprenticeContext;
    }

    public async Task<IActionResult> Index()
    {
        //var cookie = Request.Cookies[Constants.CookieTrackCookieName];
        //if (cookie == null)
        //{
        //    return RedirectToAction("CookieStart", "Home");
        //}
        
        if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {

                var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeId));
                if (apprenticeDetails != null && apprenticeDetails.MyApprenticeship != null)
                {
                    return RedirectToAction("Index", "Tasks");
                }
            }
        }
        var vm = new HomeViewModel();
        return View(vm);
    }

    public async Task<IActionResult> AccessibilityStatement()
    {
        return View();
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
            HttpOnly = false
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
            HttpOnly = false
        };
        Response.Cookies.Append(Constants.CookieTrackCookieName, "1", cookieOptions);
        
        return RedirectToAction("Index", "Home");
    }
        
    /// <summary>
    /// Necessary for the iOS holding screen for ATT
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IActionResult> Empty()
    {
        return new EmptyResult();
    }
    
    [HttpGet]
    public async Task<IActionResult> KeepAlive()
    {
        return NoContent();
    }    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Unauthorised()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}