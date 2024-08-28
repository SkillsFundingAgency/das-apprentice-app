﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        [Get("/supportguidance/categories/{contentType}")]
        Task<List<ApprenticeAppCategoryPage>> GetCategories([Path] string contentType);

        [Get("/supportguidance/category/{slug}/articles/{id}")]
        Task<ApprenticeAppContentPageCollection> GetArticlesForCategory([Path] string slug, [Path] Guid? id = null);

        [Get("/supportguidance/savedarticles/{id}")]
        Task<ApprenticeAppContentPageCollection> GetSavedArticles([Path] Guid? id = null);

        [Post("/apprentices/{id}/articles/{articleIdentifier}")]
        Task AddUpdateApprenticeArticle([Path] Guid id, [Path] string articleIdentifier, [Body] ApprenticeArticleRequest request);

        [Post("/apprentices/{id}/subscriptions")]
        Task ApprenticeAddSubscription([Path] Guid id, [Body] ApprenticeAddSubscriptionRequest request);

        [Delete("/apprentices/{id}/subscriptions")]
        Task ApprenticeRemoveSubscription([Path] Guid id, [Body] ApprenticeRemoveSubscriptionRequest request);

        [Get("/apprentices/{id}/progress/tasks")]
        Task<ApprenticeTasksCollection> GetApprenticeTasks([Path] long id, int status, DateTime fromDate, DateTime toDate);

        [Get("/apprentices/{id}/progress/tasks/{taskId}")]
        Task<ApprenticeTasksCollection> GetApprenticeTaskById([Path] long id, [Path] int taskId);

        [Post("/apprentices/{apprenticeshipId}/progress/tasks")]
        Task AddApprenticeTask([Path] long apprenticeshipId, [Body] ApprenticeTask request);

        [Put("/apprentices/{id}/progress/tasks/{taskId}")]
        Task UpdateApprenticeTask([Path] long id, [Path] int taskId, [Body] ApprenticeTask request);

        [Delete("/apprentices/{id}/progress/tasks/{taskId}")]
        Task DeleteApprenticeTask([Path] long id, [Path] int taskId);

        [Get("/apprentices/{id}/apprenticeship/{standardUid}/options/{option}/ksbs")]
        Task<List<ApprenticeKsb>> GetApprenticeshipKsbs([Path] long id, [Path] string standardUid, [Path] string option);

        [Get("/apprentices/{id}/ksbs")]
        Task<List<ApprenticeKsbProgressData>> GetApprenticeshipKsbProgresses([Path] long id, [Query] Guid[] guids);

        [Post("/apprentices/{id}/ksbs")]
        Task AddUpdateKsbProgress([Path] long id, [Body] ApprenticeKsbProgressData data);

        [Get("/apprentices/{id}/progress/taskCategories")]
        Task<ApprenticeTaskCategoryCollection> GetTaskCategories([Path] long id);

        [Post("/apprentices/{id}/progress/tasks/{taskId}/status/{statusId}")]
        Task UpdateTaskStatus([Path] long id, [Path] int taskId, [Path] int statusId);

        [Get("/apprentices/{id}/ksbs/taskid/{taskId}")]
        Task<List<ApprenticeKsbProgressData>> GetKsbProgressForTask([Path] long id, [Query] int taskId);

    }
}