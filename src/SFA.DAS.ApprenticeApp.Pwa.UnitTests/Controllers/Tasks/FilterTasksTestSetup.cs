using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Controllers;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Controllers.Tasks
{
    public class FilterTasksTestSetup : TasksController
    {
        private readonly Mock<ILogger<TasksController>> _logger;
        private readonly Mock<IOuterApiClient> _client;

        public FilterTasksTestSetup(Mock<ILogger<TasksController>> logger, Mock<IOuterApiClient> client)
            : base(logger.Object, client.Object)
        {
            _logger = logger;
            _client = client;
        }

        public List<ApprenticeTask> TestFilterTasks(List<ApprenticeTask> tasks)
        {
             return FilterTasks(tasks);
        }

       
    }
}
