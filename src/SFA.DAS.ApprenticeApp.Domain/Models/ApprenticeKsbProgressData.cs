namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeKsbProgressData
    {
        public long ApprenticeshipId { get; set; }
        public KsbType? KSBProgressType { get; set; }
        public Guid KSBId { get; set; }
        public string KsbKey { get; set; }
        public KSBStatus? CurrentStatus { get; set; }
        public string Note { get; set; }
        public List<ApprenticeTask>? Tasks { get; set; }
    }
}
