namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    /// <summary>
    /// A screen in the welcome tour. <paramref name="Image"/> is the file name stem under
    /// /assets/images/onboarding. A step with <paramref name="HasMobileImage"/> also has a
    /// "-mobile" variant of that file, and CSS swaps between the two.
    /// </summary>
    public sealed record WelcomeStep(
        string Heading,
        string[] Body,
        string Image,
        string ImageAlt,
        bool HasMobileImage = true,
        string[]? ListItems = null);

    /// <summary>
    /// The welcome tour, one record per screen. Each screen is its own page, so the order
    /// here is the order of the URLs /Welcome/1 through to /Welcome/6.
    /// </summary>
    public static class WelcomeSteps
    {
        public static readonly WelcomeStep[] All =
        [
            new("Welcome, let’s take a quick tour of Your Apprenticeship",
                [
                    "Your Apprenticeship helps you learn, prepare for your assessment, and succeed in your career.",
                    "Available on your phone, tablet and computer so you can work on the go or at your desk."
                ],
                "screen-1",
                "Welcome",
                HasMobileImage: false),

            new("All your knowledge, skills and behaviours (KSBs) in one place",
                ["View, search and filter your KSBs, link them to tasks, track your progress and capture notes and reflections."],
                "screen-2",
                "KSBs",
                HasMobileImage: false),

            new("Show your learning with notes and evidence",
                ["Your apprenticeship will involve showing what you've learnt.", 
                "Add notes to start building evidence for your apprenticeship assessments.",
                "Share with your tutors and get their feedback and support."],
                "screen-3",
                "Keep on top of things with tasks",
                HasMobileImage: false),

            new("Here for you with information you can trust",
                [
                "We understand that sometimes things do not go smoothly. We support you every step of the way, with guidance you can trust",
                "Get goverment-approved information about"                
                ],
                "screen-4",
                "Information you can trust",
                HasMobileImage: false,
                ListItems: 
                [
                    "how to do an apprenticeship",
                    "What to do if you need help or support",
                    "benefits and bursaries you can get",
                    "how to connect with other apprentices"
                ]),            

            new("Work how you want to work",
                [
                "Your Apprenticeship is available on your phone, computer and tablet, so you can work on the go, or at your desk",
                "Capture quick notes on site or write on your computer. Move seamlessly between the app and the web, picking up where you left off."
                ],
                "screen-5",
                "Your account",
                HasMobileImage: false)
        ];

        public static int Count => All.Length;

        public static bool IsValid(int step) => step >= 1 && step <= All.Length;
    }
}
