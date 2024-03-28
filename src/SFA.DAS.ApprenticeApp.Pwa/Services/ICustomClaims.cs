using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace SFA.DAS.ApprenticeApp.Pwa.Services
{
    public interface ICustomClaims
    {
        Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext);
    }
}