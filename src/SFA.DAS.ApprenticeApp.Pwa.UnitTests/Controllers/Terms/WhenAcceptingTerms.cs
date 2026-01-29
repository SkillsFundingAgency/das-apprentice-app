using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
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

            var result = await controller.TermsAccept() as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                            v.ToString().Contains($"Apprentice accepted the Terms")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Welcome");
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
