using Microsoft.AspNetCore.JsonPatch;
using RestEase;
using SFA.DAS.ApprenticeApp.Domain.Api;
using SFA.DAS.ApprenticeApp.Domain.Models;

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

        // Support and Guidance content yield
        [Get("/supportguidance/categories/{contentType}")]
        Task<List<ApprenticeAppCategoryPage>> GetCategories([Path] string contentType);

        [Get("/supportguidance/category/{slug}/articles/{id}")]
        Task<ApprenticeAppContentPageCollection> GetArticlesForCategory([Path] string slug, [Path] Guid? id = null);

        // Saved articles
        [Get("/supportguidance/savedarticles/{id}")]
        Task<ApprenticeAppContentPageCollection> GetSavedArticles([Path] Guid? id = null);

        // add or update apprenticeArticle saved status
        [Post("/apprentices/{id}/articles/{articleIdentifier}")]
        Task AddUpdateApprenticeArticle([Path] Guid id, [Path] string articleIdentifier, [Body] ApprenticeArticleRequest request);
    }
}