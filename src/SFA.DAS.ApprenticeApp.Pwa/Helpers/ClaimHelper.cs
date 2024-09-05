namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public static class Claims
    {
        public static string GetClaim(HttpContext httpContext, string claimKey)
        {
            if(httpContext.User != null && httpContext.User.Claims.Any())
            {
                try
                {
                    return httpContext.User.Claims?.FirstOrDefault(c => c.Type == claimKey).Value;
                }
                catch
                {
                    return "";
                }
            }
            return "";  
        }
    }
}
