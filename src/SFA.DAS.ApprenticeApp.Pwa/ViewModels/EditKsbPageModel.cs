using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class EditKsbPageModel
    {
        public ApprenticeKsbProgressData? KsbProgress { get; set; }
        public List<KSBStatus> KsbStatuses { get; set; }
        public string KsbDetail { get; set; }
    }
}
