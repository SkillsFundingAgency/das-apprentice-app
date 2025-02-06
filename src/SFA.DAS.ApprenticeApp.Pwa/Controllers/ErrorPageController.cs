using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers;

public class ErrorPageController : Controller
{
    [Route("ErrorPage/400")]
    public async Task<IActionResult> BadRequest()
    {
        return View("~/Views/ErrorPage/400.cshtml");
    }
    
    [Route("ErrorPage/403")]
    public async Task<IActionResult> Forbidden()
    {
        return View("~/Views/ErrorPage/403.cshtml");
    }
    
    [Route("ErrorPage/404")]
    public async Task<IActionResult> PageNotFound()
    {
        return View("~/Views/ErrorPage/404.cshtml");
    }

    [Route("ErrorPage/500")]
    public async Task<IActionResult> Error()
    {
        return View("~/Views/ErrorPage/500.cshtml");
    }
    
    [Route("ErrorPage/503")]
    public async Task<IActionResult> ServiceUnavailable()
    {
        return View("~/Views/ErrorPage/503.cshtml");
    }
}