using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
public class LearnerNotification
    {
        public long NotificationId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? LearnerAccountId { get; set; }
        public string Category { get; set; }
        public string Heading { get; set; }
        public string Body { get; set; }
        public byte? StatusId { get; set; }
        public DateTime? NotificationTime { get; set; }
        public DateTime? TimeToExpire { get; set; }
        public DateTime? TimeReceived { get; set; }
        public string Link { get; set; }
        public byte? UrgencyId { get; set; }
    }

    public class LearnerNotificationsCollection
    {
        public List<LearnerNotification> Notifications { get; set; }
    }
}