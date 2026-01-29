using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Api;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [ExcludeFromCodeCoverage]
    public class SupportController : Controller
    {
        private readonly IOuterApiClient _client;
        private readonly IApprenticeContext _apprenticeContext;

        public SupportController
            (
            IOuterApiClient client,
            IApprenticeContext apprenticeContext
            )
        {
            _client = client;
            _apprenticeContext = apprenticeContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var pages = await _client.GetCategories(Constants.ContentfulTopLevelPageTypeName);
                return View(new SupportCategoryPageModel() { Categories = pages });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        [Route("~/Support/Category/{slug?}")]
        public async Task<IActionResult> ArticlesPage(string slug)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            { 
                var contentPageCollection = await _client.GetArticlesForCategory(slug, new Guid(apprenticeId));
                return View(new SupportArticlesPageModel() { Articles = contentPageCollection.Articles, ApprenticeArticles = contentPageCollection.ApprenticeArticles?.ApprenticeArticles, CategoryPage = contentPageCollection.CategoryPage, Slug = slug });
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> SavedArticles()
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                var savedArticles = await _client.GetSavedArticles(new Guid(apprenticeId));
                return View(new SupportArticlesPageModel() { Articles = savedArticles.Articles, ApprenticeArticles = savedArticles.ApprenticeArticles?.ApprenticeArticles });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddOrUpdateApprenticeArticle(string entryId, string entryTitle, bool? likeStatus = null, bool? isSaved = null)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                await _client.AddUpdateApprenticeArticle(new Guid(apprenticeId), entryId, entryTitle, new ApprenticeArticleRequest() { LikeStatus = likeStatus, IsSaved = isSaved });
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RemoveApprenticeArticle(string entryId, bool? likeStatus = null, bool? isSaved = null)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                await _client.RemoveApprenticeArticle(new Guid(apprenticeId), entryId, new ApprenticeArticleRequest() { LikeStatus = likeStatus, IsSaved = isSaved });
                return Ok();
            }

            return Unauthorized();
        }
    }
}