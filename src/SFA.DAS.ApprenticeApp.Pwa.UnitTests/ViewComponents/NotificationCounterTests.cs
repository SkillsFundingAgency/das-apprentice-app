using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewComponents;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.ViewComponents
{
    [TestFixture]
    public class NotificationCounterTests
    {
        [Test, MoqAutoData]
        public async Task Invoke_Async_Returns_View_With_Model(
            [Frozen] Guid apprenticeId,
            [Frozen] List<ApprenticeTaskReminder> taskReminders,
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] NotificationCounter viewComponent)
        {
            taskReminders.All(x => x.ApprenticeAccountId == apprenticeId);
            taskReminders[0].ReminderStatus = ReminderStatus.Sent;

            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
                apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;
            

            client.Setup(x => x.GetTaskReminderNotifications(apprenticeId))
                .ReturnsAsync(new ApprenticeTaskRemindersCollection { TaskReminders = taskReminders });

            viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext
                {
                    HttpContext = httpContext
                }
            };

            var result = await viewComponent.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.That(result, Is.InstanceOf<ViewViewComponentResult>());
        }

        [Test, MoqAutoData]
        public async Task Invoke_Async_Returns_Null_Invalid_Apprentice(
            [Frozen] List<ApprenticeTaskReminder> taskReminders,
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] NotificationCounter viewComponent)
        {
            //Arrane
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, "");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
                apprenticeIdClaim
        })});
            httpContext.User = claimsPrincipal;

            viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext
                {
                    HttpContext = httpContext
                }
            };

            //Act
            var result = await viewComponent.InvokeAsync() as ViewComponentResult;

            //Assert
            Assert.AreEqual(null, result);
        }
    }
}
