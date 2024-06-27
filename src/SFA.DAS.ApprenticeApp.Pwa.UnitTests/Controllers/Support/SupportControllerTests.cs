using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Home
{
    public class SupportControllerTests
    {
        [Test, MoqAutoData]
        public async Task Load_IndexAsync([Greedy] SupportController controller)
        {
            var httpContext = new DefaultHttpContext();

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
            bool likeStatus = true;
            bool isSaved = true;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.AddOrUpdateApprenticeArticle(entryId, likeStatus, isSaved);
            result.Should().BeOfType(typeof(OkResult));
        }

    }
}
