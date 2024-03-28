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

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class WhenLoadingHomePage
    {
        [Test, MoqAutoData]
        public void Then_The_Homepage_Is_Loaded([Greedy] HomeController controller)
        {
            var result = controller.Index() as ActionResult;
            result.Should().NotBeNull();
        }


    }
}
