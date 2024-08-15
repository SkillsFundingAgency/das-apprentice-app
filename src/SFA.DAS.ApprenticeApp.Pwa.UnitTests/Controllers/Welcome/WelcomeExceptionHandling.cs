using NUnit.Framework;
using System;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers.Tests
{
    public class WelcomeControllerExceptionTests
    {
        public void Index_ThrowsException_ReturnsErrorView()
        {
            var controller = new WelcomeController();
            Assert.Throws<Exception>(() => controller.Index());
        }
    }
}