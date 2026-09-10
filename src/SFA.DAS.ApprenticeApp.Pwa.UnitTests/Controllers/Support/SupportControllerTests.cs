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
        public async Task Update_Saved_Article_Redirects_To_Category_PageAsync([Greedy] SupportController controller)
        {
            controller.ControllerContext = new ControllerContext { HttpContext = BuildHttpContext() };

            var result = await controller.UpdateSavedArticle("123", "An article title", true, "wellbeing");

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("ArticlesPage");
            redirect.RouteValues.Should().ContainKey("slug").WhoseValue.Should().Be("wellbeing");
            redirect.Fragment.Should().Be("accordion-default-content-123");
        }

        [Test, MoqAutoData]
        public async Task Update_Saved_Article_Without_Slug_Redirects_To_Saved_ArticlesAsync([Greedy] SupportController controller)
        {
            controller.ControllerContext = new ControllerContext { HttpContext = BuildHttpContext() };

            var result = await controller.UpdateSavedArticle("123", "An article title", false);

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("SavedArticles");
        }

        [Test, MoqAutoData]
        public async Task Update_Saved_Article_Returns_Ok_For_Background_PostAsync([Greedy] SupportController controller)
        {
            var httpContext = BuildHttpContext();
            httpContext.Request.Headers.XRequestedWith = "XMLHttpRequest";
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = await controller.UpdateSavedArticle("123", "An article title", true, "wellbeing");

            result.Should().BeOfType(typeof(OkResult));
        }

        private static DefaultHttpContext BuildHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, Guid.NewGuid().ToString());
            httpContext.User = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { apprenticeIdClaim }) });
            return httpContext;
        }
    }
}