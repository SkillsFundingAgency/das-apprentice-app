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
        NotStarted = 0,
        InProgress = 1,
        ReadyForReview = 2,
        Completed = 3
    }
    public class ApprenticeKsbCollection
    {
        public ApprenticeKsb[] ApprenticeKsbs { get; set; }
    }
}
