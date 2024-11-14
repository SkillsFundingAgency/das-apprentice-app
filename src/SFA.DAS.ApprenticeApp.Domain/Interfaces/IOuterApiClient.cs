using Microsoft.AspNetCore.JsonPatch;
using RestEase;
using SFA.DAS.ApprenticeApp.Domain.Api;
using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Domain.Interfaces
{
    public interface IOuterApiClient
    {
        //Apprentice
        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Patch("/apprentices/{id}")]
        Task UpdateApprentice([Path] Guid id, [Body] JsonPatchDocument<Apprentice> patch);

        [Post("/apprentices/{id}/subscriptions")]
        Task ApprenticeAddSubscription([Path] Guid id, [Body] ApprenticeAddSubscriptionRequest request);

        [Delete("/apprentices/{id}/subscriptions")]
        Task ApprenticeRemoveSubscription([Path] Guid id, [Body] ApprenticeRemoveSubscriptionRequest request);

        [Put("/apprentices")]
        Task<Apprentice> PutApprentice([Body] PutApprenticeRequest request);

        //ApprenticeDetails
        [Get("/apprentices/{id}/details")]
        Task<ApprenticeDetails> GetApprenticeDetails([Path] Guid id);

        //KsbProgress
        [Post("/apprentices/{id}/ksbs")]
        Task AddUpdateKsbProgress([Path] Guid id, [Body] ApprenticeKsbProgressData data);

        [Get("/apprentices/{id}/ksbs")]
        Task<List<ApprenticeKsb>> GetApprenticeshipKsbs([Path] Guid id);

        [Delete("/apprentices/{id}/ksbs/{ksbProgressId}/taskid/{taskId}")]
        Task RemoveTaskToKsbProgress([Path] Guid id, [Path] int ksbProgressId, [Path] int taskId);

        [Get("/apprentices/{id}/ksbs")]
        Task<List<ApprenticeKsbProgressData>> GetApprenticeshipKsbProgresses([Path] Guid id, [Query] Guid[] guids);

        [Get("/apprentices/{id}/ksbs/taskid/{taskId}")]
        Task<List<ApprenticeKsbProgressData>> GetKsbProgressForTask([Path] Guid id, [Path] int taskId);

        [Get("/apprentices/{id}/ksbs/{ksbId}")]
        Task<ApprenticeKsb> GetApprenticeshipKsb([Path] Guid id, [Path] Guid ksbId);

        //Support&Guidance

        [Get("/supportguidance/categories/{contentType}")]
        Task<List<ApprenticeAppCategoryPage>> GetCategories([Path] string contentType);

        [Get("/supportguidance/category/{slug}/articles/{id}")]
        Task<ApprenticeAppContentPageCollection> GetArticlesForCategory([Path] string slug, [Path] Guid? id = null);

        [Get("/supportguidance/savedarticles/{id}")]
        Task<ApprenticeAppContentPageCollection> GetSavedArticles([Path] Guid? id = null);

        [Post("/apprentices/{id}/articles/{articleIdentifier}")]
        Task AddUpdateApprenticeArticle([Path] Guid id, [Path] string articleIdentifier, [Body] ApprenticeArticleRequest request);

        //Tasks
        [Get("/apprentices/{id}/progress/taskCategories")]
        Task<ApprenticeTaskCategoryCollection> GetTaskCategories([Path] Guid id);

        [Post("/apprentices/{apprenticeshipId}/progress/tasks")]
        Task AddApprenticeTask([Path] long apprenticeshipId, [Body] ApprenticeTask request);

        [Get("/apprentices/{id}/progress/tasks")]
        Task<ApprenticeTasksCollection> GetApprenticeTasks([Path] Guid id, int status, DateTime fromDate, DateTime toDate);

        [Get("/apprentices/{id}/progress/tasks/{taskId}")]
        Task<ApprenticeTasksCollection> GetApprenticeTaskById([Path] Guid id, [Path] int taskId);

        [Delete("/apprentices/{id}/progress/tasks/{taskId}")]
        Task DeleteApprenticeTask([Path] Guid id, [Path] int taskId);

        [Put("/apprentices/{id}/progress/tasks/{taskId}")]
        Task UpdateApprenticeTask([Path] Guid id, [Path] int taskId, [Body] ApprenticeTask request);

        [Post("/apprentices/{id}/progress/tasks/{taskId}/status/{statusId}")]
        Task UpdateTaskStatus([Path] Guid id, [Path] int taskId, [Path] int statusId);

        [Get("/apprentices/{id}/progress/taskCategories/tasks/{taskId}/ksbs")]
        Task<ApprenticeTaskData> GetTaskViewData([Path] Guid id, [Path] int taskId);

        [Get("/apprentices/{id}/progress/tasks/taskReminders")]
        Task<ApprenticeTaskRemindersCollection> GetTaskReminderNotifications([Path] Guid id);
    }
}