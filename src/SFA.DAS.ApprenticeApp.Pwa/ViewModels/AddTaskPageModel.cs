using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class AddTaskPageModel
    {
        public ApprenticeTask? Task { get; set; }
        public List<ApprenticeshipCategory>? Categories { get; set; }
    }
}
