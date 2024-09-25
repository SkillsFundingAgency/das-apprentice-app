using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class AccountControllerTests
    {
        [Test, MoqAutoData]
        public void Loading_Authenticated_Page([Greedy] AccountController controller)
        {
            var result = controller.Authenticated() as ActionResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void Loading_Login_Page([Greedy] AccountController controller)
        {
            var result = controller.Login() as RedirectToActionResult;
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Home");
        }

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
            var result = controller.StubSignedIn() as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Terms");
        }
    }
}
