namespace SFA.DAS.ApprenticeApp.Pwa.Models
{
    public enum CmadNavigationType
    {
        ConfirmDetails,
        ConfirmApprenticeshipDetails,
        WelcomeIndex
    }

    public sealed class CmadNavigationResult
    {
        public CmadNavigationType NavigationType { get; init; }
        public object? RouteValues { get; init; }
        public string? ConfirmModelJson { get; init; }
    }
}
