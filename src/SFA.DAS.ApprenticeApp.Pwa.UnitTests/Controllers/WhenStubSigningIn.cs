using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class WhenSigningIn
    {
        
        [Test]
        public void Then_The_Stub_Auth_Details_Are_Returned_When_Local(
       
       StubAuthUserDetails model,
       [Frozen] Mock<IConfiguration> configuration,
       [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["StubAuth"]).Returns("true");

            var actual = controller.StubSignedIn() as NotFoundResult;
           
            actual.Should().NotBeNull();
        }
    }
}
