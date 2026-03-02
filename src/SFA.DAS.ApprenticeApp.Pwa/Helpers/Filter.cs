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
            if (string.IsNullOrEmpty(ksbFiltersValue))
                return new FilterKsbResults { FilteredKsbs = new List<ApprenticeKsb>(), HasFilterRun = false };

            // Parse the filter string into groups
            var statusFilters = new List<string>();
            var otherFilters = new List<string>();
            string keyword = null;

            foreach (string filter in ksbFiltersValue.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = filter.Split('=');
                if (parts.Length != 2) continue;

                var filterType = parts[0];
                var filterValue = parts[1];

                if (filterType == "filter")
                    statusFilters.Add(filterValue);
                else if (filterType == "other-filter")
                    otherFilters.Add(filterValue);
                else if (filterType == "keyword")
                    keyword = filterValue;
            }

            // Start with the full list
            IEnumerable<ApprenticeKsb> result = ksbs;

            // Apply status filters (OR within the group)
            if (statusFilters.Any())
            {
                result = result.Where(ksb =>
                {
                    foreach (var status in statusFilters)
                    {
                        switch (status.ToUpper())
                        {
                            case "NOT-STARTED":
                                if (ksb.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.NotStarted || ksb.Progress == null)
                                    return true;
                                break;
                            case "IN-PROGRESS":
                                if (ksb.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.InProgress)
                                    return true;
                                break;
                            case "READY-FOR-REVIEW":
                                if (ksb.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.ReadyForReview)
                                    return true;
                                break;
                            case "COMPLETED":
                                if (ksb.Progress?.CurrentStatus.GetValueOrDefault() == KSBStatus.Completed)
                                    return true;
                                break;
                        }
                    }
                    return false;
                });
            }

            // Apply other filters (currently only "LINKED-TO-A-TASK")
            if (otherFilters.Contains("linked-to-a-task", StringComparer.OrdinalIgnoreCase))
            {
                result = result.Where(ksb => ksb.Progress?.Tasks?.Count > 0);
            }

            // Apply keyword filter
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(ksb =>
                    (ksb.Progress?.Note?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (ksb.Key?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (ksb.Detail?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            return new FilterKsbResults
            {
                FilteredKsbs = result.ToList(),
                HasFilterRun = true
            };
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