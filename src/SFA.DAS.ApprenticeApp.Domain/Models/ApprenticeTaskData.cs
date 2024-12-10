namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeTaskData
    {
        public ApprenticeTask Task { get; set; }
        public ApprenticeTaskCategoryCollection TaskCategories { get; set; }
        public List<ApprenticeKsbData>? KsbProgress { get; set; }
    }
}
