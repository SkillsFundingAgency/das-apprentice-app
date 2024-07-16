using FluentAssertions;
using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Domain.UnitTests.Models
{
    internal class ApprenticeAddSubscriptionRequestTests
    {
        [Test]
        public void ApprenticeAddSubscriptionRequest_Should_SetPropertiesCorrectly()
        {
            var endpoint = "https://example.com";
            var publicKey = "publicKey";
            var authenticationSecret = "secret";
            var isSubscribed = true;

            var request = new ApprenticeAddSubscriptionRequest
            {
                Endpoint = endpoint,
                PublicKey = publicKey,
                AuthenticationSecret = authenticationSecret,
                IsSubscribed = isSubscribed
            };

            request.Endpoint.Should().Be(endpoint);
            request.PublicKey.Should().Be(publicKey);
            request.AuthenticationSecret.Should().Be(authenticationSecret);
            request.IsSubscribed.Should().Be(isSubscribed);
        }

        [Test]
        public void ApprenticeDetailsResponse_Should_SetPropertiesCorrectly()
        {
            var apprentice = new Apprentice(); 
            var myApprenticeship = new Apprenticeship(); 

            var response = new ApprenticeDetailsResponse
            {
                Apprentice = apprentice,
                MyApprenticeship = myApprenticeship
            };

            response.Apprentice.Should().Be(apprentice);
            response.MyApprenticeship.Should().Be(myApprenticeship);
        }
    }
}
