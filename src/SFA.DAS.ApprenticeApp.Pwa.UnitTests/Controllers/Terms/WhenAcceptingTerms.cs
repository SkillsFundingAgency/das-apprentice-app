using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class WhenAcceptingTerms
    {

        [Test, MoqAutoData]
        public async Task Then_The_Welcome_Page_Is_Displayed_For_Valid_Apprentice(
     [Frozen] Mock<ILogger<TermsController>> logger,
     [Frozen] Mock<IApprenticeContext> apprenticeContext,
     [Frozen] Mock<ICommitmentsService> commitmentsService,
     [Frozen] Mock<IOuterApiClient> client,
     [Greedy] TermsController controller)
        {
            // Arrange
            var apprenticeId = Guid.NewGuid().ToString();
            apprenticeContext.Setup(x => x.ApprenticeId).Returns(apprenticeId);

            var apprenticeDetails = new ApprenticeDetails();
            client.Setup(x => x.UpdateApprentice(
                    It.IsAny<Guid>(),
                    It.IsAny<JsonPatchDocument<Apprentice>>()))
                .Returns(Task.CompletedTask);

            client.Setup(x => x.GetApprenticeDetails(It.IsAny<Guid>()))
                .ReturnsAsync(apprenticeDetails);

            commitmentsService
                .Setup(x => x.HandleConfirmationStatus(apprenticeDetails, Guid.Parse(apprenticeId)))
                .ReturnsAsync(new CmadNavigationResult
                {
                    NavigationType = CmadNavigationType.WelcomeIndex
                });

            // Act
            var result = await controller.TermsAccept() as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                        v.ToString().Contains("Apprentice accepted the Terms")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));

                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Welcome");

                client.Verify(x => x.UpdateApprentice(
                    Guid.Parse(apprenticeId),
                    It.IsAny<JsonPatchDocument<Apprentice>>()), Times.Once);

                client.Verify(x => x.GetApprenticeDetails(Guid.Parse(apprenticeId)), Times.Once);
                commitmentsService.Verify(x => x.HandleConfirmationStatus(apprenticeDetails, Guid.Parse(apprenticeId)), Times.Once);
            }
        }

        [Test, MoqAutoData]
        public async Task Then_The_Home_Page_Is_Displayed_For_Invalid_Apprentice(
    [Frozen] Mock<ILogger<TermsController>> logger,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] TermsController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns((string?)null); // ← invalid apprentice

            // Act
            var actual = await controller.TermsAccept() as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                        v.ToString()!.Contains(
                            "ApprenticeId not found in user claims")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
                ));

                actual!.ActionName.Should().Be("Index");
                actual.ControllerName.Should().Be("Login");
            }
        }
    }
}
