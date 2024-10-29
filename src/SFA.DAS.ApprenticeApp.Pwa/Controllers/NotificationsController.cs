using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            var notifications = new List<NotificationViewModel>
    {
        new NotificationViewModel
        {
            Title = "What is OTJ training?",
            Description = "Find out why off-the-job training is key to your apprenticeship.",
            TimeAgo = "New - 4 hours ago",
            IsNew = true,
            LinkUrl = "#"
        },
        new NotificationViewModel
        {
            Title = "Your end-point assessment",
            Description = "Learn about the EPA you’ll take at the end of the apprenticeship.",
            TimeAgo = "4 hours ago",
            IsNew = false,
            LinkUrl = "#"
        },
        new NotificationViewModel
        {
            Title = "Submit assessment for first module",
            Description = "Due 1 February, 2:30pm\nReminder set: 5 minutes before",
            TimeAgo = "4 hours ago",
            IsNew = false,
            LinkUrl = "#"
        },
        new NotificationViewModel
        {
            Title = "Your rights as an apprentice",
            Description = "Find out what you're entitled to as an apprentice.",
            TimeAgo = "4 hours ago",
            IsNew = false,
            LinkUrl = "#"
        }
    };

            return View(notifications);
        }




    }
}
