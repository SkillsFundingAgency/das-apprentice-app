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
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Tasks
{
    public class TaskControllerTests
    {
        [Test, MoqAutoData]
        public async Task LoadToDoTasks([Greedy] TasksController controller)
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

            var result = await controller.ToDoTasks();
            result.Should().BeOfType(typeof(PartialViewResult));
            result.ViewName.Should().Be("_TasksToDo");
        }

        [Test, MoqAutoData]
        public async Task LoadToDoTasks_NoTasks([Frozen] ApprenticeTasksCollection taskResult,
            [Greedy] TasksController controller)
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

            taskResult.Tasks.Clear();
            var result = await controller.ToDoTasks();
            result.Should().BeOfType(typeof(PartialViewResult));
            result.ViewName.Should().Be("_TasksNotStarted");
        }

        [Test, MoqAutoData]
        public async Task LoadDoneTasks([Greedy] TasksController controller)
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

            var result = await controller.DoneTasks();
            result.Should().BeOfType(typeof(PartialViewResult));
            result.ViewName.Should().Be("_TasksDone");
        }

        [Test, MoqAutoData]
        public async Task LoadDoneTasks_NoTasks([Frozen] ApprenticeTasksCollection taskResult,
           [Greedy] TasksController controller)
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

            taskResult.Tasks.Clear();
            var result = await controller.DoneTasks();
            result.Should().BeOfType(typeof(PartialViewResult));
            result.ViewName.Should().Be("_TasksNotStarted");
        }

        [Test, MoqAutoData]
        public async Task Edit_Returns_View([Greedy] TasksController controller)
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
            var result = await controller.Edit(1);
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task EditTask_After_Updated([Frozen] ApprenticeTask task,
            [Greedy] TasksController controller)
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
            var result = await controller.Edit(task) as RedirectToActionResult;
            result.Should().BeOfType(typeof(RedirectToActionResult));
            result.ActionName.Should().Be("Edit");
            result.ControllerName.Should().Be("Tasks");
        }

        [Test, MoqAutoData]
        public async Task Add_Returns_View([Greedy] TasksController controller)
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
            var result = await controller.Add();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task CloseTask_AfterSave([Frozen] ApprenticeTask task,
            [Greedy] TasksController controller)
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
            var result = await controller.Add(task) as RedirectToActionResult;
            result.Should().BeOfType(typeof(RedirectToActionResult));
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public async Task ListTasks_After_TaskDelete([Greedy] TasksController controller)
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
            var result = await controller.DeleteApprenticeTask(1) as RedirectToActionResult;
            result.Should().BeOfType(typeof(RedirectToActionResult));
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public async Task DeleteTask_Must_Have_ValidId(
            [Frozen] Mock<ILogger<TasksController>> logger, [Greedy] TasksController controller)
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
            var result = await controller.DeleteApprenticeTask(0) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Task Id cannot be null or zero. Cannot delete task.")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Tasks");
            }
        }

        [Test, MoqAutoData]
        public async Task ListTasks_After_ChangeTaskStatus([Greedy] TasksController controller)
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
            var result = await controller.ChangeTaskStatus(1,1) as RedirectToActionResult;
            result.Should().BeOfType(typeof(RedirectToActionResult));
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public async Task ChangeTaskStatus_Must_Have_ValidId(
             [Frozen] Mock<ILogger<TasksController>> logger, [Greedy] TasksController controller)
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
            var result = await controller.ChangeTaskStatus(0,0) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Task Id cannot be null or zero. Cannot change task status.")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Tasks");
            }
        }


    }
}
