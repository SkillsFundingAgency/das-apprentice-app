using FluentAssertions;

namespace SFA.DAS.ApprenticeApp.Application.UnitTests
{
    public class WhenGettingConstants
    {
        [Test]
        [TestCase(Constants.StubAuthCookieName, "SFA.ApprenticeApp.StubAuthCookie")]
        [TestCase(Constants.ApprenticeIdClaimKey, "apprentice_id")]
        public void Then_The_Correct_Values_Are_Returned(string constant, string expectedValue)
        {
            constant.Should().Be(expectedValue);
        }
    }
}
