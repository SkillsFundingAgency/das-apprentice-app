using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewHelpers;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Helpers
{
    public class ViewHelperTests
    {
        [Test]
        public void KsbStatuses_ReturnsAllKsbStatuses()
        {
            // Act
            var result = KsbHelpers.KSBStatuses();
            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.That(result, Contains.Item(KSBStatus.NotStarted));
            Assert.That(result, Contains.Item(KSBStatus.InProgress));
            Assert.That(result, Contains.Item(KSBStatus.ReadyForReview));
            Assert.That(result, Contains.Item(KSBStatus.Completed));
        }

       
        [TestCase("K1", ExpectedResult = KsbType.Knowledge)]
        [TestCase("S1", ExpectedResult = KsbType.Skill)]
        [TestCase("B1", ExpectedResult = KsbType.Behaviour)]
        public KsbType GetKsbType_Returns_Knowledge_KsbType(string key)
        {
            // Act
            return KsbHelpers.GetKsbType(key);
        }


        [Test]
        [TestCase(KSBStatus.NotStarted, ExpectedResult = "Not started")]
        [TestCase(KSBStatus.InProgress, ExpectedResult = "In progress")]
        [TestCase(KSBStatus.ReadyForReview, ExpectedResult = "Ready for review")]
        [TestCase(KSBStatus.Completed, ExpectedResult = "Completed")]
        public string GetEnumDescription_Returns_Enum_Description(KSBStatus value)
        {
            // Act
            return ViewHelpers.Helpers.GetEnumDescription(value);
        }

        [Test]
        public void GetEnumDescription_Returns_Null_When_Name_Not_Found()
        {
            // Act
            var result = ViewHelpers.Helpers.GetEnumDescription((KSBStatus)100);
            // Assert
            Assert.IsNull(result);
        }
    }
}
