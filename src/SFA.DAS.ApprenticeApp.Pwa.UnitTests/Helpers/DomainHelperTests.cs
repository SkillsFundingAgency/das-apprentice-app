using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Helpers
{
    public class DomainHelperTests
    {
        [Test]
        [TestCase("https://xyz", true)]
        [TestCase(".localhost", false)]
        public void Return_Correct_Is_Secure(string parentDomain, bool expectedResult)
        {
            var actual = new DomainHelper(parentDomain);
            actual.ParentDomain.Should().Be(parentDomain);
            actual.Secure.Should().Be(expectedResult);

        }
    }
}
