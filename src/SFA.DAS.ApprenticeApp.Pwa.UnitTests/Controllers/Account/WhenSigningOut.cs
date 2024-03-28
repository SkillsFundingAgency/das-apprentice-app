using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class WhenSigningOut
    {
        [Test, MoqAutoData]
        public void Then_The_Stub_User_Is_Signed_Out(
             [Frozen] Mock<IConfiguration> configuration, [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["StubAuth"]).Returns("true");
            controller.ControllerContext = new ControllerContext();
            var serviceProvider = new Mock<IServiceProvider>();
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var authResult = AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), null));

            authResult.Properties.StoreTokens(new[]{new AuthenticationToken { Name = "id_token", Value = "idTokenValue" }});

            authenticationServiceMock
                .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), null))
                .ReturnsAsync(authResult);

            serviceProvider.Setup(_ => _.GetService(typeof(IAuthenticationService))).Returns(authenticationServiceMock.Object);
            var claimsPrincipal = new ClaimsPrincipal(new[] { new ClaimsIdentity() });
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal,
                RequestServices = serviceProvider.Object
            };

            var actual = controller.SigningOut();
            actual.Should().NotBeNull();
        }
    }
}
