using RestEase;
using SFA.DAS.ApprenticeApp.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace SFA.DAS.ApprenticeApp.Domain.Interfaces
{
    public interface IOuterApiClient
    {
        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Get("/apprentices/{id}/details")]
        Task<ApprenticeDetails> GetApprenticeDetails([Path] Guid id);

        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprentice([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);

        [Post("/apprentices/{id}/subscriptions")]
        Task ApprenticeAddSubscription([Path] Guid id, [Body] ApprenticeAddSubscriptionRequest request);

        [Delete("/apprentices/{id}/subscriptions")]
        Task ApprenticeRemoveSubscription([Path] Guid id, [Body] ApprenticeRemoveSubscriptionRequest request);


    }
}