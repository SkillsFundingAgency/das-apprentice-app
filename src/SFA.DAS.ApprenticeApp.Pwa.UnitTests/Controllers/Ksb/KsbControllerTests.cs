using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class KsbControllerTests
    {
        [Test, MoqAutoData]
        public async Task LoadIndex([Greedy] KsbController controller)
        {
            var result = await controller.Index();
            result.Should().BeOfType(typeof(ViewResult));
        }

    }
}