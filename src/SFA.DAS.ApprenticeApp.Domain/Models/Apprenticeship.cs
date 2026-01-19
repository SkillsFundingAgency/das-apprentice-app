using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class Apprenticeship
    {
        public long Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public string? EmployerName { get; set; }
        public string? CourseName { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public DateTime ApprovedOn { get; set; }
        public DateTime? LastViewed { get; set; }
        public DateTime? StoppedReceivedOn { get; set; }
        public bool IsStopped => StoppedReceivedOn != null;
        public bool HasBeenConfirmedAtLeastOnce { get; set; }
    }

    public class ApprenticeshipsList
    {
        public List<Apprenticeship>? Apprenticeships { get; set; }
    }
}
