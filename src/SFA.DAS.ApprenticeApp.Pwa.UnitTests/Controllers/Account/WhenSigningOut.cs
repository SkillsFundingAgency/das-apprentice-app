using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class WhenSigningOut
    {
        [Test, MoqAutoData]
        public async Task Then_The_User_Is_Signed_Out(
             
            [Greedy] AccountController controller)
        {
            ApplicationConfiguration _config = new() { StubAuth = "true" };
            var httpContext = new DefaultHttpContext();
             
            var claimsPrincipal = new ClaimsPrincipal(new[] { new ClaimsIdentity()});
           
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.SigningOut() as SignOutResult;
            result.Should().NotBeNull();
        }       
    }
}
