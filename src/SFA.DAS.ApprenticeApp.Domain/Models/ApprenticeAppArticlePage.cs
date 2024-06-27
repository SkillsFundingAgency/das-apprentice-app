namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeAppArticlePage
    {
        public ApprenticeAppPageSysProperties? Sys { get; set; }
        public string? Slug { get; set; }
        public string? Heading { get; set; }
        public object? Content { get; set; }
        public string? Id { get; set; }
    }
}