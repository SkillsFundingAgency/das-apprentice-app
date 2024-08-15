using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Welcome
{
    [TestFixture]
    public class WhenLoadingTheWelcomePage
    {
        [Test, MoqAutoData]
        public void Then_The_Welcome_Page_is_displayed(
         [Greedy] WelcomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Cookies.Append(Constants.WelcomeSplashScreenCookieName, "1");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Index() as ActionResult;
            Assert.IsNotNull(result);
        }
    }
}