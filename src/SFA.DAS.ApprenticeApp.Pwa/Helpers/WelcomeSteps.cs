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
        bool HasMobileImage = true);

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
                "KSBs"),

            new("Keep on top of things with tasks",
                ["Create tasks to help manage your apprenticeship, with reminders to keep you on track."],
                "screen-3",
                "Keep on top of things with tasks"),

            new("Information you can trust",
                ["Get Government-approved information about what you’re entitled to, what you can claim, and where to go if you need support."],
                "screen-4",
                "Information you can trust"),

            new("Stay notified",
                ["Get notified about your task deadlines and useful information about your apprenticeship."],
                "screen-5",
                "Stay notified"),

            new("Your account",
                ["View your apprenticeship details, your progress, and change settings to make Your Apprenticeship work for you."],
                "screen-6",
                "Your account")
        ];

        public static int Count => All.Length;

        public static bool IsValid(int step) => step >= 1 && step <= All.Length;
    }
}
