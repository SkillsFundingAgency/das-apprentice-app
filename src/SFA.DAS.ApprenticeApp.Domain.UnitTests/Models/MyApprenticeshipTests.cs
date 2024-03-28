using FluentAssertions;
using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Domain.UnitTests.Models
{

    public class MyApprenticeshipTests
    {
        [Test]
        [TestCase(null, null, null)]
        [TestCase("01/01/2024", "01/01/2026", "731:00:00:00")]
        [TestCase("01/01/2024", null, null)]
        [TestCase(null, "01/01/2026", null)]

        public void IsApprenticeShipLengthValid(DateTime? startDate, DateTime? endDate, TimeSpan? length)
        {
            MyApprenticeship myApprenticeship = new();
            myApprenticeship.StartDate = startDate;
            myApprenticeship.EndDate = endDate;

            myApprenticeship.ApprenticeshipLength.Should().Be(length);

        }
    }
}
