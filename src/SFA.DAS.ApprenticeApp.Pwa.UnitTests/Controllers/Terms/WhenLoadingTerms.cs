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
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Terms
{
    public class WhenLoadingTerms
    {
        [Test, MoqAutoData]
        public async Task Then_The_Profile_Page_is_displayed_if_Already_Accepted(
          [Frozen] Mock<IOuterApiClient> _client,
          [Greedy] TermsController controller)
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

            var result = await controller.Index() as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Profile");
        }

        [Test, MoqAutoData]
        public async Task Then_an_errorpage_is_displayed_for_invalid_apprentice(
          [Frozen] Mock<IOuterApiClient> _client,
          [Greedy] TermsController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, "");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.Index() as RedirectToActionResult;
            result.ActionName.Should().Be("Error");
            result.ControllerName.Should().Be("Account");
        }

        [Test, MoqAutoData]
        public async Task Then_The_Terms_Page_Is_Displayed_For_Valid_Apprentice_Not_Yet_Accepted(
            [Frozen] Mock<IOuterApiClient> _client,
       [Greedy] TermsController controller)
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


            Apprentice apprentice = new()
            {
                ApprenticeId = Guid.NewGuid()
            };
            _client.Setup(x => x.GetApprentice(apprenticeId));

            var actual = await controller.Index() as ViewResult;
            actual.Should().NotBeNull();
        }
    }
}
