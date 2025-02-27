using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Application;
using OpenTelemetry;

public class UserIdProcessor : BaseProcessor<Activity>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserIdProcessor> _logger;

    public UserIdProcessor(IHttpContextAccessor httpContextAccessor, ILogger<UserIdProcessor> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void OnStart(Activity activity)
    {
        if (activity == null)
        {
            _logger.LogWarning("UserIdProcessor: Activity is null, skipping.");
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userId = Claims.GetClaim(httpContext, Constants.ApprenticeIdClaimKey);
            if (!string.IsNullOrEmpty(userId))
            {
                // Set user ID for OpenTelemetry tracing
                activity.SetTag("enduser.id", userId);

                // Set user ID for Application Insights
                activity.SetTag("ai.user.id", userId);

                _logger.LogInformation($"UserIdProcessor: Set enduser.id and ai.user.id = {userId}");
            }
            else
            {
                _logger.LogWarning("UserIdProcessor: No userId found in claims");
            }
        }
        else
        {
            _logger.LogDebug("UserIdProcessor: User is not authenticated yet. Skipping.");
        }
    }
}
