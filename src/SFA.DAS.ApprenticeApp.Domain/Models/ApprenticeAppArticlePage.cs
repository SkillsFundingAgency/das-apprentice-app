namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeAppArticlePage
    {
        public ApprenticeAppPageSysProperties? Sys { get; set; }
        public string? Slug { get; set; }
        public string? Heading { get; set; }
        public object? Content { get; set; }
        public string? Id { get; set; }
        public int? ArticleOrder { get; set; }

        public string? ArticleText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace((string?)Content))
                {
                    return Markdig.Markdown.ToHtml(Content.ToString());
                }
                    
                return "";
            }
        }
    }
}