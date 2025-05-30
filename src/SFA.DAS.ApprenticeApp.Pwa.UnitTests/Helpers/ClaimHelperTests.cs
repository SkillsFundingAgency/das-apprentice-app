﻿using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using System.Security.Claims;
using SFA.DAS.ApprenticeApp.Application;
using System;
using AutoFixture.NUnit3;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Helpers
{
    public class ClaimHelperTests
    {
        [Test]
        [TestCase(Constants.ApprenticeIdClaimKey, ExpectedResult = "00000000-0000-0000-0000-000000000000")]
        [TestCase(Constants.TermsAcceptedClaimKey, ExpectedResult = "True")]
        [TestCase("NonexistentClaim", ExpectedResult = "")]
        public string GetClaim_Returns_Claim(string claimKey)
        {
           
            var apprenticeIdClaim = new Claim(Constants.ApprenticeIdClaimKey, "00000000-0000-0000-0000-000000000000");
            var termsAcceptedClaim = new Claim(Constants.TermsAcceptedClaimKey, "True");

            // Arrange
            var httpContext = new DefaultHttpContext();

            ClaimsPrincipal claims = new ClaimsPrincipal();
            claims.AddIdentity(new ClaimsIdentity(new[]
            {
                apprenticeIdClaim,
                termsAcceptedClaim
            }));

            httpContext.User = claims;
            // Act
            return Claims.GetClaim(httpContext, claimKey);
           
        }

        [Test]
        public void GetClaim_Returns_EmptyString_InvalidUser()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            // Act
            var result = Claims.GetClaim(httpContext, "");

            //Assert
            Assert.AreEqual("", result);
        }
    }
}
