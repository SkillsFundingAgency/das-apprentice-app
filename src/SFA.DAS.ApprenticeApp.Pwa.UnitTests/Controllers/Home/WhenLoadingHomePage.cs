using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class WhenLoadingHomePage
    {
        [Test, MoqAutoData]
        public async Task Then_The_Homepage_Is_Loaded([Greedy] HomeController controller)
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

            var result = await controller.Index() as ActionResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Redirect_If_logged_in(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] ApprenticeDetails apprenticeDetails,
            [Greedy] HomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString(), ClaimValueTypes.String)
            }, "Custom");

            httpContext.User = new ClaimsPrincipal(identity);
            apprenticeDetails.MyApprenticeship = new MyApprenticeship();
            client.Setup(c => c.GetApprenticeDetails(apprenticeId)).ReturnsAsync(apprenticeDetails);

                var result = await controller.Index() as RedirectToActionResult;
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Tasks");
        }

        [Test, MoqAutoData]
        public async Task Redirect_If_Authenticated([Greedy] HomeController controller)
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
                var result = await controller.Index() as ActionResult;
                result.Should().NotBeNull();
            }
        }
    }
}
