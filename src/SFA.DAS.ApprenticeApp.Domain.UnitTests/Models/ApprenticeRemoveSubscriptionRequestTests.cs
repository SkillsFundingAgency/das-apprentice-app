using FluentAssertions;
using SFA.DAS.ApprenticeApp.Domain.Models;


namespace SFA.DAS.ApprenticeApp.Domain.UnitTests.Models
{
    internal class ApprenticeRemoveSubscriptionRequestTests
    {
        [Test]
        public void ApprenticeRemoveSubscriptionRequest_Should_SetPropertiesCorrectly()
        {
            var endpoint = "https://example.com";

            var request = new ApprenticeRemoveSubscriptionRequest
            {
                Endpoint = endpoint
            };

            request.Endpoint.Should().Be(endpoint);
        }
    }
}
