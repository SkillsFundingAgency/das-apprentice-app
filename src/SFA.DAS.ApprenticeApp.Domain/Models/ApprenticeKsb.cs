using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class ApprenticeKsb
    {
        public KsbType Type { get; set; }
        public Guid Id { get; set; }
        public required string Key { get; set; }
        public required string Detail { get; set; }
        public int? TaskId { get; set; }
        public KSBStatus? Status { get; set; }
    }

    public enum KsbType
    {
        Knowledge = 1,
        Skill = 2,
        Behaviour = 3,
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
    public class ApprenticeKsbCollection
    {
        public ApprenticeKsb[] ApprenticeKsbs { get; set; }
    }
}
