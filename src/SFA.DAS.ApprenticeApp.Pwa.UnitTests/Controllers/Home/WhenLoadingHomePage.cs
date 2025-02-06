using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class WhenLoadingHomePage
    {
        [Test, MoqAutoData]
        public async Task Then_The_Homepage_Is_Loaded([Greedy] HomeController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.Index() as ActionResult;
            result.Should().NotBeNull();
        }


        [Test, MoqAutoData]
        public async Task Redirect_If_logged_in(
                     [Frozen] ApplicationConfiguration configuration,
                    [Frozen] Mock<IOuterApiClient> client,
                    [Frozen] ApprenticeDetails apprenticeDetails,
                    [Greedy] HomeController controller,
                    [Frozen] Mock<IRequestCookieCollection> cookies)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString(), ClaimValueTypes.String),
                new Claim(Constants.ApprenticeNameClaimKey, "user1@test.com", ClaimValueTypes.String)
            }, "Custom");

            cookies.Setup(c => c[Constants.CookieTrackCookieName]).Returns("1");
            httpContext.Request.Cookies = cookies.Object;            
            httpContext.User = new ClaimsPrincipal(identity);
            apprenticeDetails.MyApprenticeship = new MyApprenticeship();
            client.Setup(c => c.GetApprenticeDetails(apprenticeId)).ReturnsAsync(apprenticeDetails);

            var result = await controller.Index() as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
        }

            [Test, MoqAutoData]
            public async Task Redirect_If_Authenticated([Greedy] HomeController controller)
            {
                var httpContext = new DefaultHttpContext();
                var apprenticeId = Guid.NewGuid();
                var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
                var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            apprenticeIdClaim
        })});
                httpContext.User = claimsPrincipal;
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                };

                var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("UserId", "123", ClaimValueTypes.Integer32)
            }, "Custom");

                httpContext.User = new ClaimsPrincipal(identity);

                if (httpContext.User.Identity.IsAuthenticated)
                {
                    var result = await controller.Index() as ActionResult;
                    result.Should().NotBeNull();
                }
            }
            
            [Test, MoqAutoData]
            public async Task Then_cookie_start_screen_loads([Greedy] HomeController controller, [Frozen] Mock<IRequestCookieCollection> cookies)
            {
                
                var httpContext = new DefaultHttpContext();
                var apprenticeId = Guid.NewGuid();
                var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
                var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
                {
                    apprenticeIdClaim
                })});
                httpContext.User = claimsPrincipal;
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                };

                cookies.Setup(c => c[Constants.CookieTrackCookieName]).Returns("1");
                httpContext.Request.Cookies = cookies.Object;
                httpContext.User = claimsPrincipal;
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                };

                var result = await controller.CookieStart() as ActionResult;
                result.Should().NotBeNull();
            }
            
            [Test, MoqAutoData]
            public async Task Then_cookie_AcceptCookies_redirect([Greedy] HomeController controller)
            {
                var httpContext = new DefaultHttpContext();
                var apprenticeId = Guid.NewGuid();
                var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
                var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
                {
                    apprenticeIdClaim
                })});
                httpContext.User = claimsPrincipal;
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                };

                var result = await controller.AcceptCookies() as ActionResult;
                result.Should().NotBeNull();
            }                 
            
            [Test, MoqAutoData]
            public async Task Then_cookie_DeclineCookies_redirect([Greedy] HomeController controller)
            {
                var httpContext = new DefaultHttpContext();
                var apprenticeId = Guid.NewGuid();
                var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
                var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
                {
                    apprenticeIdClaim
                })});
                httpContext.User = claimsPrincipal;
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                };

                var result = await controller.DeclineCookies() as ActionResult;
                result.Should().NotBeNull();
            }             
            
        }
    }