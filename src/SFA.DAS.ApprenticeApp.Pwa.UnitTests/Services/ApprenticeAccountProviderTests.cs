using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RestEase;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Services
{
    public class ApprenticeAccountProviderTests
    {
        [Test, MoqAutoData]
        public async Task GetApprenticeAccount_Returns_Null_If_Not_Found(
            [Frozen] Mock<IOuterApiClient> client,
            ApprenticeAccountProvider sut)
        {
            var id = Guid.NewGuid();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "apprentice/id");
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                ReasonPhrase = "NotFound",
                Content = new StringContent("Not Found")
            };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var apiException = await ApiException.CreateAsync(requestMessage, responseMessage);

            client.Setup(x => x.GetApprentice(id)).ThrowsAsync(apiException);

            var result = await sut.GetApprenticeAccount(id);

            result.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task PutApprenticeAccount_Returns_Null_If_Not_Found(
            [Frozen] Mock<IOuterApiClient> client,
            ApprenticeAccountProvider sut)
        {
            client.Setup(x => x.PutApprentice(It.IsAny<PutApprenticeRequest>())).ReturnsAsync((Apprentice)null);
            var result = await sut.PutApprenticeAccount("email", "govIdentifier");

            result.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task PutApprenticeAccount_Returns_Apprentice(
           [Frozen] Mock<IOuterApiClient> client,
           ApprenticeAccountProvider sut)
        {
            var result = await sut.PutApprenticeAccount("email", "govIdentifier") as Apprentice;

            result.Should().BeOfType<Apprentice>();
            result.ApprenticeId.Should().NotBeEmpty();
        }
    }
}
