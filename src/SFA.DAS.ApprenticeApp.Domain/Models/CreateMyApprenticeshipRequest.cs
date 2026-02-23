namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class CreateMyApprenticeshipRequest
    {
        public long ApprenticeshipId { get; set; }
        public long Uln { get; set; }
        public string EmployerName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long TrainingProviderId { get; set; }
        public string TrainingProviderName { get; set; }
    }
}
