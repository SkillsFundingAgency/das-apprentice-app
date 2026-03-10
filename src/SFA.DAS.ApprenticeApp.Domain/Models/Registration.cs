
namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class Registration
    {
        public Guid RegistrationId { get; set; }
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime CommitmentsApprovedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Email Email { get; set; }
        public Guid? ApprenticeId { get; set; }
        public ApprenticeDetails Approvals { get; set; }
        public DateTime? CreatedOn { get; private set; } = DateTime.UtcNow;
        public DateTime? FirstViewedOn { get; private set; }
        public DateTime? SignUpReminderSentOn { get; private set; }
        public Apprenticeship? Apprenticeship { get; private set; }
        public DateTime? StoppedReceivedOn { get; private set; }
    }

    public class Email
    {
        public string DisplayName { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public string Address { get; set; }
    }
}
