using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class TasksPageModel
    {
        public int Year { get; set; }
        public List<ApprenticeTask> TasksTodo { get; set; }
        public List<ApprenticeTask> TasksDone { get; set; }
    }
}
