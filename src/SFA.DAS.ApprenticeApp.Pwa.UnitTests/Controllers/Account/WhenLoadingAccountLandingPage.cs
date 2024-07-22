using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class WhenLoadingAccountLandingPage
    {
        [Test, MoqAutoData]
        public void Loading_Page([Greedy] AccountController controller)
        {
            var result = controller.AccountLandingPage() as ActionResult;
            result.Should().NotBeNull();
        }
    }
}
