using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class WhenStubSigningIn
    {
        [Test, MoqAutoData]
        public void Then_The_View_Is_Displayed(
            string emailClaimValue,
            string nameClaimValue,
            StubAuthUserDetails stubAuthUserDetails, 
            [Greedy] AccountController controller
            )
        {
            var httpContext = new DefaultHttpContext();
            var emailClaim = new Claim(ClaimTypes.Email, emailClaimValue);
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, nameClaimValue);
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
            var result = controller.StubSignedIn() as ViewResult;
            result.ViewData.Should().NotBeNull();
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void And_Terms_Accepted_Then_Profile_Displayed([Greedy] AccountController controller)
        {
            var result = controller.Authenticated() as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Tasks");

        }

        [Test, MoqAutoData]
        public async Task Redirect_To_Terms_After_Entering_Credentials(
            string emailClaimValue,
            string nameClaimValue,
            StubAuthUserDetails model,
            Mock<IStubAuthenticationService> stubAuthenticationService,
            [Frozen] ClaimsPrincipal claims,            
            [Greedy] AccountController controller)
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);
            var httpContext = new DefaultHttpContext() { RequestServices = serviceProviderMock.Object};
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var emailClaim = new Claim(ClaimTypes.Email, emailClaimValue);
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, nameClaimValue);
            var apprenticeshipIdClaim = new Claim("apprenticeshipId", "1");
            var standardUIdClaim = new Claim("standardUId", "1");
                        
            stubAuthenticationService.Setup(s => s.GetStubSignInClaims(model)).ReturnsAsync(claims);
            claims.AddIdentity(new ClaimsIdentity(new[]
            {
                emailClaim,
                nameClaim,
                apprenticeshipIdClaim,
                standardUIdClaim,
                apprenticeIdClaim
            }));

            httpContext.User = claims;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);
            model.Id = null; 
            var result = await controller.SignIn(model) as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Terms");

        }
        
        //[Test, MoqAutoData]
        //public async Task Stay_On_Account_Details_If_UserId_Is_Missing (
        //   StubAuthUserDetails model,
        //   [Greedy] AccountController controller)
        //{
        //    model.Id = null;
        //    var result = await controller.SignIn(model) as ViewResult;
        //    result.Should().NotBeNull();
        //}

        [Test, MoqAutoData]
        public void GetErrorView([Greedy] AccountController controller)
        {
            var result = controller.Error() as ViewResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void GetloginView([Greedy] AccountController controller)
        {
            var result = controller.Login() as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Home");
        }
    }
}
