using RestEase;
using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Domain.Interfaces
{
    public interface IOuterApiClient
    {
        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Get("/apprentices/{id}/homepage")]
        Task<ApprenticeHomepage> GetApprenticeHomepage([Path] Guid id);
    }
}