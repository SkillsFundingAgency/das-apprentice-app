using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class SupportArticlesPageModel
    {
        public List<ApprenticeAppCategoryPage>? CategoryPage { get; set; }
        public List<ApprenticeAppArticlePage>? Articles { get; set; }
        public List<ApprenticeArticle>? ApprenticeArticles { get; set; }
        public string? Slug { get; set; }
    }
}