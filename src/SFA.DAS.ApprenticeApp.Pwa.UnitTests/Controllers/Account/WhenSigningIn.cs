using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class WhenSigningIn
    {
        [Test, MoqAutoData]
        public void Then_The_Signin_Page_Is_Displayed([Greedy] AccountController controller)
        {
            var result = controller.SignIn() as ActionResult;
            result.Should().NotBeNull();
        }
    }
}
