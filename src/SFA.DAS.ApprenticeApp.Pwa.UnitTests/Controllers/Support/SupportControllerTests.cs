using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.ApprenticeApp.Application;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class SupportControllerTests
    {
        [Test, MoqAutoData]
        public async Task Load_IndexAsync([Greedy] SupportController controller)
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

            var result = await controller.Index();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task Load_Articles_PageAsync([Greedy] SupportController controller)
        {
            var httpContext = new DefaultHttpContext();
            var slug = "123";
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

            var result = await controller.ArticlesPage(slug);
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task Load_Saved_Articles_PageAsync([Greedy] SupportController controller)
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

            var result = await controller.SavedArticles();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Test, MoqAutoData]
        public async Task Add_Update_Apprentice_ArticlesAsync([Greedy] SupportController controller)
        {
            var httpContext = new DefaultHttpContext();
            string entryId = "123";
            string entryTitle = "title";
            bool likeStatus = true;
            bool isSaved = true;

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

            var result = await controller.AddOrUpdateApprenticeArticle(entryId, entryTitle, likeStatus, isSaved);
            result.Should().BeOfType(typeof(OkResult));
        }
    }
}