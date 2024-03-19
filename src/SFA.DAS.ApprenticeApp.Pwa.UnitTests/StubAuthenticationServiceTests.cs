using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests
{
    public class StubAuthenticationServiceTests
    {
        [Test, MoqAutoData]
        public void Then_Cookies_Are_Added_To_The_Response_When_local(
            StubAuthUserDetails model,
            [Frozen] Mock<IResponseCookies> responseCookies,
            [Frozen] Mock<IConfiguration> configuration)
        {
            configuration.Setup(x => x["StubAuth"]).Returns("true");
            var service = new StubAuthenticationService(configuration.Object, null, null);
            service.AddStubApprenticeAuth(responseCookies.Object, model);
            responseCookies.Verify(x => x.Append(Constants.StubAuthCookieName, JsonConvert.SerializeObject(model), It.Is<CookieOptions>(c => c.Domain!.Equals("localhost") && !c.IsEssential)));
        }

        [Test, MoqAutoData]
        public void Then_Cookies_Are_Added_To_The_Response_With_Optional_Parameters(
            StubAuthUserDetails model,
            [Frozen] Mock<IResponseCookies> responseCookies,
            [Frozen] Mock<IConfiguration> configuration)
        {
            configuration.Setup(x => x["StubAuth"]).Returns("true");
            var service = new StubAuthenticationService(configuration.Object, null, null);

            service.AddStubApprenticeAuth(responseCookies.Object, model, true);

            responseCookies.Verify(x => x.Append(Constants.StubAuthCookieName, JsonConvert.SerializeObject(model),
                It.Is<CookieOptions>(c =>
                    c.Domain!.Equals("localhost")
                    && c.IsEssential
                    && c.Secure
                    && c.HttpOnly
                    && c.Path!.Equals("/")
                    )));
        }

        [Test, MoqAutoData]
        public void Then_Cookies_Are_Not_Added_To_The_Response_When_Prod(
            StubAuthUserDetails model,
            [Frozen] Mock<IResponseCookies> responseCookies,
            [Frozen] Mock<IConfiguration> configuration)
        {
            configuration.Setup(x => x["EnvironmentName"]).Returns("prd");
            var service = new StubAuthenticationService(configuration.Object, null, null);

            service.AddStubApprenticeAuth(responseCookies.Object, model);

            responseCookies.Verify(x => x.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never());
        }

        [Test, MoqAutoData]
        public async Task GetStubSignInClaims_Then_The_Claims_Are_Added_From_The_Model(
            StubAuthUserDetails model,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] Mock<ICustomClaims> claims)
        {
            httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            claims.Setup(x => x.GetClaims(It.IsAny<TokenValidatedContext>())).ReturnsAsync(new List<Claim>());
            var service = new StubAuthenticationService(configuration.Object, claims.Object, httpContextAccessor.Object);

            var actual = await service.GetStubSignInClaims(model);

            actual!.Identity!.AuthenticationType.Should().Be(CookieAuthenticationDefaults.AuthenticationScheme);
            actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))!.Value.Should()
                .Be(model.Email);
            actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))!.Value.Should()
                .Be(model.Id);
            actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals("sub"))!.Value.Should()
                .Be(model.Id);
        }

        [Test, MoqAutoData]
        public async Task GetStubSignInClaims_Then_The_Custom_Clams_Are_Added(
            string claimValue,
            string claimKey,
            StubAuthUserDetails model,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] Mock<ICustomClaims> claims)
        {
            httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            var claim = new Claim(claimKey, claimValue);
            claims.Setup(x => x.GetClaims(It.IsAny<TokenValidatedContext>())).ReturnsAsync(new List<Claim> { claim });
            var service = new StubAuthenticationService(configuration.Object, claims.Object, httpContextAccessor.Object);
            var actual = await service.GetStubSignInClaims(model);

            actual.Identities.FirstOrDefault()!.AuthenticationType.Should().Be(CookieAuthenticationDefaults.AuthenticationScheme);
            actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals(claimKey))!.Value.Should()
                .Be(claimValue);
        }
    }
}