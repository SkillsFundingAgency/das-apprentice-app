using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class WhenSigningIn
    {
       [Test, MoqAutoData]
       public void Then_The_Stub_Auth_Details_Are_Returned_When_Local(
       [Frozen] Mock<ILogger<HomeController>> logger,
       [Frozen] Mock<ApplicationConfiguration> configuration,
       [Greedy] AccountController controller)
        {
            configuration.StubAuth = "true";
            var actual = controller.StubSignedIn() as NotFoundResult;
            actual.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void Then_The_Stub_Auth_Details_Are_Returned_When_Not_Prod(
       string emailClaimValue,
       string nameClaimValue,
       //string returnUrl,
       StubAuthUserDetails model,
       [Frozen] Mock<ApplicationConfiguration> configuration,
       [Frozen] Mock<IStubAuthenticationService> stubAuthService,
       [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
            var httpContext = new DefaultHttpContext();
            var emailClaim = new Claim(ClaimTypes.Email, emailClaimValue);
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, nameClaimValue);
            var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            emailClaim,
            nameClaim
        })});
            httpContext.User = claimsPrinciple;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var actual = controller.StubSignedIn() as ViewResult;
            var actualModel = actual?.Model as ApprenticeStubViewModel;

            using (new AssertionScope())
            {
                actual.Should().NotBeNull();
                actualModel.Should().NotBeNull();
                actualModel.Email.Should().BeEquivalentTo(emailClaimValue);
                actualModel.Id.Should().BeEquivalentTo(nameClaimValue);
                //actualModel.ReturnUrl.Should().BeEquivalentTo(returnUrl);
            }
        }
    }
}
