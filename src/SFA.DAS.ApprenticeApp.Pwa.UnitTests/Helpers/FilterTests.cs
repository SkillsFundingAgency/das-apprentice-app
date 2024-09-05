using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Helpers
{
    public class FilterTests
    {
        [Test]
        public void FilterTaskResults_Filter_Returns_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeTask>
            {
                new ApprenticeTask { ApprenticeshipCategoryId = 1 },
                new ApprenticeTask { ApprenticeshipCategoryId = 2 },
                new ApprenticeTask { ApprenticeshipCategoryId = 3 },
                new ApprenticeTask { ApprenticeshipCategoryId = 4 }
            };

            // Act
            var result = Filter.FilterTaskResults(tasks, "filter=ASSIGNMENT&filter=EPA&filter=DEADLINE&filter=MILESTONE");

            // Assert
            result.FilteredTasks.Count.Should().Be(4);
            result.HasFilterRun.Should().BeTrue();
        } 
    }
}
