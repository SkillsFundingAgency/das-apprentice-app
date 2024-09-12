﻿using FluentAssertions;
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

        [Test]
        public void FilterTaskResults_OtherFilter_ReminderSet_Returns_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeTask>
            {
                new ApprenticeTask { ApprenticeshipCategoryId = 1, TaskReminders = new List<TaskReminder>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 2, TaskReminders = new List<TaskReminder>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 3, TaskReminders = new List<TaskReminder>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 4, TaskReminders = new List<TaskReminder>() }
            };

            tasks[0].TaskReminders = [new TaskReminder() { ReminderId = 1, TaskId = 1, ReminderUnit = 0, ReminderValue = 1 }];

            // Act
            var result = Filter.FilterTaskResults(tasks, "other-filter=REMINDER-SET");

            // Assert
            result.FilteredTasks.Count.Should().Be(1);
            result.HasFilterRun.Should().BeTrue();
        }

        [Test]
        public void FilterTaskResults_OtherFilter_KSB_Returns_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeTask>
            {
                new ApprenticeTask { TaskId = 1, ApprenticeshipCategoryId = 1, TaskLinkedKsbs = new List<TaskKSBs>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 2, TaskLinkedKsbs = new List<TaskKSBs>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 3, TaskLinkedKsbs = new List<TaskKSBs>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 4, TaskLinkedKsbs = new List<TaskKSBs>() }
            };

            tasks[0].TaskLinkedKsbs = [new TaskKSBs() { TaskId = 1, KSBProgressId = 1 }];

            // Act
            var result = Filter.FilterTaskResults(tasks, "other-filter=KSB");

            // Assert
            result.FilteredTasks.Count.Should().Be(1);
            result.HasFilterRun.Should().BeTrue();
        }

        [Test]
        public void FilterTaskResults_OtherFilter_Note_Returns_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeTask>
            {
                new ApprenticeTask { ApprenticeshipCategoryId = 1, Note = "Test Note" },
                new ApprenticeTask { ApprenticeshipCategoryId = 2 },
                new ApprenticeTask { ApprenticeshipCategoryId = 3 },
                new ApprenticeTask { ApprenticeshipCategoryId = 4 }
            };
            // Act
            var result = Filter.FilterTaskResults(tasks, "other-filter=NOTE-ATTACHED");

            // Assert
            result.FilteredTasks.Count.Should().Be(1);
            result.HasFilterRun.Should().BeTrue();
        }

        [Test]
        public void FilterTaskResults_OtherFilter_FileAttached_Returns_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeTask>
            {
                new ApprenticeTask { ApprenticeshipCategoryId = 1, TaskFiles = new List<TaskFile>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 2, TaskFiles = new List<TaskFile>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 3, TaskFiles = new List<TaskFile>() },
                new ApprenticeTask { ApprenticeshipCategoryId = 4, TaskFiles = new List<TaskFile>() }
            };

            tasks[0].TaskFiles = [new TaskFile() { TaskFileId = 1, TaskId = 1, FileName = "Test File", FileType = "Test Type", FileContents = "Test Contents" }];
            // Act
            var result = Filter.FilterTaskResults(tasks, "other-filter=FILES-ATTACHED");

            // Assert
            result.FilteredTasks.Count.Should().Be(1);
            result.HasFilterRun.Should().BeTrue();
        }

        [Test]
        public void FilterTaskResults_No_Filter_Returns_No_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeTask>
            {
                new ApprenticeTask { ApprenticeshipCategoryId = 1 },
                new ApprenticeTask { ApprenticeshipCategoryId = 2 }
            };

            // Act
            var result = Filter.FilterTaskResults(tasks, "");

            // Assert
            result.FilteredTasks.Count.Should().Be(0);
            result.HasFilterRun.Should().BeFalse();
        }
    }
}