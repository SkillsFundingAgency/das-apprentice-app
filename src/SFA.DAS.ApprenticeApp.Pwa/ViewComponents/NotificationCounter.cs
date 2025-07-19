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
            if(!string.IsNullOrEmpty(apprenticeId))
            {
                var notificationValue = 0;
                
                var taskNotificationsResult = await _client.GetTaskReminderNotifications(new Guid(apprenticeId));
                if (taskNotificationsResult != null && taskNotificationsResult.TaskReminders.Count > 0)
                {
                    notificationValue += taskNotificationsResult.TaskReminders.Count;
                }
                
                // todo surveys
                var surveryCookie = Request.Cookies["SurveyNotification"];
                if (surveryCookie != null)
                {
                    var surveryCookieValue = int.Parse(Request.Cookies["SurveyNotification"]);

                    if (surveryCookieValue == 1)
                    {
                        notificationValue++;
                    }
                }
                    
                return View("_NotificationCount", notificationValue);                
                
            }
            return Content(string.Empty);
        }
    }
}