namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public static class Claims
    {
        public static string GetClaim(HttpContext httpContext, string claimKey)
        {
            if (httpContext.User != null && httpContext.User.Claims.Any())
            {
                try
                {
                    var claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == claimKey);
                    return claim?.Value ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
