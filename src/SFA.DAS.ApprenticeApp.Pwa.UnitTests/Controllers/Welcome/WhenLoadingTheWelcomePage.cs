using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;
namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Welcome
{
    [TestFixture]
    public class WhenLoadingTheWelcomePage
    {
        [Test, MoqAutoData]
        public void User_CanUse_App(
            [Frozen] ApplicationConfiguration configuration,
            [Frozen] Mock<IRequestCookieCollection> cookies,
            [Greedy] WelcomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeNameClaim = new Claim(Constants.ApprenticeNameClaimKey, "test1@test.com");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
               apprenticeIdClaim,
               apprenticeNameClaim
            })});
            cookies.Setup(c => c[Constants.WelcomeSplashScreenCookieName]).Returns("1");
            httpContext.Request.Cookies = cookies.Object;
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = controller.Index() as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Ksb");
        }

        [Test, MoqAutoData]
        public void First_Visit_Shows_The_First_Step(
            [Frozen] Mock<IRequestCookieCollection> cookies,
            [Greedy] WelcomeController controller)
        {
            var controllerContext = BuildContext(cookies, cookieValue: null);
            controller.ControllerContext = controllerContext;

            var result = controller.Index() as ViewResult;

            var model = result.Model.Should().BeOfType<WelcomePageModel>().Subject;
            model.Number.Should().Be(1);
            model.Total.Should().Be(6);
            model.IsFirst.Should().BeTrue();
            controllerContext.HttpContext.Response.Headers.SetCookie.ToString()
                .Should().Contain(Constants.WelcomeSplashScreenCookieName);
        }

        [Test, MoqAutoData]
        public void A_Step_Is_Shown_Even_Once_The_Cookie_Is_Set(
            [Frozen] Mock<IRequestCookieCollection> cookies,
            [Greedy] WelcomeController controller)
        {
            controller.ControllerContext = BuildContext(cookies, cookieValue: "1");

            var result = controller.Index(3) as ViewResult;

            var model = result.Model.Should().BeOfType<WelcomePageModel>().Subject;
            model.Number.Should().Be(3);
            model.IsFirst.Should().BeFalse();
            model.IsLast.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public void The_Last_Step_Is_Marked_As_Such(
            [Frozen] Mock<IRequestCookieCollection> cookies,
            [Greedy] WelcomeController controller)
        {
            controller.ControllerContext = BuildContext(cookies, cookieValue: "1");

            var result = controller.Index(6) as ViewResult;

            result.Model.Should().BeOfType<WelcomePageModel>().Subject.IsLast.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public void A_Step_Outside_The_Tour_Goes_Back_To_The_Start(
            [Frozen] Mock<IRequestCookieCollection> cookies,
            [Greedy] WelcomeController controller)
        {
            controller.ControllerContext = BuildContext(cookies, cookieValue: "1");

            var result = controller.Index(99) as RedirectToActionResult;

            result.ActionName.Should().Be("Index");
            result.RouteValues["step"].Should().Be(1);
        }

        private static ControllerContext BuildContext(Mock<IRequestCookieCollection> cookies, string? cookieValue)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, Guid.NewGuid().ToString());
            httpContext.User = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { apprenticeIdClaim }) });

            cookies.Setup(c => c[Constants.WelcomeSplashScreenCookieName]).Returns(cookieValue);
            httpContext.Request.Cookies = cookies.Object;

            return new ControllerContext { HttpContext = httpContext };
        }
    }
}
