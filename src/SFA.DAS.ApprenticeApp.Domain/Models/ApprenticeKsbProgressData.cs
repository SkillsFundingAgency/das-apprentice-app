using System.ComponentModel;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeKsbProgressData
    {
        public long ApprenticeshipId { get; set; }
        public int KsbProgressId { get; set; }
        public KsbType? KsbProgressType { get; set; }
        public Guid KsbId { get; set; }
        public string KsbKey { get; set; }
        public KSBStatus? CurrentStatus { get; set; }
        public string Note { get; set; }
        public List<ApprenticeTask>? Tasks { get; set; }
    }

    [Flags]
    public enum KSBStatus
    {
        [Description("Not started")]
        NotStarted = 0,

        [Description("In progress")]
        InProgress = 1,

        [Description("Ready for review")]
        ReadyForReview = 2,

        [Description("Completed")]
        Completed = 3
    }

    
}
