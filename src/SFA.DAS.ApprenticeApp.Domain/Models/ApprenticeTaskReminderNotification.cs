namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeTaskReminder
    {
        public int TaskId { get; set; }
        public long ApprenticeshipId { get; set; }
        public Guid ApprenticeAccountId { get; set; }
        public DateTime? DueDate { get; set; }
        public string Title { get; set; }
        public int? ApprenticeshipCategoryId { get; set; }
        public string Note { get; set; }
        public DateTime? CompletionDateTime { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public int? ReminderValue { get; set; }
        public ReminderUnit? ReminderUnit { get; set; }
        public bool IsNew
        {
            get
            {
                return ReminderStatus == Domain.Models.ReminderStatus.NotSent;
            }
        }
        public ReminderStatus? ReminderStatus { get; set; }
    }

    public class ApprenticeTaskRemindersCollection
    {
        public List<ApprenticeTaskReminder> TaskReminders { get; set; }
    }
}
