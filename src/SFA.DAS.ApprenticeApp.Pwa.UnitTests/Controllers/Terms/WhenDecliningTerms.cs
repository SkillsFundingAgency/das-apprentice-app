using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Terms
{
    public class WhenDecliningTerms
    {
        [Test, MoqAutoData]
        public async Task Then_User_Is_Signed_Out(
            [Greedy] TermsController controller)
        {
            var actual = controller.TermsDecline() as RedirectToActionResult;
            actual.ActionName.Should().Be("SigningOut");
            actual.ControllerName.Should().Be("Account");
        }


    }
}
