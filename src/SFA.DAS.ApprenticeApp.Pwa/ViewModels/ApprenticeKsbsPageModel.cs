using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class ApprenticeKsbsPageModel
    {
        public List<ApprenticeKsb>? Ksbs { get; set; }

        public int? KnowledgeCount { get; set; }
        public int? SkillCount { get; set; }
        public int? BehaviourCount { get; set; }
    }
}
