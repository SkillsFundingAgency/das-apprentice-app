using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    //public class NotificationViewModel
    //{
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public string TimeAgo { get; set; }
    //    public bool IsNew { get; set; }
    //    public string LinkUrl { get; set; }
    //}

    public class NotificationPageModel
    { 
        public List<ApprenticeTaskReminder> TaskReminders { get; set; }
    }

    
}