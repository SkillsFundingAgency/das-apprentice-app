using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class AccountControllerTests
    {
        [Test, MoqAutoData]
        public async Task Loading_Authenticated_Page(
            [Frozen] Mock<ILogger<AccountController>> logger,
            Mock<IRequestCookieCollection> cookies,
            [Greedy] AccountController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var lastnameClaim = new Claim(Constants.ApprenticeLastNameClaimKey, "test");

            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
               apprenticeIdClaim, lastnameClaim
            })});

            cookies.Setup(c => c[Constants.ApprenticeshipIdClaimKey]).Returns("1");
            cookies.Setup(c => c[Constants.StandardUIdClaimKey]).Returns("1");
            httpContext.Request.Cookies = cookies.Object;
            httpContext.User = claimsPrincipal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Authenticated();

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Apprentice authenticated and cookies added for")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
            }
        }

        [Test, MoqAutoData]
        public async Task Loading_Authenticated_Page_LoadsError_ForNoApprentice(
    [Frozen] Mock<IOuterApiClient> client,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] AccountController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns((string?)null);   // ← no apprentice

            var httpContext = new DefaultHttpContext();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.Authenticated() as RedirectToActionResult;

            // Assert
            result!.ActionName.Should().Be("EmailMismatchError");
            result.ControllerName.Should().Be("Account");
        }

        [Test, MoqAutoData]
        public async Task Loading_Authenticated_Page_LoadsError_ForNoApprenticeship(
          [Frozen] Mock<IOuterApiClient> client,
          [Greedy] AccountController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var lastnameClaim = new Claim(Constants.ApprenticeLastNameClaimKey, "test");

            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
               apprenticeIdClaim, lastnameClaim
            })});
            httpContext.User = claimsPrincipal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(client => client.GetApprenticeDetails(It.IsAny<Guid>())).ReturnsAsync(new ApprenticeDetails() { MyApprenticeship = null });
            var result = await controller.Authenticated() as RedirectToActionResult;
            result.ActionName.Should().Be("CmadError");
            result.ControllerName.Should().Be("Cmad");
        }
        //  


        [Test, MoqAutoData]
        public void Loading_YourAccount_Page([Greedy] AccountController controller)
        {
            var result = controller.YourAccount() as ActionResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void Loading_Error_Page([Greedy] AccountController controller)
        {
            var result = controller.Error() as ActionResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Post_AccountDetails_StubFails_InProd(
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] StubAuthenticationViewModel model,
            [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("PRD");
            var result = await controller.AccountDetails(model);
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test, MoqAutoData]
        public async Task Post_AccountDetails_HandlesError(
            [Frozen] Mock<IConfiguration> configuration,

            [Frozen] Mock<IStubAuthenticationService> authenticationService,
            [Frozen] StubAuthenticationViewModel model,
            [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");
            authenticationService.Setup(x => x.GetStubSignInClaims(model)).Throws(new Exception());
            var result = await controller.AccountDetails(model) as RedirectToActionResult;

            result.ActionName.Should().Be("Error");
            result.ControllerName.Should().Be("Account");
        }


        [Test, MoqAutoData]
        public void Get_AccountDetails_StubFails_InProd(
           [Frozen] Mock<IConfiguration> configuration,
           [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("PRD");
            var result = controller.AccountDetails("returnUrl");
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test, MoqAutoData]
        public void Getting_AccountDetails_Claims_ShouldNotBeNull(
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] StubAuthenticationViewModel model,
            [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");

            var result = controller.AccountDetails("returnUrl") as ViewResult;
            result.Should().BeOfType(typeof(ViewResult));
            result.ViewName.Should().Be("AccountDetails");
        }

        [Test, MoqAutoData]
        public void StubSignedIn_Redirects_To_Terms(
             [Frozen] Mock<IConfiguration> configuration,
             [Greedy] AccountController controller)
        {
            var httpContext = new DefaultHttpContext();
            var emailClaim = new Claim(ClaimTypes.Email, "test@test.com");
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, "test");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
     {
         emailClaim,
         nameClaim
     })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");
            var result = controller.StubSignedIn() as RedirectToActionResult;
            result.ActionName.Should().Be("ConfirmDetails");
            result.ControllerName.Should().Be("Cmad");
        }

        [Test, MoqAutoData]
        public void StubSignedIn_Fails_In_Prod(
             [Frozen] Mock<IConfiguration> configuration,
             [Greedy] AccountController controller)
        {
            var httpContext = new DefaultHttpContext();
            var emailClaim = new Claim(ClaimTypes.Email, "test@test.com");
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, "test");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
     {
         emailClaim,
         nameClaim
     })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("PRD");
            var result = controller.StubSignedIn();
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}
