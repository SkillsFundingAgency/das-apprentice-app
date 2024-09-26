using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class AddTaskPageModel : TaskViewModelBase
    {
        
    }

        public class EditTaskPageModel :TaskViewModelBase
    {

        public List<ApprenticeKsbData>? KsbProgressData { get; set; }
        public string? LinkedKsbGuids { get; set; }
    }

    public class TaskViewModelBase 
    {
        public int? StatusId { get; set; }
        public ApprenticeTask? Task { get; set; }
        public List<ApprenticeshipCategory>? Categories { get; set; }
    }
}


