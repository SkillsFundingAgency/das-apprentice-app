using AutoFixture.NUnit3;
using Azure.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Tasks
{
    public class TaskControllerTests
    {
        [Test, MoqAutoData]
        public async Task LoadToDoTasks(
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

            var result = await controller.ToDoTasks();

            result.Should().BeOfType(typeof(PartialViewResult));
            result.ViewName.Should().Be("_TasksToDo");
        }

        [Test, MoqAutoData]
        public async Task LoadToDoTasksWithSortingByDueDate(
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

            httpContext.Response.Cookies.Append(Constants.TaskFilterSortCookieName, "due_date");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.ToDoTasks();

            result.Should().BeOfType(typeof(PartialViewResult));
            result.ViewName.Should().Be("_TasksToDo");
        }

        [Test, MoqAutoData]
        public async Task TodoTasksWithCookieSet(
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim
            })});
            httpContext.User = claimsPrincipal;

            httpContext.Response.Cookies.Append(Constants.TaskFilterYearCookieName, "2024");
            httpContext.Response.Cookies.Append(Constants.TaskFiltersDoneCookieName, "TaskFiltersDoneCookieName");
            httpContext.Response.Cookies.Append(Constants.TaskFilterSortCookieName, "sortorder");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.ToDoTasks();

            result.Should().BeOfType(typeof(PartialViewResult));
            result.ViewName.Should().Be("_TasksToDo");
        }

        [Test, MoqAutoData]
        public async Task DoneTasksWithYearCookieSet(
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim
            })});
            httpContext.User = claimsPrincipal;

            httpContext.Request.Cookies = MockRequestCookieCollection(Constants.TaskFilterYearCookieName, "2024");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.DoneTasks() as ActionResult;
            Assert.IsNotNull(result);
        }

        [Test, MoqAutoData]
        public async Task DoneTasksWithSortCookieSet(
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim
            })});
            httpContext.User = claimsPrincipal;

            httpContext.Request.Cookies = MockRequestCookieCollection(Constants.TaskFilterSortCookieName, "sortorder");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.DoneTasks() as ActionResult;
            Assert.IsNotNull(result);
        }

        [Test, MoqAutoData]
        public async Task TodoTasksWithYearCookieSet(
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim
            })});
            httpContext.User = claimsPrincipal;

            httpContext.Request.Cookies = MockRequestCookieCollection(Constants.TaskFilterYearCookieName, "2024");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.ToDoTasks() as ActionResult;
            Assert.IsNotNull(result);
        }

        [Test, MoqAutoData]
        public async Task TodoTasksWithSortCookieSet(
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim
            })});
            httpContext.User = claimsPrincipal;

            httpContext.Request.Cookies = MockRequestCookieCollection(Constants.TaskFilterSortCookieName, "sortorder");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.ToDoTasks() as ActionResult;
            Assert.IsNotNull(result);
        }

        private static IRequestCookieCollection MockRequestCookieCollection(string key, string value)
        {
            var requestFeature = new HttpRequestFeature();
            var featureCollection = new FeatureCollection();

            requestFeature.Headers = new HeaderDictionary();
            requestFeature.Headers.Add(HeaderNames.Cookie, new StringValues(key + "=" + value));

            featureCollection.Set<IHttpRequestFeature>(requestFeature);

            var cookiesFeature = new RequestCookiesFeature(featureCollection);

            return cookiesFeature.Cookies;
        }

        [Test, MoqAutoData]
        public async Task LoadToDoTasksWithSortingByRecentlyAdded(
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim
            })});
            httpContext.User = claimsPrincipal;

            httpContext.Response.Cookies.Append(Constants.TaskFilterSortCookieName, "recently_added");

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
        public async Task LoadToDoTasks_NoApprenticeId([Frozen] ApprenticeTasksCollection taskResult,
           [Greedy] TasksController controller)
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
        public async Task Edit_Returns_View_NoApprenticeId(
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] TasksController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns((string?)null); // ← no apprentice ID

            // Act
            var result = await controller.Edit(1) as RedirectToActionResult;

            // Assert
            result!.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Tasks");
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
            result.ActionName.Should().Be("Index");
            result.RouteValues["status"].Should().Be(0);
        }

        [Test, MoqAutoData]
        public async Task EditTask_HandlesError(
           [Frozen] Mock<IOuterApiClient> client,
           ApprenticeTask task,
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

            client.Setup(x => x.UpdateApprenticeTask(apprenticeId, task.TaskId, task)).ThrowsAsync(new Exception("Error"));
            var result = await controller.Edit(task) as RedirectToActionResult;
            result.Should().BeOfType(typeof(RedirectToActionResult));
            result.ActionName.Should().Be("Index");
            result.RouteValues["status"].Should().Be(0);
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
        public async Task CloseTask_AfterSave(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<ILogger<TasksController>> logger,
            [Frozen] ApprenticeTask task,
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            long apprenticeshipId = 123;
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim
            })});
            httpContext.User = claimsPrincipal;

            var formContext = new FormCollection(new Dictionary<string, StringValues> { { "time", "03:03" } });
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = formContext;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(client => client.GetApprenticeDetails(It.IsAny<Guid>())).ReturnsAsync(new ApprenticeDetails() { MyApprenticeship = new MyApprenticeship() { ApprenticeshipId = 123} });

            task.ApprenticeshipId = apprenticeshipId;
            var result = await controller.Add(task) as RedirectToActionResult;
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Adding new task for apprentice with id")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                logger.Verify(x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Task added successfully for apprentice with id")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.RouteValues["status"].Should().Be(0);
            }
        }

        [Test, MoqAutoData]
        public async Task AddTask_MustHaveValidId(
           [Frozen] Mock<IOuterApiClient> client,
           [Frozen] ApprenticeDetails apprenticeDetails,
           [Frozen] Mock<ILogger<TasksController>> logger,
           [Frozen] ApprenticeTask task,
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

            if (task.ApprenticeshipId == apprenticeDetails.MyApprenticeship.ApprenticeshipId)
            {
                task.ApprenticeshipId = task.ApprenticeshipId + 1;
            }

            var result = await controller.Add(task) as RedirectToActionResult;
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Invalid apprenticeship id. Cannot add task")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Tasks");
            }
        }

        [Test, MoqAutoData]
        public async Task AddTask_ReturnsView(
           [Frozen] Mock<IOuterApiClient> client,
           [Frozen] ApprenticeDetails apprenticeDetails,
           [Frozen] Mock<ILogger<TasksController>> logger,
           [Frozen] ApprenticeTask task,
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

            var formContext = new FormCollection(new Dictionary<string, StringValues> { { "time", "03:03" } });
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = formContext;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(client => client.GetApprenticeDetails(It.IsAny<Guid>())).ReturnsAsync(new ApprenticeDetails() { MyApprenticeship = new MyApprenticeship() { ApprenticeshipId = 123 } });

            task.ApprenticeshipId = 123;
            var result = await controller.Add(task) as RedirectToActionResult;

            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.RouteValues["status"].Should().Be((int)task.Status);

        }

        [Test, MoqAutoData]
        public async Task AddTask_HandlesError(
          [Frozen] Mock<IOuterApiClient> client,
          [Frozen] ApprenticeDetails apprenticeDetails,
          [Frozen] Mock<ILogger<TasksController>> logger,
          [Frozen] ApprenticeTask task,
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

            var formContext = new FormCollection(new Dictionary<string,StringValues> { { "time", "03:03" } });
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = formContext;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };


            client.Setup(c => c.GetApprenticeDetails(apprenticeId)).ReturnsAsync(apprenticeDetails);
            task.ApprenticeshipId = apprenticeDetails.MyApprenticeship.ApprenticeshipId;
            client.Setup(x => x.AddApprenticeTask(1, task)).ThrowsAsync(new Exception("Error"));
            var result = await controller.Add(task) as RedirectToActionResult;

            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.RouteValues["status"].Should().Be((int)task.Status);

        }

        [Test, MoqAutoData]
        public async Task Task_Index_load(
          [Frozen] Mock<IOuterApiClient> client,
          [Frozen] ApprenticeDetails apprenticeDetails,
          [Frozen] Mock<ILogger<TasksController>> logger,
          [Frozen] ApprenticeTask task,
          [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "1");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim
            })});
            httpContext.User = claimsPrincipal;

            httpContext.Response.Cookies.Append(Constants.TaskFilterYearCookieName, "2024");
            httpContext.Response.Cookies.Append(Constants.TaskFilterSortCookieName, "sort");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            string sortoder = "sort";
            int year = 2024;

            var result = controller.Index(sortoder, year);

            result.Should().NotBeNull();

        }

        [Test, MoqAutoData]
        public async Task AddTask_MustHave_ValidApprenticeId(
    [Frozen] ApprenticeTask task,
    [Frozen] Mock<IApprenticeContext> apprenticeContext,
    [Greedy] TasksController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns((string?)null); // ← invalid apprentice

            // Act
            var result = await controller.Add(task);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
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
            result.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task DeleteTask_Must_Have_ValidId(
            [Frozen] Mock<ILogger<TasksController>> logger,
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
        public async Task DeleteTask_Must_Have_ValidApprenticeId(
            [Greedy] TasksController controller)
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
            var result = await controller.DeleteApprenticeTask(1);

            result.Should().BeOfType(typeof(OkResult));
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
            [Frozen] Mock<IOuterApiClient> client,
            ApprenticeDetails apprenticeDetails,
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

            client.Setup(x => x.GetApprenticeDetails(apprenticeId));

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

        [Test, MoqAutoData]
        public async Task ChangeTaskStatus_Must_Have_ApprenticeId(
        [Frozen] Mock<IOuterApiClient> client,
        ApprenticeDetails apprenticeDetails,
        [Frozen] Mock<ILogger<TasksController>> logger,
        [Frozen] Mock<IApprenticeContext> apprenticeContext,
        [Greedy] TasksController controller)
        {
            // Arrange
            apprenticeContext
                .Setup(x => x.ApprenticeId)
                .Returns((string?)null); // ← no apprentice ID

            // Act
            var result = await controller.ChangeTaskStatus(1, 0);

            // Assert
            result.Should().BeOfType<OkResult>();
        }
        
        [Test, MoqAutoData]
        public async Task AddFromKsb_NoApprenticeId_ReturnsRedirectToIndexWhenTaskIdProvided(
            [Frozen] Mock<ILogger<TasksController>> logger,
            [Frozen] Mock<IApprenticeContext> apprenticeContext,
            [Greedy] TasksController controller)
        {
            apprenticeContext.Setup(x => x.ApprenticeId).Returns((string?)null);

            var result = await controller.AddFromKsb(linkedKsbGuids: "a,b", taskId: 123, statusId: 0) as RedirectToActionResult;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Tasks");
            }
        }

        [Test, MoqAutoData]
        public async Task AddFromKsb_NoApprenticeId_RedirectsToAddWhenTaskIdZero(
            [Frozen] Mock<ILogger<TasksController>> logger,
            [Frozen] Mock<IApprenticeContext> apprenticeContext,
            [Greedy] TasksController controller)
        {
            apprenticeContext.Setup(x => x.ApprenticeId).Returns((string?)null);

            var result = await controller.AddFromKsb(linkedKsbGuids: "a,b", taskId: 0, statusId: 0) as RedirectToActionResult;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Add");
            }
        }

        [Test, MoqAutoData]
        public async Task AddFromKsb_TaskIdGreaterThanZero_ReturnsEditViewWithCombinedKsbs(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<IApprenticeContext> apprenticeContext,
            [Greedy] TasksController controller)
        {
            var apprenticeId = Guid.NewGuid();
            apprenticeContext.Setup(a => a.ApprenticeId).Returns(apprenticeId.ToString());

            var existingGuid = Guid.NewGuid();
            var existingProgress = new ApprenticeKsbData
            {
                KsbId = existingGuid,
                KsbKey = "EX",
                Detail = "Existing",
                CurrentStatus = KSBStatus.InProgress
            };

            var taskData = new ApprenticeTaskData
            {
                Task = new ApprenticeTask { TaskId = 10 },
                TaskCategories = null,
                KsbProgress = new List<ApprenticeKsbData> { existingProgress }
            };

            var newGuid = Guid.NewGuid();
            var apiKsbs = new List<ApprenticeKsb>
            {
                new ApprenticeKsb { Id = newGuid, Key = "NEW", Detail = "New detail", Type = KsbType.Skill, Progress = null }
            };

            client.Setup(c => c.GetTaskViewData(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(taskData);
            client.Setup(c => c.GetApprenticeshipKsbs(It.IsAny<Guid>())).ReturnsAsync(apiKsbs);
            apprenticeContext.Setup(a => a.ApprenticeId).Returns(apprenticeId.ToString());

            var linked = $"{existingGuid},{newGuid}";

            var result = await controller.AddFromKsb(linkedKsbGuids: linked, taskId: taskData.Task.TaskId, statusId: 1) as ViewResult;

            result.Should().NotBeNull();
            result!.ViewName.Should().Be("Edit");
            result.Model.Should().BeOfType<EditTaskPageModel>();
            var vm = (EditTaskPageModel)result.Model;
            vm.LinkedKsbGuids.Should().Be(linked);
            vm.KsbProgressData.Should().NotBeNull();
            vm.KsbProgressData.Count.Should().Be(2);
            vm.KsbProgressData.Select(k => k.KsbId).Should().Contain(existingGuid);
            vm.KsbProgressData.Select(k => k.KsbId).Should().Contain(newGuid);
            vm.Task.TaskId.Should().Be(taskData.Task.TaskId);
        }

        [Test, MoqAutoData]
        public async Task AddFromKsb_TaskIdZero_TempDataMissing_RedirectsToAdd(
            [Frozen] Mock<IApprenticeContext> apprenticeContext,
            [Greedy] TasksController controller)
        {
            var apprenticeId = Guid.NewGuid();
            apprenticeContext.Setup(a => a.ApprenticeId).Returns(apprenticeId.ToString());

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await controller.AddFromKsb(linkedKsbGuids: "abc", taskId: 0, statusId: 0) as RedirectToActionResult;

            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Add");
        }

        [Test, MoqAutoData]
        public async Task AddFromKsb_TaskIdZero_WithTempData_ReturnsAddViewWithMergedKsbsAndCategories(
            [Frozen] Mock<IOuterApiClient> client,
            [Frozen] Mock<IApprenticeContext> apprenticeContext,
            [Greedy] TasksController controller)
        {
            var apprenticeId = Guid.NewGuid();
            apprenticeContext.Setup(a => a.ApprenticeId).Returns(apprenticeId.ToString());

            var existingGuid = Guid.NewGuid();
            var tempModel = new EditTaskPageModel
            {
                Task = new ApprenticeTask { TaskId = 0 },
                Categories = null,
                KsbProgressData = new List<ApprenticeKsbData>
                {
                    new ApprenticeKsbData { KsbId = existingGuid, KsbKey = "EX", Detail = "Existing" }
                },
                LinkedKsbGuids = string.Empty,
                StatusId = 0
            };

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData.Put("TempAddTask", tempModel);

            var newGuid = Guid.NewGuid();
            var apiKsbs = new List<ApprenticeKsb>
            {
                new ApprenticeKsb { Id = newGuid, Key = "NEW", Detail = "New", Type = KsbType.Knowledge, Progress = null }
            };
            client.Setup(c => c.GetApprenticeshipKsbs(It.IsAny<Guid>())).ReturnsAsync(apiKsbs);

            var categoriesResponse = new ApprenticeTaskCategoryCollection { TaskCategories = new List<ApprenticeshipCategory> { new ApprenticeshipCategory { CategoryId = 5, Title = "Cat" } } };
            client.Setup(c => c.GetTaskCategories(It.IsAny<Guid>())).ReturnsAsync(categoriesResponse);

            var linked = $"{existingGuid},{newGuid}";

            var result = await controller.AddFromKsb(linkedKsbGuids: linked, taskId: 0, statusId: 2) as ViewResult;

            result.Should().NotBeNull();
            result!.ViewName.Should().Be("Add");
            result.Model.Should().BeOfType<EditTaskPageModel>();
            var vm = (EditTaskPageModel)result.Model;
            vm.LinkedKsbGuids.Should().Be(linked);
            vm.KsbProgressData.Should().NotBeNull();
            vm.KsbProgressData.Select(k => k.KsbId).Should().Contain(existingGuid);
            vm.KsbProgressData.Select(k => k.KsbId).Should().Contain(newGuid);
            vm.Categories.Should().NotBeNull();
            vm.Categories.Any(c => c.CategoryId == 5).Should().BeTrue();
        }

        [Test, MoqAutoData]
        public void SaveTaskAndRedirectToLinkKsbs_PopulatesTempDataAndRedirects(
        [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var form = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["title"] = "My title",
                ["note"] = "Some note",
                ["Apprenticeshipid"] = "12345",
                ["duedate-day"] = "15",
                ["duedate-month"] = "4",
                ["duedate-year"] = "2026",
                ["time"] = "09:30",
                ["ReminderValue"] = "60",
                ["ksbslinked"] = "11111111-1111-1111-1111-111111111111,22222222-2222-2222-2222-222222222222",
                ["statusId"] = "2",
                ["taskId"] = "0"
            };
            httpContext.Request.Form = new FormCollection(form);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var result = controller.SaveTaskAndRedirectToLinkKsbs() as RedirectToActionResult;

            result.Should().NotBeNull();
            result!.ActionName.Should().Be("LinkKsbsToTask");
            result.ControllerName.Should().Be("Ksb");

            result.RouteValues.Should().ContainKey("taskId");
            result.RouteValues["linkedKsbGuids"].Should().Be("11111111-1111-1111-1111-111111111111,22222222-2222-2222-2222-222222222222");
            result.RouteValues["statusId"].Should().Be(2);

            var saved = controller.TempData.Get<EditTaskPageModel>("TempAddTask");
            saved.Should().NotBeNull();
            saved!.Task?.Title.Should().Be("My title");
            saved.Task?.Note.Should().Be("Some note");
            saved.Task?.ApprenticeshipId.Should().Be(12345);
            saved.Task?.DueDate.Should().NotBeNull();
            saved.Task?.DueDate.Value.ToString("yyyy-MM-dd HH:mm").Should().Be("2026-04-15 09:30");
            saved.Task?.ReminderValue.Should().Be(60);
            saved.LinkedKsbGuids.Should().Be("11111111-1111-1111-1111-111111111111,22222222-2222-2222-2222-222222222222");
            saved.StatusId.Should().Be(2);
        }

        [Test, MoqAutoData]
        public void SaveTaskAndRedirectToLinkKsbs_HandlesInvalidDateAndDefaults(
            [Greedy] TasksController controller)
        {
            var httpContext = new DefaultHttpContext();
            var form = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["title"] = "Another title",
                ["note"] = "",
                ["Apprenticeshipid"] = "999",
                ["duedate-day"] = "aa",
                ["duedate-month"] = "bb",
                ["duedate-year"] = "cccc",
                ["time"] = "not-a-time",
                ["ksbslinked"] = "",
                ["statusId"] = "invalid"
            };
            httpContext.Request.Form = new FormCollection(form);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var result = controller.SaveTaskAndRedirectToLinkKsbs() as RedirectToActionResult;

            result.Should().NotBeNull();
            result!.ActionName.Should().Be("LinkKsbsToTask");
            result.ControllerName.Should().Be("Ksb");
            result.RouteValues?["linkedKsbGuids"].Should().Be(string.Empty);
            result.RouteValues?["statusId"].Should().Be(0);

            var saved = controller.TempData.Get<EditTaskPageModel>("TempAddTask");
            saved.Should().NotBeNull();
            saved.Task?.Title.Should().Be("Another title");
            saved.Task?.ApprenticeshipId.Should().Be(999);
            saved.Task?.DueDate.Should().BeNull();
            saved.StatusId.Should().Be(0);
        }
    }
}