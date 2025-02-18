using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Application;

public class UserIdProcessor : BaseProcessor<Activity>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserIdProcessor> _logger;
    private const string TargetPath = "/Tasks/Index"; // Case-sensitive

    public UserIdProcessor(IHttpContextAccessor httpContextAccessor, ILogger<UserIdProcessor> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public override void OnStart(Activity activity)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            _logger.LogWarning("UserIdProcessor: HttpContext is null. Skipping.");
            return;
        }

        if (!httpContext.Request.Path.HasValue)
        {
            _logger.LogWarning("UserIdProcessor: Request.Path is null or empty.");
            return;
        }

        var requestPath = httpContext.Request.Path.Value!;
        _logger.LogDebug($"UserIdProcessor: Captured request path: {requestPath}");

        // ✅ Ensure telemetry is only processed after the user reaches /Tasks/Index
        if (!requestPath.Equals(TargetPath, StringComparison.Ordinal))
        {
            _logger.LogDebug($"UserIdProcessor: Skipping telemetry, not on {TargetPath} yet.");
            return;
        }

        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            var userId = Claims.GetClaim(httpContext, Constants.ApprenticeIdClaimKey);
            if (!string.IsNullOrEmpty(userId))
            {
                activity.SetTag("enduser.id", userId);
                _logger.LogDebug($"UserIdProcessor: Set enduser.id = {userId}");
            }
            else
            {
                _logger.LogWarning("UserIdProcessor: No userId found in claims after reaching /Tasks/Index.");
            }
        }
        else
        {
            _logger.LogDebug("UserIdProcessor: User is not authenticated yet. Skipping.");
        }
    }
}
