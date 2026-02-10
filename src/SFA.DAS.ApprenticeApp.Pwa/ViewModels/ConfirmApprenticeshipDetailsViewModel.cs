namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class ConfirmApprenticeshipDetailsViewModel
    {
        public Guid ApprenticeId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long RevisionId { get; set; }
        public string? FullName { get; set; }
        public string? Employer { get; set; }
        public string? Provider { get; set; }
        public string? Apprenticeship { get; set; }
        public string? Level { get; set; }
        public string? Type { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? ApprenticeshipDurationReduction { get; set; }
        public string? TrainingReduction { get; set; }
    }
}
