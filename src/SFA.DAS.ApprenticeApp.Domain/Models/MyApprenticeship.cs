using System.ComponentModel;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class MyApprenticeship
    {
        public long ApprenticeshipId { get; set; }
        public long Uln { get; set; }
        public string EmployerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long TrainingProviderId { get; set; }
        public string TrainingProviderName { get; set; }
        public string TrainingCode { get; set; }
        public string StandardUId { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }

        public TimeSpan? ApprenticeshipLength
        {
            get
            {
                if (EndDate.HasValue && StartDate.HasValue)
                {
                    return EndDate.Value - StartDate.Value;
                }
                return null;
            }
        }
        public ApprenticeshipType? ApprenticeshipType { get; set; }
    }

    public enum ApprenticeshipType
    {        
        [Description("Apprenticeship")]
        Apprenticeship = 0,
        [Description("Foundation Apprenticeship")]
        FoundationApprenticeship = 1
    }
}