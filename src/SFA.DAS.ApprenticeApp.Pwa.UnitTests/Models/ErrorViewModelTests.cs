using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Models
{
    public class ErrorViewModelTests
    {
        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("fakeId", true)]
        public void IsStoppedReturnsExpected(string requestId, bool expectedResult)
        {
            ErrorViewModel errorViewModel = new() { RequestId = requestId };
            errorViewModel.ShowRequestId.Should().Be(expectedResult);
        }
    }
}
