using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSavedArticle(string entryId, string entryTitle, bool isSaved, string? slug = null)
        {
            var apprenticeId = _apprenticeContext.ApprenticeId;

            if (string.IsNullOrEmpty(apprenticeId))
            {
                return RedirectToAction("Index", "Home");
            }

            await _client.AddUpdateApprenticeArticle(new Guid(apprenticeId), entryId, Slugify(entryTitle), new ApprenticeArticleRequest() { IsSaved = isSaved });

            // The enhanced journey posts this form in the background and updates the
            // button itself, so there is nothing to redirect to.
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Ok();
            }

            // A slug means the post came from a category page, where we send the
            // apprentice back to the article they acted on. Without one it came from
            // their saved articles, where a removed article is no longer on the page.
            return string.IsNullOrEmpty(slug)
                ? RedirectToAction("SavedArticles")
                : RedirectToAction("ArticlesPage", "Support", new { slug }, $"accordion-default-content-{entryId}");
        }

        private static string Slugify(string entryTitle) =>
            Regex.Replace(entryTitle ?? string.Empty, @"\s+", "-").ToLowerInvariant();
    }
}
