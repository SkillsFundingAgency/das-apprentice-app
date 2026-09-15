using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class CheckUlnViewModel
    {
        public Guid ApprenticeId { get; set; }
        public List<ApprenticeshipIds>? ApprenticeshipIds { get; set; }
        public string? Uln { get; set; }
    }

    public class ApprenticeshipIds
    {
        public Guid RegistrationId { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long RevisionId { get; set; }
        public long CommitmentsApprenticeshipId { get; set; }
    }
}
