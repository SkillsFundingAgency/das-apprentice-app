using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;
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
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Frozen] Mock<ICommitmentsService> commitmentsService,
    [Greedy] TermsController controller)
        {
            // Arrange
            var apprenticeId = Guid.NewGuid().ToString();
            apprenticeContext.Setup(x => x.ApprenticeId).Returns(apprenticeId);

            var httpContext = new DefaultHttpContext();
            var claimsPrincipal = new ClaimsPrincipal(new[]
            {
        new ClaimsIdentity(new[]
        {
            new Claim(Constants.TermsAcceptedClaimKey, "True")
        })
    });

            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var apprenticeDetails = new ApprenticeDetails();

            _client.Setup(x => x.UpdateApprentice(
                    Guid.Parse(apprenticeId),
                    It.IsAny<JsonPatchDocument<Apprentice>>()))
                .Returns(Task.CompletedTask);

            _client.Setup(x => x.GetApprenticeDetails(Guid.Parse(apprenticeId)))
                .ReturnsAsync(apprenticeDetails);

            commitmentsService
                .Setup(x => x.HandleConfirmationStatus(apprenticeDetails, Guid.Parse(apprenticeId)))
                .ReturnsAsync(new CmadNavigationResult
                {
                    NavigationType = CmadNavigationType.WelcomeIndex
                });

            // Act
            var result = await controller.Index() as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Welcome");

            _client.Verify(x => x.UpdateApprentice(
                Guid.Parse(apprenticeId),
                It.IsAny<JsonPatchDocument<Apprentice>>()), Times.Once);

            _client.Verify(x => x.GetApprenticeDetails(Guid.Parse(apprenticeId)), Times.Once);

            commitmentsService.Verify(x => x.HandleConfirmationStatus(
                apprenticeDetails,
                Guid.Parse(apprenticeId)), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Terms_Page_Is_Displayed_For_Valid_Apprentice_Not_Yet_Accepted(
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] TermsController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var termsAcceptedClaim = new Claim(Constants.TermsAcceptedClaimKey, "False");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            apprenticeIdClaim,
            termsAcceptedClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var actual = await controller.Index() as ViewResult;
            actual.Should().NotBeNull();
        }
    }
}
