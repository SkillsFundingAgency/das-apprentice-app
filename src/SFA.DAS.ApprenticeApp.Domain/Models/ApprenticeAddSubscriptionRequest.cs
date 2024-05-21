namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeAddSubscriptionRequest
    {
        public string Endpoint { get; set; }
        public string PublicKey { get; set; }
        public string AuthenticationSecret { get; set; }
        public bool IsSubscribed { get; set; }
    }

    public class ApprenticeDetailsResponse
    {
        public Apprentice Apprentice { get; set; }
        public Apprenticeship MyApprenticeship { get; set; }
    }
}
