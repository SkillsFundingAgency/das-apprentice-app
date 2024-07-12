using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Welcome
{
    [TestFixture]
    public class WhenLoadingTheWelcomePage
    {
        [Test, MoqAutoData]
        public void Then_The_Welcome_Page_is_displayed(
         [Greedy] WelcomeController controller)
        {
            var result = controller.Index() as ActionResult;
            Assert.IsNotNull(result);
        }
    }
}