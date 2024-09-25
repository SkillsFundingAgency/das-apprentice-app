using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.AppStart;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.AppStart
{
    public class WhenGettingStubCustomClaims
    {
        //[Test, MoqAutoData]
        //public async Task Then_Additional_Claims_Are_Added(CustomClaims customClaims)
        //{
        //    var tokenValidatedContext = ArrangeTokenValidatedContext();
        //    var result = await customClaims.GetClaims(tokenValidatedContext);
            
        //    result.Count().Should().Be(4);
        //}

        //private TokenValidatedContext ArrangeTokenValidatedContext()
        //{
     
        //    var identity = new ClaimsIdentity(new List<Claim>());

        //   var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(identity));
        //    return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(ApprenticeStubAuthHandler)),
        //        new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
        //    {
        //        Principal = claimsPrincipal
        //    };
        //}
    }
}
