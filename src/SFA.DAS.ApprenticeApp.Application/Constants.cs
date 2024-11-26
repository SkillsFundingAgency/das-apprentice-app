namespace SFA.DAS.ApprenticeApp.Application
{
    public static class Constants
    {
        public const string StubAuthCookieName = "SFA.ApprenticeApp.StubAuthCookie";
        public const string WelcomeSplashScreenCookieName = "SFA.ApprenticeApp.WelcomeSplashScreen";
        public const string TaskFiltersDoneCookieName = "SFA.ApprenticeApp.TaskFiltersDone";
        public const string TaskFiltersTodoCookieName = "SFA.ApprenticeApp.TaskFiltersTodo";
        public const string TaskFilterYearCookieName = "SFA.ApprenticeApp.TaskFilterYear";
        public const string TaskFilterSortCookieName = "SFA.ApprenticeApp.TaskFilterSort";
        public const string KsbFiltersCookieName = "SFA.ApprenticeApp.KsbFilters";
        public const string ApprenticeIdClaimKey = "apprentice_id";
        public const string TermsAcceptedClaimKey = "TermsOfUseAccepted";
        public const string ApprenticeshipIdClaimKey = "ApprenticeshipId";
        public const string StandardUIdClaimKey = "StandardUId";

        public const string ContentfulTopLevelPageTypeName = "apprenticeAppCategory";
        public const string ContentfulContentPageTypeName = "apprenticeAppArticle";

        public const int ToDoStatus = 0;
        public const int DoneStatus = 1;
    }
}