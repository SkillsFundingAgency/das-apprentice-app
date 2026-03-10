namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeDetails
    {
        public Apprentice Apprentice { get; set; } = null!;
        public ApprenticeshipsList? Apprenticeship { get; set; } = null!;
        public MyApprenticeship? MyApprenticeship { get; set; } = null!;
    }
}