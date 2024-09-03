using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public class Filter
    {
        public static FilterTaskResults FilterTaskResults(List<ApprenticeTask> tasks, string taskFiltersValue)
        {
            var filteredTasks = new List<ApprenticeTask>();

            if (taskFiltersValue != null)
            {
                foreach (string filter in taskFiltersValue.Split("&"))
                {
                    string[] filterparts = filter.Split("=");
                    var filterType = filterparts[0];
                    var filterValue = filterparts[1];

                    if (filterType == "filter")
                    {
                        switch (filterValue.ToUpper())
                        {
                            case "ASSIGNMENT":
                                filteredTasks.AddRange(tasks.Where(x => x.ApprenticeshipCategoryId.GetValueOrDefault() == 1).ToList());
                                break;
                            case "EPA":
                                filteredTasks.AddRange(tasks.Where(x => x.ApprenticeshipCategoryId.GetValueOrDefault() == 2).ToList());
                                break;
                            case "DEADLINE":
                                filteredTasks.AddRange(tasks.Where(x => x.ApprenticeshipCategoryId.GetValueOrDefault() == 3).ToList());
                                break;
                            case "MILESTONE":
                                filteredTasks.AddRange(tasks.Where(x => x.ApprenticeshipCategoryId.GetValueOrDefault() == 4).ToList());
                                break;
                        }
                    }
                    if (filterType == "other-filter")
                    {
                        switch (filterValue.ToUpper())
                        {
                            case "REMINDER-SET":
                                filteredTasks.AddRange(tasks.Where(x => x.TaskReminders.Count > 0).ToList());
                                break;
                            case "KSB":
                                filteredTasks.AddRange(tasks.Where(x => x.TaskLinkedKsbs.Count > 0).ToList());
                                break;
                            case "NOTE-ATTACHED":
                                filteredTasks.AddRange(tasks.Where(x => x.Note != null).ToList());
                                break;
                            case "FILES-ATTACHED":
                                filteredTasks.AddRange(tasks.Where(x => x.TaskFiles.Count > 0).ToList());
                                break;
                        }
                    }
                }
                return new FilterTaskResults() { FilteredTasks = filteredTasks, HasFilterRun = true };
            }

            return new FilterTaskResults() { FilteredTasks = new List<ApprenticeTask>(), HasFilterRun = false };
        }
    }

    public class FilterTaskResults
    {
        public List<ApprenticeTask>? FilteredTasks { get; set; }
        public bool HasFilterRun { get; set; }
    }
}