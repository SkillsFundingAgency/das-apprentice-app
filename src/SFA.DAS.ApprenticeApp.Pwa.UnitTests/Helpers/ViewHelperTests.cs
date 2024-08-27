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

        [Test]
        public void GetKsbType_Returns_Knowledge_KsbType()
        {
            // Act
            var result = KsbHelpers.GetKsbType("K1");
            // Assert
            Assert.AreEqual(KsbType.Knowledge, result);
        }

        [Test]
        public void GetKsbType_Returns_Skills_KsbType()
        {
            // Act
            var result = KsbHelpers.GetKsbType("S1");
            // Assert
            Assert.AreEqual(KsbType.Skill, result);
        }

        [Test]
        public void GetKsbType_Returns_Behaviour_KsbType()
        {
            // Act
            var result = KsbHelpers.GetKsbType("B1");
            // Assert
            Assert.AreEqual(KsbType.Behaviour, result);
        }
    }
}
