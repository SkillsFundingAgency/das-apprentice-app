using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Account
{
    public class AccountControllerTests
    {
        [Test, MoqAutoData]
        public async Task Loading_Authenticated_Page(
            [Frozen] Mock<ILogger<AccountController>> logger,
            Mock<IRequestCookieCollection> cookies,
            [Greedy] AccountController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var lastnameClaim = new Claim(Constants.ApprenticeLastNameClaimKey, "test");

            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
               apprenticeIdClaim, lastnameClaim
            })});

            cookies.Setup(c => c[Constants.ApprenticeshipIdClaimKey]).Returns("1");
            cookies.Setup(c => c[Constants.StandardUIdClaimKey]).Returns("1");
            httpContext.Request.Cookies = cookies.Object;
            httpContext.User = claimsPrincipal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Authenticated();

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Apprentice authenticated and cookies added for")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
            }
        }

        [Test, MoqAutoData]
        public async Task Loading_Authenticated_Page_LoadsError_ForNoApprentice(
    [Frozen] Mock<IOuterApiClient> client,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] AccountController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns((string?)null);   // ← no apprentice

            var httpContext = new DefaultHttpContext();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.Authenticated() as RedirectToActionResult;

            // Assert
            result!.ActionName.Should().Be("EmailMismatchError");
            result.ControllerName.Should().Be("Account");
        }

        [Test, MoqAutoData]
        public async Task Loading_Authenticated_Page_LoadsError_ForNoApprenticeship(
          [Frozen] Mock<IOuterApiClient> client,
          [Greedy] AccountController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var lastnameClaim = new Claim(Constants.ApprenticeLastNameClaimKey, "test");

            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
               apprenticeIdClaim, lastnameClaim
            })});
            httpContext.User = claimsPrincipal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(client => client.GetApprenticeDetails(It.IsAny<Guid>())).ReturnsAsync(new ApprenticeDetails() { MyApprenticeship = null });
            var result = await controller.Authenticated() as RedirectToActionResult;
            result.ActionName.Should().Be("CmadError");
            result.ControllerName.Should().Be("Account");
        }

        [Test, MoqAutoData]
        public async Task Loading_YourAccount_Page(
        [Frozen] Mock<IApprenticeContext> apprenticeContext,
        [Frozen] Mock<IOuterApiClient> client,
        [Frozen] Mock<IRequestCookieCollection> cookies,
        [Greedy] AccountController controller)
        {
            var apprenticeId = Guid.NewGuid().ToString();

            apprenticeContext.Setup(x => x.ApprenticeId).Returns(apprenticeId);

            var httpContext = new DefaultHttpContext();

            cookies.Setup(c => c[Constants.KsbFiltersCookieName]).Returns("filter=NOT-STARTED");
            httpContext.Request.Cookies = cookies.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var ksbs = new List<ApprenticeKsb>
            {
                new ApprenticeKsb { Type = KsbType.Knowledge },
                new ApprenticeKsb { Type = KsbType.Skill },
                new ApprenticeKsb { Type = KsbType.Behaviour },
                new ApprenticeKsb { Type = KsbType.Skill }
            };

            object value = client.Setup(x => x.GetApprenticeshipKsbs(It.IsAny<Guid>()))
                  .ReturnsAsync(ksbs);

            client.Setup(x => x.GetApprenticeDetails(It.IsAny<Guid>()))
                  .ReturnsAsync(new ApprenticeDetails
                  {
                      MyApprenticeship = new MyApprenticeship()
                  });

            var result = await controller.YourAccount() as ViewResult;

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<ApprenticeAccountModel>();

            var model = result!.Model as ApprenticeAccountModel;
            var ksbsModel = model!.apprenticeKsbsPageModel;

            using (new AssertionScope())
            {
                ksbsModel.Should().NotBeNull();
                ksbsModel!.Ksbs.Should().HaveCount(4);
                ksbsModel.KnowledgeCount.Should().Be(1);
                ksbsModel.SkillCount.Should().Be(2);
                ksbsModel.BehaviourCount.Should().Be(1);
                ksbsModel.MyApprenticeship.Should().NotBeNull();
            }
        }

        [Test, MoqAutoData]
        public void Loading_Error_Page([Greedy] AccountController controller)
        {
            var result = controller.Error() as ActionResult;
            result.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Post_AccountDetails_StubFails_InProd(
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] StubAuthenticationViewModel model,
            [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("PRD");
            var result = await controller.AccountDetails(model);
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test, MoqAutoData]
        public async Task Post_AccountDetails_HandlesError(
            [Frozen] Mock<IConfiguration> configuration,

            [Frozen] Mock<IStubAuthenticationService> authenticationService,
            [Frozen] StubAuthenticationViewModel model,
            [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");
            authenticationService.Setup(x => x.GetStubSignInClaims(model)).Throws(new Exception());
            var result = await controller.AccountDetails(model) as RedirectToActionResult;

            result.ActionName.Should().Be("Error");
            result.ControllerName.Should().Be("Account");
        }


        [Test, MoqAutoData]
        public void Get_AccountDetails_StubFails_InProd(
           [Frozen] Mock<IConfiguration> configuration,
           [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("PRD");
            var result = controller.AccountDetails("returnUrl");
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test, MoqAutoData]
        public void Getting_AccountDetails_Claims_ShouldNotBeNull(
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] StubAuthenticationViewModel model,
            [Greedy] AccountController controller)
        {
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");

            var result = controller.AccountDetails("returnUrl") as ViewResult;
            result.Should().BeOfType(typeof(ViewResult));
            result.ViewName.Should().Be("AccountDetails");
        }

        [Test, MoqAutoData]
        public async Task StubSignedIn_Redirects_To_Terms(
            [Frozen] Mock<IConfiguration> configuration,
            [Greedy] AccountController controller)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
    
            // Create a mock session - mock the Set method (not SetString which is an extension method)
            var sessionMock = new Mock<ISession>();
    
            // Set up the Set method which SetString extension method calls internally
            sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => 
                {
                    // Optionally capture values if needed for assertions
                })
                .Verifiable();
    
            // Also mock IsAvailable to return true
            sessionMock.SetupGet(s => s.IsAvailable).Returns(true);
    
            httpContext.Session = sessionMock.Object;
    
            var emailClaim = new Claim(ClaimTypes.Email, "test@test.com");
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, "test");
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, Guid.NewGuid().ToString());
    
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                emailClaim,
                nameClaim,
                apprenticeIdClaim
            })});
    
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
    
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");
    
            // Act
            var result = await controller.StubSignedIn() as RedirectToActionResult;
    
            // Assert
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Terms");
    
            // Verify Set was called (which SetString calls)
            sessionMock.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()), Times.AtLeastOnce);
        }
        
        [Test, MoqAutoData]
        public async Task StubSignedIn_Fails_In_Prod(
            [Frozen] Mock<IConfiguration> configuration,
            [Greedy] AccountController controller)
        {
            var httpContext = new DefaultHttpContext();
            var emailClaim = new Claim(ClaimTypes.Email, "test@test.com");
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, "test");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                emailClaim,
                nameClaim
            })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("PRD");
            var result = await controller.StubSignedIn();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test, MoqAutoData]
        public void IsUserInNewUiCohort_ReturnsFalse_ForOtherProviderIds([Greedy] AccountController controller)
        {
            // Arrange & Act & Assert
            controller.IsUserInNewUiCohort(0).Should().BeFalse();
            controller.IsUserInNewUiCohort(6).Should().BeFalse();
            controller.IsUserInNewUiCohort(999).Should().BeFalse();
            controller.IsUserInNewUiCohort(-1).Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task AccountDetails_Post_WithApprenticeId_AddsNewUiEnabledClaim(
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] Mock<IStubAuthenticationService> authenticationService,
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] AccountController controller)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");
            
            var model = new StubAuthenticationViewModel { Email = "test@test.com" };
            var apprenticeId = Guid.NewGuid();
            
            // Mock authentication service to return claims with apprentice ID
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString()));
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            authenticationService.Setup(x => x.GetStubSignInClaims(model))
                .ReturnsAsync(claimsPrincipal);
            
            // Mock client to return apprentice details
            client.Setup(c => c.GetApprenticeDetails(It.Is<Guid>(id => id == apprenticeId)))
                .ReturnsAsync(new ApprenticeDetails
                {
                    MyApprenticeship = new MyApprenticeship
                    {
                        ApprenticeshipId = 123,
                        StandardUId = "456"
                    }
                });

            // Act
            var result = await controller.AccountDetails(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            // Verify that the NewUiEnabledClaim was added
            claimsIdentity.HasClaim(Constants.NewUiEnabledClaimKey, "true").Should().BeTrue();
        }
        
[Test, MoqAutoData]
public void OptInNewUi_SetsCookieAndSessionAndRedirectsToWelcome(
    [Frozen] Mock<ILogger<AccountController>> logger,
    [Frozen] Mock<IUrlHelper> urlHelper,
    [Greedy] AccountController controller)
{
    // Arrange
    var httpContextMock = new Mock<HttpContext>();
    var returnUrl = "/some-path";

    // Mock Request Cookies – ensure welcome cookie exists
    var requestCookiesMock = new Mock<IRequestCookieCollection>();
    requestCookiesMock.Setup(c => c[Constants.WelcomeSplashScreenCookieName]).Returns("1");
    httpContextMock.Setup(ctx => ctx.Request.Cookies).Returns(requestCookiesMock.Object);

    // Mock Response Cookies
    var responseMock = new Mock<HttpResponse>();
    var responseCookiesMock = new Mock<IResponseCookies>();
    responseMock.Setup(r => r.Cookies).Returns(responseCookiesMock.Object);
    httpContextMock.Setup(ctx => ctx.Response).Returns(responseMock.Object);

    // Mock Session
    var sessionMock = new Mock<ISession>();
    var sessionSetCalls = new List<(string key, byte[] value)>();
    var sessionRemoveCalls = new List<string>();
    sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
        .Callback<string, byte[]>((key, value) => sessionSetCalls.Add((key, value)));
    sessionMock.Setup(s => s.Remove(It.IsAny<string>()))
        .Callback<string>(key => sessionRemoveCalls.Add(key));
    httpContextMock.Setup(ctx => ctx.Session).Returns(sessionMock.Object);

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = httpContextMock.Object
    };
    controller.Url = urlHelper.Object;

    const string expectedCookieName = "SFA.ApprenticeApp.NewUiOptIn";

    // Act
    var result = controller.OptInNewUi(returnUrl) as RedirectToActionResult;

    // Assert
    using (new AssertionScope())
    {
        // Verify opt‑in cookie appended
        responseCookiesMock.Verify(x => x.Append(
            expectedCookieName,
            "true",
            It.Is<CookieOptions>(o =>
                o.HttpOnly == true &&
                o.Secure == true &&
                o.SameSite == SameSiteMode.Lax &&
                o.Path == "/" &&
                o.Expires.HasValue &&
                o.Expires.Value > DateTimeOffset.UtcNow.AddDays(364) &&
                o.Expires.Value < DateTimeOffset.UtcNow.AddDays(366)
            )), Times.Once);

        // Verify welcome cookie deleted (no options passed)
        responseCookiesMock.Verify(x => x.Delete(Constants.WelcomeSplashScreenCookieName), Times.Once);

        // Session: one Remove (ForceOldUI) and two Sets (OptInUser, UserType)
        sessionMock.Verify(s => s.Remove(It.IsAny<string>()), Times.Once);
        sessionMock.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Exactly(2));

        sessionRemoveCalls.Should().Contain("ForceOldUI");

        var optInUserCall = sessionSetCalls.FirstOrDefault(c => c.key == "OptInUser");
        optInUserCall.Should().NotBeNull();
        System.Text.Encoding.UTF8.GetString(optInUserCall.value).Should().Be("true");

        var userTypeCall = sessionSetCalls.FirstOrDefault(c => c.key == "UserType");
        userTypeCall.Should().NotBeNull();
        System.Text.Encoding.UTF8.GetString(userTypeCall.value).Should().Be("SpecialUser");

        logger.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("User opted into new UI via button.")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

        // Redirect to Welcome/Index
        result.Should().NotBeNull();
        result!.ActionName.Should().Be("Index");
        result.ControllerName.Should().Be("Welcome");
    }
}

[Test, MoqAutoData]
public void OptInNewUi_RedirectsToWelcome_EvenWhenReturnUrlIsInvalid(
    [Frozen] Mock<ILogger<AccountController>> logger,
    [Frozen] Mock<IUrlHelper> urlHelper,
    [Greedy] AccountController controller)
{
    // Arrange
    var httpContextMock = new Mock<HttpContext>();
    string returnUrl = null;   // null returnUrl

    var requestCookiesMock = new Mock<IRequestCookieCollection>();
    requestCookiesMock.Setup(c => c[Constants.WelcomeSplashScreenCookieName]).Returns("1");
    httpContextMock.Setup(ctx => ctx.Request.Cookies).Returns(requestCookiesMock.Object);

    var responseMock = new Mock<HttpResponse>();
    var responseCookiesMock = new Mock<IResponseCookies>();
    responseMock.Setup(r => r.Cookies).Returns(responseCookiesMock.Object);
    httpContextMock.Setup(ctx => ctx.Response).Returns(responseMock.Object);

    var sessionMock = new Mock<ISession>();
    var sessionSetCalls = new List<(string key, byte[] value)>();
    var sessionRemoveCalls = new List<string>();
    sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
        .Callback<string, byte[]>((key, value) => sessionSetCalls.Add((key, value)));
    sessionMock.Setup(s => s.Remove(It.IsAny<string>()))
        .Callback<string>(key => sessionRemoveCalls.Add(key));
    httpContextMock.Setup(ctx => ctx.Session).Returns(sessionMock.Object);

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = httpContextMock.Object
    };
    controller.Url = urlHelper.Object;

    // Act
    var result = controller.OptInNewUi(returnUrl) as RedirectToActionResult;

    // Assert
    using (new AssertionScope())
    {
        responseCookiesMock.Verify(x => x.Append(It.IsAny<string>(), "true", It.IsAny<CookieOptions>()), Times.Once);
        responseCookiesMock.Verify(x => x.Delete(Constants.WelcomeSplashScreenCookieName), Times.Once);

        sessionMock.Verify(s => s.Remove(It.IsAny<string>()), Times.Once);
        sessionMock.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Exactly(2));

        sessionRemoveCalls.Should().Contain("ForceOldUI");

        var optInUserCall = sessionSetCalls.FirstOrDefault(c => c.key == "OptInUser");
        optInUserCall.Should().NotBeNull();
        System.Text.Encoding.UTF8.GetString(optInUserCall.value).Should().Be("true");

        var userTypeCall = sessionSetCalls.FirstOrDefault(c => c.key == "UserType");
        userTypeCall.Should().NotBeNull();
        System.Text.Encoding.UTF8.GetString(userTypeCall.value).Should().Be("SpecialUser");

        result.Should().NotBeNull();
        result!.ActionName.Should().Be("Index");
        result.ControllerName.Should().Be("Welcome");
    }
}        

    }
}