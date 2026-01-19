using SFA.DAS.ApprenticeApp.Application;

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public class ApprenticeContext : IApprenticeContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApprenticeContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? ApprenticeId
        {
            get
            {
                var sesionValue = _httpContextAccessor.HttpContext?.Session.GetString("_currentApprenticeId");

                if (Guid.TryParse(sesionValue, out var id))
                    return id.ToString();      
                
                var claim = _httpContextAccessor.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == Constants.ApprenticeIdClaimKey)?.Value;

                return Guid.TryParse(claim, out var claimId) ? claimId.ToString() : null;
            }
        }
    }
}
