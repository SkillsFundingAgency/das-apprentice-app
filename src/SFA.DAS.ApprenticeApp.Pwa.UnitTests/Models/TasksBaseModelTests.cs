using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Models
{
    public class TasksBaseModelTests
    {
        [Test]
        public void TasksBaseModelTests_test()
        {
            var sut = new TasksBaseModel
            {
                Sort = "sortorder",
                Year = 2000
            };

            Assert.AreEqual("sortorder", sut.Sort);
            Assert.AreEqual(2000, sut.Year);
        }
    }
}
