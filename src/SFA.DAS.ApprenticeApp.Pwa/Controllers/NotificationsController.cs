using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly IOuterApiClient _client;

        public NotificationsController(
                    ILogger<NotificationsController> logger,
                    IOuterApiClient client
                    )
        {
            _logger = logger;
            _client = client;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);

            if (!string.IsNullOrEmpty(apprenticeId))
            {
                try
                {
                    var notificationsResult = await _client.GetTaskReminderNotifications(new Guid(apprenticeId));
                    var vm = new NotificationPageModel
                    {
                        TaskReminders = notificationsResult.TaskReminders
                    };
                    return View(vm);
                }
                catch (Exception)
                {
                    _logger.LogWarning("Error in Notifications: GetTaskReminderNotifications");
                } 
            }
            else
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Notifications Index.");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DeleteNotification(int taskId)
        {
            var apprenticeId = Claims.GetClaim(HttpContext, Constants.ApprenticeIdClaimKey);
            
            if (!string.IsNullOrEmpty(apprenticeId))
            {
                try
                {
                    _logger.LogInformation("Updating reminder notification for {taskId} to dismissed", taskId);
                    await _client.UpdateTaskReminderStatus(new Guid(apprenticeId), taskId, (int)ReminderStatus.Dismissed );
                }
                catch (Exception)
                {
                    _logger.LogWarning("Error in Notifications: DeleteTaskReminderNotification");
                }
            }
            else
            {
                _logger.LogWarning("ApprenticeId not found in user claims for Notifications DeleteNotification.");
            }
            return RedirectToAction("Index");
        }
    }
}
