using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.ApprenticeApp.Application;
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
            new Claim(Constants.ApprenticeIdClaimKey,$"fd0daf58-af19-440d-b52f-7e1d47267d3b"),
            //new Claim(Constants.ApprenticeIdClaimKey,$"EB73DC36-1B62-4D0F-A557-6281F006DB8A"),
            new Claim(ClaimTypes.Name,$"Mr Active Apprentice"),
            new Claim(ClaimTypes.DateOfBirth, "01/01/2005")
        };
    }
}