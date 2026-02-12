using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewHelpers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        [TestCase("X1", ExpectedResult = KsbType.Knowledge)]
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

        [Test]
        [TestCase(ApprenticeshipType.Apprenticeship, "Apprenticeship")]
        [TestCase(ApprenticeshipType.FoundationApprenticeship, "Foundation Apprenticeship")]
        public void ApprenticeshipType_HasExpectedDescription(ApprenticeshipType type, string expectedDescription)
        {
            var description = ViewHelpers.Helpers.GetEnumDescription(type);
            Assert.AreEqual(expectedDescription, description);
        }

        [Test]        
        public void ApprenticeshipType_Returns_Null()
        {
            ApprenticeshipType? type = null;

            var description = ViewHelpers.Helpers.GetEnumDescription(type);

            Assert.IsNull(description);
        }
    }
    
     [TestFixture]
    public class SessionExtensionsTests
    {
        [Test]
        public void IsSpecialUser_WhenSessionIsNull_ReturnsFalse()
        {
            // Arrange
            ISession session = null;

            // Act
            var result = session.IsSpecialUser();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSpecialUser_WhenSessionIsNotAvailable_ReturnsFalse()
        {
            // Arrange
            var session = new TestSession { IsAvailable = false };

            // Act
            var result = session.IsSpecialUser();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSpecialUser_WhenSessionIsAvailableButUserTypeIsNull_ReturnsFalse()
        {
            // Arrange
            var session = new TestSession { IsAvailable = true };
            // UserType not set

            // Act
            var result = session.IsSpecialUser();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSpecialUser_WhenSessionIsAvailableAndUserTypeIsNotSpecialUser_ReturnsFalse()
        {
            // Arrange
            var session = new TestSession { IsAvailable = true };
            session.SetString("UserType", "RegularUser");

            // Act
            var result = session.IsSpecialUser();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSpecialUser_WhenSessionIsAvailableAndUserTypeIsSpecialUser_ReturnsTrue()
        {
            // Arrange
            var session = new TestSession { IsAvailable = true };
            session.SetString("UserType", "SpecialUser");

            // Act
            var result = session.IsSpecialUser();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsSpecialUser_WhenSessionIsAvailableAndUserTypeIsSpecialUserCaseInsensitive_ReturnsFalse()
        {
            // Arrange
            var session = new TestSession { IsAvailable = true };
            session.SetString("UserType", "specialuser"); // lowercase

            // Act
            var result = session.IsSpecialUser();

            // Assert
            Assert.IsFalse(result); // Should be false because case-sensitive comparison
        }

        // Test Session implementation for mocking
        private class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _store = new();

            public bool IsAvailable { get; set; }
            public string Id { get; set; } = "TestSessionId";
            public IEnumerable<string> Keys => _store.Keys;

            public void Clear() => _store.Clear();

            public Task CommitAsync(CancellationToken cancellationToken = default) 
                => Task.CompletedTask;

            public Task LoadAsync(CancellationToken cancellationToken = default) 
                => Task.CompletedTask;

            public void Remove(string key) => _store.Remove(key);

            public void Set(string key, byte[] value) => _store[key] = value;

            public bool TryGetValue(string key, out byte[] value) 
                => _store.TryGetValue(key, out value);

            // Helper method for testing
            public void SetString(string key, string value)
            {
                if (value == null)
                {
                    _store.Remove(key);
                }
                else
                {
                    _store[key] = System.Text.Encoding.UTF8.GetBytes(value);
                }
            }
        }
    }
}
