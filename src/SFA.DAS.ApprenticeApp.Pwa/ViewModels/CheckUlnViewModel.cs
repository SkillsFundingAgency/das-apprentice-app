using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class CheckUlnViewModel
    {
        public Guid ApprenticeId { get; set; }          
        public List<RegistrationDetails> RegistrationIds { get; set; }
        public int? Uln { get; set; }
    }

    public class RegistrationDetails
    {        
        public long? CommitmentApprenticeshipIds { get; set; }
        public Guid RegistrationId { get; set; }
    }
}
