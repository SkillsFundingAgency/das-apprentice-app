using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.ApprenticeApp.Pwa.Services;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart;

public class CustomClaims : ICustomClaims
{
    public async Task<IEnumerable<Claim?>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var value = tokenValidatedContext?.Principal?.Identities.First().Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            ?.Value;
        return new List<Claim>
        {
            new Claim("ApprenticeId",$"ABC123-{value}"),
            new Claim(ClaimTypes.Name,$"Mr Active Apprentice"),
            new Claim(ClaimTypes.DateOfBirth, "01/01/2005")
        };
    }
}