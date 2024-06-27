namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeAppContentPageCollection
    {
        public List<ApprenticeAppCategoryPage>? CategoryPage { get; set; }
        public List<ApprenticeAppArticlePage>? Articles { get; set; }
        public ApprenticeArticlesCollection? ApprenticeArticles { get; set; }
    }

    public class ApprenticeArticlesCollection
    {
        public List<ApprenticeArticle>? ApprenticeArticles { get; set; }
    }
}