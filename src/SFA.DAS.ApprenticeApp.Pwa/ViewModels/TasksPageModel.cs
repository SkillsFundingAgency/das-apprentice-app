using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class TasksPageModel : TasksBaseModel
    {
        public List<ApprenticeTask> Tasks { get; set; }
    }
}
