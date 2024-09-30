using SFA.DAS.ApprenticeApp.Web.Extensions;
using NUnit.Framework;
using System;

namespace SFA.DAS.ApprenticeApp.Web.UnitTests.Extensions {
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void ToGdsFormat_ShouldReturnDateInExpectedFormat()
        {
            var date = new DateTime(2024, 9, 24); // Example date
            var result = date.ToGdsFormat();
            Assert.AreEqual("24 September 2024", result);
        }

        [Test]
        public void ToGdsFormat_ShouldHandleSingleDigitDay()
        {
            var date = new DateTime(2024, 9, 5); // Example date with single digit day
            var result = date.ToGdsFormat();
            Assert.AreEqual("5 September 2024", result);
        }
    }
}