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
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Terms
{
    public class WhenDecliningTerms
    {
        [Test, MoqAutoData]
        public async Task Then_User_Is_Signed_Out(
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
            var actual = controller.TermsDecline() as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                            v.ToString().Contains($"Apprentice declined the Terms")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                actual.ActionName.Should().Be("SigningOut");
                actual.ControllerName.Should().Be("Account");
            }

        }


    }
}
