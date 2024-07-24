using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class WhenLoadingHomePage
    {
        [Test, MoqAutoData]
        public void Then_The_Homepage_Is_Loaded([Greedy] HomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Index() as ActionResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void Redirect_If_logged_in([Greedy] HomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("UserId", "123", ClaimValueTypes.Integer32)
            }, "Custom");

            httpContext.User = new ClaimsPrincipal(identity);

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var result = controller.Index() as ActionResult;
                result.Should().NotBeNull();
            }
        }
    }
}
