using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class TasksBaseModel
    {
        public int Year { get; set; }
        public string Sort { get; set; }

        // Null means the apprentice has no tasks of that status.
        public List<ApprenticeTask>? ToDoTasks { get; set; }
        public List<ApprenticeTask>? DoneTasks { get; set; }
    }
}
