using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using System;
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
            var result = Filter.FilterTaskResults(tasks, "other-filter=REMINDER");

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
            var result = Filter.FilterTaskResults(tasks, "other-filter=NOTE-ADDED");

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


        // KSB FILTERS TESTS

        [Test]
        public void FilterKsbResults_Filter_Returns_Results()
        {
            // Arrange
            var ksbs = new List<ApprenticeKsb>
            {
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.NotStarted } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.Completed } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.InProgress } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.ReadyForReview } }
            };

            // Act
            var result = Filter.FilterKsbResults(ksbs, "filter=ASSIGNMENT&filter=EPA&filter=NOT-STARTED&filter=IN-PROGRESS&filter=READY-FOR-REVIEW&filter=COMPLETED");

            // Assert
            result.FilteredKsbs.Count.Should().Be(4);
            result.HasFilterRun.Should().BeTrue();
        }

        [Test]
        public void FilterKsbResults_otherFilter_Returns_Results()
        {
            // Arrange
            var ksbs = new List<ApprenticeKsb>
            {
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.NotStarted, Tasks = new List<ApprenticeTask>() { new ApprenticeTask() { TaskId = 1 }, new ApprenticeTask() { TaskId = 2 } } } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.Completed } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.InProgress } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.ReadyForReview } }
            };

            // Act
            var result = Filter.FilterKsbResults(ksbs, "other-filter=LINKED-TO-A-TASK");

            // Assert
            result.FilteredKsbs.Count.Should().Be(1);
            result.HasFilterRun.Should().BeTrue();
        }

        [Test]
        public void FilterKsbResults_OtherFilter_ProgressSet_Returns_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeKsb>
            {
                new ApprenticeKsb {  Id = Guid.NewGuid() },
                new ApprenticeKsb {  Id = Guid.NewGuid() },
                new ApprenticeKsb {  Id = Guid.NewGuid() },
                new ApprenticeKsb {  Id = Guid.NewGuid() }
            };

            tasks[0].Progress = new ApprenticeKsbProgressData() { KsbProgressId = 1, CurrentStatus = KSBStatus.Completed };

            // Act
            var result = Filter.FilterKsbResults(tasks, "other-filter=REMINDER-SET");

            // Assert
            result.FilteredKsbs.Count.Should().Be(0);
            result.HasFilterRun.Should().BeTrue();
        }

 

        [Test]
        public void FilterKsbResults_No_Filter_Returns_No_Results()
        {
            // Arrange
            var tasks = new List<ApprenticeKsb>
            {
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.NotStarted } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.Completed } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.InProgress } },
                new ApprenticeKsb {  Id = Guid.NewGuid(), Progress = new ApprenticeKsbProgressData() { CurrentStatus = KSBStatus.ReadyForReview } }
            };

            // Act
            var result = Filter.FilterKsbResults(tasks, "");

            // Assert
            result.FilteredKsbs.Count.Should().Be(0);
            result.HasFilterRun.Should().BeFalse();
        }
    }
}
