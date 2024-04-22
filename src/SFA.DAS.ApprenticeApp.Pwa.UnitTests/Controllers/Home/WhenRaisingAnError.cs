using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class WhenRaisingAnError
    {
        [Test, MoqAutoData]
        public async Task Then_The_Error_View_Is_Displayed([Greedy] HomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = Guid.NewGuid().ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = controller.Error() as ActionResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_The_Error_View_Is_Displayed_When_Unauthorised(
            [Greedy] HomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = Guid.NewGuid().ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var errorViewModel = new ErrorViewModel()
            {
                RequestId = Guid.NewGuid().ToString()
            };
            var result = controller.Unauthorised() as ActionResult;
            result.Should().NotBeNull();
        }
    }
}
