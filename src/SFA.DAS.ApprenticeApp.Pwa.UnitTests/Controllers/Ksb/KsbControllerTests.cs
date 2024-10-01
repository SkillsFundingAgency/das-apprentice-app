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
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Ksb
{
    public class KsbControllerTests
    {
        [Test, MoqAutoData]
        public async Task LoadIndex([Greedy] KsbController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var standardUIdClaim = new Claim(Constants.StandardUIdClaimKey, "ST001");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim,
                standardUIdClaim
            })});
            httpContext.User = claimsPrincipal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.Index();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task LoadIndex_NoApprenticeship(
            [Frozen] Mock<IOuterApiClient> client,
            ApprenticeDetails apprenticeDetails,
            [Frozen] Mock<ILogger<KsbController>> logger,
            [Greedy] KsbController controller)
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

            client.Setup(client => client.GetApprenticeDetails(It.IsAny<Guid>())).ReturnsAsync((ApprenticeDetails)null);

            var result = await controller.Index() as RedirectToActionResult;
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Apprentice Details not found - 'apprenticeDetails' is null in Ksbs Index.")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task LoadIndex_NoApprenticeId(
           ApprenticeDetails apprenticeDetails,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Greedy] KsbController controller)
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
                           v.ToString().Contains($"ApprenticeId not found in user claims for Ksbs Index")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task LoadLinkKsbs([Greedy] KsbController controller)
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeId = Guid.NewGuid();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, apprenticeId.ToString());
            var apprenticeshipIdClaim = new Claim(Constants.ApprenticeshipIdClaimKey, "123");
            var standardUIdClaim = new Claim(Constants.StandardUIdClaimKey, "ST001");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                apprenticeshipIdClaim,
                standardUIdClaim
            })});
            httpContext.User = claimsPrincipal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.LinkKsbs();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task LoadLinkKsbs_NoApprenticeship(
            [Frozen] Mock<IOuterApiClient> client,
            ApprenticeDetails apprenticeDetails,
            [Frozen] Mock<ILogger<KsbController>> logger,
            [Greedy] KsbController controller)
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

            client.Setup(client => client.GetApprenticeDetails(It.IsAny<Guid>())).ReturnsAsync((ApprenticeDetails)null);

            var result = await controller.LinkKsbs() as RedirectToActionResult;
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Apprentice Details not found - 'apprenticeDetails' is null in Ksbs LinkKsbs")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task LoadLinkKsbs_NoApprenticeId(
           ApprenticeDetails apprenticeDetails,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Greedy] KsbController controller)
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

            var result = await controller.LinkKsbs() as RedirectToActionResult;
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"ApprenticeId not found in user claims for Ksbs LinkKsbs")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Home");
            }
        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_Async([Greedy] KsbController controller)
        { 
            Guid id = Guid.NewGuid();

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

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.AddUpdateKsbProgress(id);
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_Get_ExistingProgress(
            List<ApprenticeKsbProgressData> ksbProgressResult,
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] KsbController controller)
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

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(x => x.GetApprenticeshipKsbProgresses(It.IsAny<long>(), It.IsAny<Guid[]>())).ReturnsAsync(ksbProgressResult);
            Guid id = ksbProgressResult.Select(k => k.KsbId).First();

            var result = await controller.AddUpdateKsbProgress(id);
            result.Should().BeOfType(typeof(ViewResult));
            
        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_Get_NewProgress(
            ApprenticeKsb ksbResult,
            [Frozen] Mock<IOuterApiClient> client,
            [Greedy] KsbController controller)
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

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(x => x.GetApprenticeshipKsb(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                  .ReturnsAsync(ksbResult);
            Guid id = ksbResult.Id;
            ksbResult.Progress = null;
            var result = await controller.AddUpdateKsbProgress(id);
            result.Should().BeOfType(typeof(ViewResult));

        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_Get_NoKsb(
           [Frozen] Mock<IOuterApiClient> client,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Greedy] KsbController controller)
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

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(x => x.GetApprenticeshipKsb(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                 .ReturnsAsync((ApprenticeKsb?)null);
            var result = await controller.AddUpdateKsbProgress(Guid.NewGuid());
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Ksb not found for AddUpdateKsbProgress in KsbController")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().BeOfType(typeof(ViewResult));
            }

        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_Get_NoApprenticeId(
           [Frozen] Mock<IOuterApiClient> client,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Greedy] KsbController controller)
        {
            Guid id = Guid.NewGuid();

            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey,"");
            var claimsPrincipal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                apprenticeIdClaim
            })});

            httpContext.User = claimsPrincipal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.AddUpdateKsbProgress(id);
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Invalid apprenticeId for AddUpdateKsbProgress in KsbController")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().BeOfType(typeof(UnauthorizedResult));
            }
        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_Get_NoApprenticeshipId(
           [Frozen] Mock<IOuterApiClient> client,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Greedy] KsbController controller)
        {
            Guid id = Guid.NewGuid();

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
            client.Setup(client => client.GetApprenticeDetails(It.IsAny<Guid>())).ReturnsAsync((ApprenticeDetails)null);
            var result = await controller.AddUpdateKsbProgress(id) as ViewResult;
            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Invalid apprenticeshipId for AddUpdateKsbProgress in KsbController")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().BeOfType(typeof(ViewResult));
                result.ViewName.Should().Be("Index");
            }
        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_HttpPost_Async_NoApprenticeId(
           [Frozen] Mock<IOuterApiClient> client,
           ApprenticeDetails apprenticeDetails,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Frozen] ApprenticeKsbProgressData ksbProgressData,
           [Greedy] KsbController controller)
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

            ksbProgressData.ApprenticeshipId = 0;
            var result = await controller.AddUpdateKsbProgress(ksbProgressData);

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Invalid apprentice id for HttpPost method AddUpdateKsbProgress in KsbController")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().BeOfType(typeof(UnauthorizedResult));
            }
        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_HttpPost_Async_NoData(
          [Frozen] Mock<IOuterApiClient> client,
          ApprenticeDetails apprenticeDetails,
          [Frozen] Mock<ILogger<KsbController>> logger,
          [Greedy] KsbController controller)
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

            var result = await controller.AddUpdateKsbProgress(null);

            result.Should().BeOfType(typeof(ViewResult));

        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_HttpPost_Async(
           [Frozen] Mock<IOuterApiClient> client,
           ApprenticeDetails apprenticeDetails,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Frozen] ApprenticeKsbProgressData ksbProgressData,
           [Greedy] KsbController controller)
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            ksbProgressData.ApprenticeshipId = 0;
            var result = await controller.AddUpdateKsbProgress(ksbProgressData) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"AddUpdateKsbProgress for KSB")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Ksb");
            }
        }

        [Test, MoqAutoData]
        public async Task Add_Update_KsbProgress_HttpPost_Async_Catch(
          [Frozen] Mock<IOuterApiClient> client,
          ApprenticeDetails apprenticeDetails,
          [Frozen] Mock<ILogger<KsbController>> logger,
          [Frozen] ApprenticeKsbProgressData ksbProgressData,
          [Greedy] KsbController controller)
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            ksbProgressData.ApprenticeshipId = 0;
            client.Setup(c => c.AddUpdateKsbProgress(It.IsAny<long>(), It.IsAny<ApprenticeKsbProgressData>())).ThrowsAsync(new Exception("Error"));
            var result = await controller.AddUpdateKsbProgress(ksbProgressData) as RedirectToActionResult;

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"AddUpdateKsbProgress for KSB")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Ksb");
            }
        }

        [Test, MoqAutoData]
        public async Task EditKsbProgress_HttpPost_Async(
          [Frozen] Mock<IOuterApiClient> client,
          ApprenticeDetails apprenticeDetails,
          [Frozen] Mock<ILogger<KsbController>> logger,
          [Greedy] KsbController controller)
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var ksbId = Guid.NewGuid();
            var ksbKey = "K1";
            var ksbType = KsbType.Knowledge;
            var ksbStatus = KSBStatus.Completed;
            var note = "This is the note";

            var result = await controller.EditKsbProgress(ksbId, ksbKey, ksbType, ksbStatus, note);

           result.Should().BeOfType(typeof(OkResult));
           
        }

        [Test, MoqAutoData]
        public async Task EditKsbProgress_HttpPost_Async_Catch(
         [Frozen] Mock<IOuterApiClient> client,
         ApprenticeDetails apprenticeDetails,
         [Frozen] Mock<ILogger<KsbController>> logger,
         [Greedy] KsbController controller)
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var ksbId = Guid.NewGuid();
            var ksbKey = "K1";
            var ksbType = KsbType.Knowledge;
            var ksbStatus = KSBStatus.Completed;
            var note = "This is the note";

            client.Setup(c => c.AddUpdateKsbProgress(It.IsAny<long>(), It.IsAny<ApprenticeKsbProgressData>())).ThrowsAsync(new Exception("Error"));
            var result = await controller.EditKsbProgress(ksbId, ksbKey, ksbType, ksbStatus, note);

            result.Should().BeOfType(typeof(OkResult));

        }


        [Test, MoqAutoData]
        public async Task EditKsbProgress_Async_NoApprenticeshipId(
           [Frozen] Mock<IOuterApiClient> client,
           ApprenticeDetails apprenticeDetails,
           [Frozen] Mock<ILogger<KsbController>> logger,
           [Greedy] KsbController controller)
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

            var ksbId = Guid.NewGuid();
            var ksbKey = "K1";
            var ksbType = KsbType.Knowledge;
            var ksbStatus = KSBStatus.Completed;
            var note = "This is the note";
            var result = await controller.EditKsbProgress(ksbId, ksbKey, ksbType, ksbStatus, note);

            result.Should().BeOfType(typeof(UnauthorizedResult));
        }

        [Test, MoqAutoData]
        public async Task RemoveTaskFromKsbProgress_HttpDelete_Async(
         int progressId, int taskId,
         [Frozen] Mock<ILogger<KsbController>> logger,
         [Greedy] KsbController controller)
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.RemoveTaskFromKsbProgress(progressId, taskId);

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Removing Task")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().BeOfType(typeof(OkResult));
            }

        }

        [Test, MoqAutoData]
        public async Task RemoveTaskFromKsbProgress_HttpDelete_Async_NoApprenticeId(
         int progressId, int taskId,
         [Frozen] Mock<ILogger<KsbController>> logger,
         [Greedy] KsbController controller)
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

            var result = await controller.RemoveTaskFromKsbProgress(progressId, taskId);

            using (new AssertionScope())
            {
                logger.Verify(x => x.Log(LogLevel.Warning,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Invalid apprentice id for method EditKsbProgress in KsbController")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
                result.Should().BeOfType(typeof(UnauthorizedResult));
            }

        }
        [Test, MoqAutoData]
        public async Task RemoveTaskFromKsbProgress_HttpDelete_Async_Catch(
        [Frozen] Mock<IOuterApiClient> client,
        int progressId, int taskId,
        [Frozen] Mock<ILogger<KsbController>> logger,
        [Greedy] KsbController controller)
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            client.Setup(c => c.RemoveTaskToKsbProgress(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception("Error"));
            var result = await controller.RemoveTaskFromKsbProgress(progressId, taskId);

            result.Should().BeOfType(typeof(OkResult));
        }
    }
}