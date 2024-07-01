namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeAppCategoryPage
    {
        public ApprenticeAppPageSysProperties? Sys { get; set; }
        public string? Slug { get; set; }
        public string? Heading { get; set; }
        public object? Content { get; set; }
    }
}