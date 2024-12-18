using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewComponents
{
    public class NotificationCounter : ViewComponent
    {
        private readonly IOuterApiClient _client;

        public NotificationCounter(IOuterApiClient client)
        {
            _client = client;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);
            var notificationsResult =  await _client.GetTaskReminderNotifications(new Guid(apprenticeId));
            if (notificationsResult != null && notificationsResult.TaskReminders.Count > 0)
            {
                int notificationValue = notificationsResult.TaskReminders.Count;
                return View("../_NotificationCount", notificationValue);
            }
            return null;
        }
    }
}