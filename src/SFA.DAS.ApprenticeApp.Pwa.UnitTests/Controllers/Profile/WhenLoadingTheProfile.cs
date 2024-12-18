﻿using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Profile
{
    public class WhenLoadingTheProfile
    {
        [Test, MoqAutoData]
        public async Task Then_The_Profile_Details_Are_Displayed_For_A_Valid_Apprentice(
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] ProfileController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var termsAccepted = "True";
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var termsAcceptedClaim = new Claim(Constants.TermsAcceptedClaimKey, termsAccepted);
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                termsAcceptedClaim
            })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            client.Setup(c => c.GetApprenticeDetails(It.IsAny<Guid>()))
                .ReturnsAsync(new ApprenticeDetails
                {
                    Apprentice = new Apprentice(),
                    MyApprenticeship = new MyApprenticeship()
                });
            var result = await controller.Index() as ViewResult;
            result.Model.Should().BeOfType(typeof(ProfileViewModel));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Home_Page_Is_Displayed_For_Invalid_Apprentice(
            [Frozen] Mock<IOuterApiClient> client,
            ApprenticeDetails apprenticeDetails,
            [Frozen] Mock<ILogger<ProfileController>> logger,
            [Greedy] ProfileController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, "");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.Index() as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                            v.ToString().Contains($"ApprenticeId not found in user claims for Profile Index")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_The_Home_Page_Is_Displayed_For_Unauthorised_Apprentice(
            [Frozen] Mock<ILogger<ProfileController>> logger,
            [Greedy] ProfileController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, "");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
                apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var actual = await controller.Index() as RedirectToActionResult;

            using (new AssertionScope())
            {

                logger.Verify(x => x.Log(LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                            v.ToString().Contains($"ApprenticeId not found in user claims for Profile Index")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                actual.ActionName.Should().Be("Index");
                actual.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_AddSubscription_Is_Called_For_Valid_Apprentice(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] ApprenticeAddSubscriptionRequest request,
            [Greedy] ProfileController controller)
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

            var result = await controller.AddSubscription(request);

            client.Verify(x => x.ApprenticeAddSubscription(It.IsAny<Guid>(), It.IsAny<ApprenticeAddSubscriptionRequest>()), Times.Once);
            var redirectResult = result as RedirectToActionResult;

            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.ActionName.Should().Be("Index");
                redirectResult.ControllerName.Should().Be("Profile");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_RemoveSubscription_Is_Called_For_Valid_Apprentice(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] ApprenticeRemoveSubscriptionRequest request,
            [Greedy] ProfileController controller)
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

            var result = await controller.RemoveSubscription(request);

            client.Verify(x => x.ApprenticeRemoveSubscription(It.IsAny<Guid>(), It.IsAny<ApprenticeRemoveSubscriptionRequest>()), Times.Once);
            var redirectResult = result as RedirectToActionResult;

            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.ActionName.Should().Be("Index");
                redirectResult.ControllerName.Should().Be("Profile");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_AddSubscriptionFails_When_Not_Logged_In(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<ILogger<ProfileController>> logger,
            [Frozen] ApprenticeAddSubscriptionRequest request,
            [Greedy] ProfileController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, "");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
                apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.AddSubscription(request) as RedirectToActionResult; ;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains("ApprenticeId not found in user claims for Profile Index")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Profile");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_AddSubscriptionFails_When_No_Endpoint(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<ILogger<ProfileController>> logger,
            [Frozen] ApprenticeAddSubscriptionRequest request,
            [Greedy] ProfileController controller)
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

            request.Endpoint = null;
            var result = await controller.AddSubscription(request) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains("Endpoint not found in subscription request.")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Profile");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_RemoveSubscriptionFails_When_Not_Logged_In(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<ILogger<ProfileController>> logger,
            [Frozen] ApprenticeRemoveSubscriptionRequest request,
            [Greedy] ProfileController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, "");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
                apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.RemoveSubscription(request) as RedirectToActionResult; ;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains("ApprenticeId not found in user claims for Profile Index")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Profile");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_RemoveSubscriptionFails_When_No_Endpoint(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<ILogger<ProfileController>> logger,
            [Frozen] ApprenticeRemoveSubscriptionRequest request,
            [Greedy] ProfileController controller)
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

            request.Endpoint = "";
            var result = await controller.RemoveSubscription(request) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains("Endpoint not found in remove subscription request for apprentice")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Profile");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_AddSubscription_LogsWarning_When_ModelState_Invalid
            ([Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<ILogger<ProfileController>> logger,
            [Frozen] ApprenticeAddSubscriptionRequest request,
            [Greedy] ProfileController controller)
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

            controller.ModelState.AddModelError("key", "error");
            var result = await controller.AddSubscription(request) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains("ProfileController: ModelState is not valid in AddSubscription.")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Profile");
            }
        }

        [Test, MoqAutoData]
        public async Task Then_RemoveSubscription_LogsWarning_When_ModelStatus_Invalid(
                [Frozen] Mock<IOuterApiClient> client,
                [Frozen] Mock<ILogger<ProfileController>> logger,
                [Frozen] ApprenticeRemoveSubscriptionRequest request,
                [Greedy] ProfileController controller)
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

            controller.ModelState.AddModelError("key", "error");
            var result = await controller.RemoveSubscription(request) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains("ProfileController: ModelState is not valid in RemoveSubscription.")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Profile");
            }
        }


    }
}