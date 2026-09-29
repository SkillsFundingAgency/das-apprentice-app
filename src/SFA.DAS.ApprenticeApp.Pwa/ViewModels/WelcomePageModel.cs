using SFA.DAS.ApprenticeApp.Pwa.Helpers;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class WelcomePageModel
    {
        public WelcomeStep Step { get; set; } = null!;
        public int Number { get; set; }
        public int Total { get; set; }

        public bool IsFirst => Number == 1;
        public bool IsLast => Number == Total;
    }
}
