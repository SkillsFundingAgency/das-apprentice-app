using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
        public void Then_The_Welcome_Page_is_displayed_OnFirstUse(
         [Greedy] WelcomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Index() as ActionResult;
            Assert.IsNotNull(result);
        }

        [Test, MoqAutoData]
        public void Then_The_User_IsRedirected_If_Cookie_Exists(Mock<IRequestCookieCollection> cookies,
         [Greedy] WelcomeController controller)
        {
            var httpContext = new DefaultHttpContext();

            cookies.Setup(c => c[Constants.WelcomeSplashScreenCookieName]).Returns("1");
            httpContext.Request.Cookies = cookies.Object;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Index() as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Tasks");

        }
    }
}