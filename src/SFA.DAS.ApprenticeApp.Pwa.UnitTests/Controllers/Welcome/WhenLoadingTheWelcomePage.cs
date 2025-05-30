﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
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
            result.ControllerName.Should().Be("Tasks");
        }
    }
}
