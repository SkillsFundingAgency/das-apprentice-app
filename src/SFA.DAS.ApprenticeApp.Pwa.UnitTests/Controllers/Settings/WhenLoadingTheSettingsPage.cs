using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Settings
{
    public class WhenLoadingTheSettingsPage
    {
        [Test, MoqAutoData]
        public void Then_The_Settings_Page_is_displayed(
         [Greedy] SettingsController controller)
        {
            var result =  controller.Index() as ActionResult;
            Assert.IsNotNull(result);
        }
    }
}
