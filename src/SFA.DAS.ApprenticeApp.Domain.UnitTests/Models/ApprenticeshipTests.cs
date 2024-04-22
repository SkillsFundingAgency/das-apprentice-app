using FluentAssertions;
using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Domain.UnitTests.Models
{
    public class ApprenticeshipTests
    {
        [Test]
        [TestCase(null, false)]
        [TestCase("01/01/2000", true)]
        public void IsStoppedReturnsExpected(DateTime? stoppedReceivedOn, bool isStopped)
        {
            Apprenticeship apprenticeship = new();
            apprenticeship.StoppedReceivedOn = stoppedReceivedOn;
            apprenticeship.IsStopped.Should().Be(isStopped);
        }
    }
}
