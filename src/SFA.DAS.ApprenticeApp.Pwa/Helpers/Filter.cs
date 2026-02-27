using SFA.DAS.ApprenticeApp.Domain.Models;
using System.Linq; // Already used, but ensures Distinct is available

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public class Filter
    {
        public static FilterTaskResults FilterTaskResults(List<ApprenticeTask> tasks, string taskFiltersValue)
        {
            var filteredTasks = new List<ApprenticeTask>();

            if (!string.IsNullOrEmpty(taskFiltersValue))
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
                            case "NONE":
                                filteredTasks.AddRange(tasks.Where(x => x.ApprenticeshipCategoryId.GetValueOrDefault() == 0).ToList());
                                break;
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
                            case "REMINDER":
                                filteredTasks.AddRange(tasks.Where(x => x.TaskReminders.Count > 0).ToList());
                                break;
                            case "KSB":
                                filteredTasks.AddRange(tasks.Where(x => x.TaskLinkedKsbs.Count > 0).ToList());
                                break;
                            case "NOTE-ADDED":
                                filteredTasks.AddRange(tasks.Where(x => x.Note != null).ToList());
                                break;
                            case "FILES-ATTACHED":
                                filteredTasks.AddRange(tasks.Where(x => x.TaskFiles.Count > 0).ToList());
                                break;
                        }
                    }
                }
                
                // Tidy collection
                List<ApprenticeTask> filteredTasksClean = filteredTasks.Distinct().ToList();
                
                return new FilterTaskResults() { FilteredTasks = filteredTasksClean, HasFilterRun = true };
            }
            return new FilterTaskResults() { FilteredTasks = new List<ApprenticeTask>(), HasFilterRun = false };
        }

        public static FilterKsbResults FilterKsbResults(List<ApprenticeKsb> ksbs, string ksbFiltersValue)
        {
            var filteredKsbs = new List<ApprenticeKsb>();

            if (!string.IsNullOrEmpty(ksbFiltersValue))
            {
                foreach (string filter in ksbFiltersValue.Split("&"))
                {
                    string[] filterparts = filter.Split("=");
                    var filterType = filterparts[0];
                    var filterValue = filterparts[1];

                    if (filterType == "filter")
                    {
                        switch (filterValue.ToUpper())
                        {
                            case "NOT-STARTED":
                                filteredKsbs.AddRange(ksbs.Where(x => x.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.NotStarted).ToList());
                                filteredKsbs.AddRange(ksbs.Where(x => x.Progress == null));
                                break;
                            case "IN-PROGRESS":
                                filteredKsbs.AddRange(ksbs.Where(x => x.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.InProgress).ToList());
                                break;
                            case "READY-FOR-REVIEW":
                                filteredKsbs.AddRange(ksbs.Where(x => x.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.ReadyForReview).ToList());
                                break;
                            case "COMPLETED":
                                filteredKsbs.AddRange(ksbs.Where(x => x.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.Completed).ToList());
                                break;
                        }
                    }
                    if (filterType == "other-filter")
                    {
                        switch (filterValue.ToUpper())
                        {
                            case "LINKED-TO-A-TASK":
                                filteredKsbs.AddRange(ksbs.Where(x => x.Progress?.Tasks?.Count > 0).ToList());
                                break;
                        }
                    }
                    // New keyword filter
                    if (filterType == "keyword")
                    {
                        var keyword = filterValue;
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            filteredKsbs.AddRange(ksbs.Where(x =>
                                (x.Progress?.Note?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (x.Key?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (x.Detail?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                            ).ToList());
                        }
                    }
                }

                // Remove duplicates (a KSB could match multiple filters)
                filteredKsbs = filteredKsbs.Distinct().ToList();

                return new FilterKsbResults() { FilteredKsbs = filteredKsbs, HasFilterRun = true };
            }
            return new FilterKsbResults() { FilteredKsbs = new List<ApprenticeKsb>(), HasFilterRun = false };
        }
    }
   
    public class FilterTaskResults
    {
        public List<ApprenticeTask>? FilteredTasks { get; set; }
        public bool HasFilterRun { get; set; }
    }

    public class FilterKsbResults
    {
        public List<ApprenticeKsb>? FilteredKsbs { get; set; }
        public bool HasFilterRun { get; set; }
    }
}