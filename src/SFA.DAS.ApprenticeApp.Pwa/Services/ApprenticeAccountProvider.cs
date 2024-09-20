using RestEase;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeApp.Pwa.Services
{
    public class ApprenticeAccountProvider : IApprenticeAccountProvider
    {
        private readonly IOuterApiClient _client;

        public ApprenticeAccountProvider(IOuterApiClient client)
        {
            _client = client;
        }
        public async Task<IApprenticeAccount?> GetApprenticeAccount(Guid id)
        {
            try
            {
                return await _client.GetApprentice(id);
            }
            catch (ApiException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IApprenticeAccount?> PutApprenticeAccount(string email, string govIdentifier)
        {
            
            return await _client.PutApprentice(new PutApprenticeRequest(email, govIdentifier));
        }
    }
}
