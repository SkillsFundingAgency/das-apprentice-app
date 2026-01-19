using AutoFixture.NUnit3;
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
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Notifications
{
    [TestFixture]
    public class NotificationsControllerTests
    {
        [Test, MoqAutoData]
        public async Task Redirect_To_Home_NoApprenticeId(
    [Frozen] Mock<ILogger<NotificationsController>> logger,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] NotificationsController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns(string.Empty); // ← no apprentice id

            // Act
            var result = await controller.Index() as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                        v.ToString()!.Contains(
                            "ApprenticeId not found in user claims for Notifications Index.")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ));

                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task NotificationResult_ReturnsView(
            List<ApprenticeTaskReminder> taskReminderResult,
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] NotificationsController controller
            )
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var apprenticeId = new Guid();
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

            client.Setup(x => x.GetTaskReminderNotifications(apprenticeId)).ReturnsAsync(new ApprenticeTaskRemindersCollection
            {
                TaskReminders = taskReminderResult
            });

            //Act
            var result = await controller.Index() as ViewResult;
            
            //Assert
            result.Should().NotBe(null);
        }

        [Test, MoqAutoData]
        public async Task NotificationResult_NoResult_LoadsHomePage(
    [Frozen] Mock<ILogger<NotificationsController>> logger,
    [Frozen] Mock<IOuterApiClient> client,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] NotificationsController controller)
        {
            // Arrange
            var apprenticeId = Guid.NewGuid();

            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns(apprenticeId.ToString()); // ← valid apprentice

            client
                .Setup(x => x.GetTaskReminderNotifications(apprenticeId))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.Index() as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                        v.ToString()!.Contains(
                            "Error in Notifications: GetTaskReminderNotifications")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ));

                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_ReturnsRedirectToIndex(
            int taskId,
            [Frozen] Mock<ILogger<NotificationsController>> logger,
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] NotificationsController controller
            )
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var apprenticeId = new Guid();
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

            //Act
            var result = await controller.DeleteNotification(taskId) as RedirectToActionResult;

            //Assert
            using (new AssertionScope())
            {
               logger.Verify(logger => logger.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Updating reminder notification for {taskId} to dismissed")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
            }
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_ExceptionCaught(
    int taskId,
    [Frozen] Mock<ILogger<NotificationsController>> logger,
    [Frozen] Mock<IOuterApiClient> client,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] NotificationsController controller)
        {
            // Arrange
            var apprenticeId = Guid.NewGuid();

            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns(apprenticeId.ToString()); // ← valid apprentice

            client
                .Setup(x => x.UpdateTaskReminderStatus(
                    apprenticeId,
                    taskId,
                    (int)ReminderStatus.Dismissed))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.DeleteNotification(taskId) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                        v.ToString()!.Contains(
                            "Error in Notifications: DeleteTaskReminderNotification")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ));

                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
            }
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_NoApprenticeId(
    int taskId,
    [Frozen] Mock<ILogger<NotificationsController>> logger,
    [Frozen] Mock<IOuterApiClient> client,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] NotificationsController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns((string?)null); // ← no apprentice ID

            // Act
            var result = await controller.DeleteNotification(taskId) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((object v, Type _) =>
                        v.ToString()!.Contains(
                            "ApprenticeId not found in user claims for Notifications DeleteNotification.")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ));

                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
            }
        }

        [Test, MoqAutoData]
        public void NoNotifications_Returns_Partial(
            [Greedy] NotificationsController controller)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var apprenticeId = new Guid();
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

            //Act
            var result = controller.NoNotifications() as PartialViewResult;

            //Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("_NoNotifications");
        }
    }
}