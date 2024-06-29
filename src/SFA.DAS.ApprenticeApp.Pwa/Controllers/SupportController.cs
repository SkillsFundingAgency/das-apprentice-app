using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Api;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [ExcludeFromCodeCoverage]
    public class SupportController : Controller
    {
        private readonly ILogger<SupportController> _logger;
        private readonly IOuterApiClient _client;

        public SupportController
            (
            ILogger<SupportController> logger,
            IOuterApiClient client
            )
        {
            _logger = logger;
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var pages = await _client.GetCategories(Constants.ContentfulTopLevelPageTypeName);

            return View(new SupportCategoryPageModel() { Categories = pages });
        }

        [HttpGet]
        [Route("~/Support/Category/{slug?}")]
        public async Task<IActionResult> ArticlesPage(string slug)
        {
            var apprenticeId = "fd0daf58-af19-440d-b52f-7e1d47267d3b";
            var contentPageCollection = new ApprenticeAppContentPageCollection();

            if (UserIsAuthenticated())
            {
                apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;
                contentPageCollection = await _client.GetArticlesForCategory(slug, new Guid(apprenticeId));
            }
            else
            {
                // CHANGE APP ID TO NULL
                contentPageCollection = await _client.GetArticlesForCategory(slug, new Guid(apprenticeId));
            }

            return View(new SupportArticlesPageModel() { Articles = contentPageCollection.Articles, ApprenticeArticles = contentPageCollection.ApprenticeArticles?.ApprenticeArticles, CategoryPage = contentPageCollection.CategoryPage, Slug = slug });
        }

        public async Task<IActionResult> SavedArticles()
        {
            var apprenticeId = "fd0daf58-af19-440d-b52f-7e1d47267d3b";
            var savedArticles = new ApprenticeAppContentPageCollection();

            if (UserIsAuthenticated())
            {
                apprenticeId = HttpContext.User?.Claims?.First(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;
                savedArticles = await _client.GetSavedArticles(new Guid(apprenticeId));
            }
            else
            {
                // CHANGE APP ID TO NULL
                savedArticles = await _client.GetSavedArticles(new Guid(apprenticeId));
            }
            
            return View(new SupportArticlesPageModel() { Articles = savedArticles.Articles, ApprenticeArticles = savedArticles.ApprenticeArticles?.ApprenticeArticles });
        }

        [HttpGet]
        public async Task<IActionResult> AddOrUpdateApprenticeArticle(string entryId, bool? likeStatus = null, bool? isSaved = null)
        {
            var apprenticeId = "fd0daf58-af19-440d-b52f-7e1d47267d3b";

            await _client.AddUpdateApprenticeArticle(new Guid(apprenticeId), entryId, new ApprenticeArticleRequest() { LikeStatus = likeStatus, IsSaved = isSaved });
            return Ok();
        }

        protected bool UserIsAuthenticated()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return true;
            }

            return false;
        }
    }
}