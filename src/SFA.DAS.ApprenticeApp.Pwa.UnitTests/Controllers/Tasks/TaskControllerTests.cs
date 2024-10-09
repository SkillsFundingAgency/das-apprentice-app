using AutoFixture.NUnit3;
using Azure.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
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
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
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
            httpContext.Response.Cookies.Append(Constants.TaskFiltersCookieName, "TaskFiltersCookieName");
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
        public async Task Edit_Returns_View_NoApprenticeId([Greedy] TasksController controller)
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
            var result = await controller.Edit(1) as RedirectToActionResult;
            result.ActionName.Should().Be("Index");
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

            var result = await controller.Add(task);
           
            result.Should().BeOfType(typeof(UnauthorizedResult));           
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

            result.Should().BeOfType(typeof(UnauthorizedResult));
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
            [Frozen] Mock<ILogger<TasksController>> logger, [Greedy] TasksController controller)
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
            var result = await controller.ChangeTaskStatus(1, 0);

            result.Should().BeOfType(typeof(UnauthorizedResult));
        }

        [Test, MoqAutoData]
        public async Task NoTasks_ReturnsView(
               [Greedy] TasksController controller)
        {
            var result = controller.TasksNotStarted() as ViewResult;
            result.Should().NotBeNull();
        }
    }
}