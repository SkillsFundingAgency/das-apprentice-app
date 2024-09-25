namespace SFA.DAS.ApprenticeApp.Application
{
    public static class Constants
    {
        public const string StubAuthCookieName = "SFA.ApprenticeApp.StubAuthCookie";
        public const string WelcomeSplashScreenCookieName = "SFA.ApprenticeApp.WelcomeSplashScreen";
        public const string TaskFiltersCookieName = "SFA.ApprenticeApp.TaskFilters";
        public const string ApprenticeIdClaimKey = "apprentice_id";
        public const string ApprenticeshipIdClaimKey = "ApprenticeshipId";
        public const string StandardUIdClaimKey = "StandardUId";

        public const string ContentfulTopLevelPageTypeName = "apprenticeAppCategory";
        public const string ContentfulContentPageTypeName = "apprenticeAppArticle";

        public const int ToDoStatus = 0;
        public const int DoneStatus = 1;
    }
}