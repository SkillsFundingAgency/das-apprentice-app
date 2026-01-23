using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class TasksPageModel
    {
        public int Year { get; set; }
        public List<ApprenticeTask> Tasks { get; set; }
    }
}
