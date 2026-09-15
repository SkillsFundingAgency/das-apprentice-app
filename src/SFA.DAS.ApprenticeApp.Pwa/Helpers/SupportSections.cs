using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    /// <summary>
    /// A section on the Support index. <paramref name="Id"/> is the anchor target,
    /// and is deliberately separate from <paramref name="Title"/> so that reworded
    /// headings do not break links people have already shared or bookmarked.
    /// </summary>
    public sealed record SupportSection(string Id, string Title);

    /// <summary>
    /// Groups the Support and Guidance categories into the sections shown on the
    /// Support index. Contentful has no concept of these sections, so membership
    /// is held here by slug. A category whose slug is not listed - a new one added
    /// in Contentful, or one that has been renamed - falls into <see cref="Fallback"/>
    /// rather than disappearing from the page.
    /// </summary>
    public static class SupportSections
    {
        public static readonly SupportSection NeedHelpNow = new("need-help-now", "Need help now?");
        public static readonly SupportSection HowToDoYourApprenticeship = new("how-to", "How to do an apprenticeship");
        public static readonly SupportSection Benefits = new("benefits", "Apprentice benefits");
        public static readonly SupportSection Support = new("support", "Support with your apprenticeship");

        public static readonly SupportSection Fallback = Support;

        /// <summary>The order sections appear on the page.</summary>
        public static readonly SupportSection[] Order =
        [
            NeedHelpNow,
            HowToDoYourApprenticeship,
            Benefits,
            Support
        ];

        private static readonly Dictionary<string, SupportSection> SectionBySlug = new()
        {
            ["contact-us"] = NeedHelpNow,
            ["mental-health-support"] = NeedHelpNow,
            ["thinking-of-dropping-out"] = NeedHelpNow,
            ["check-your-pay-and-get-help-with-money"] = NeedHelpNow,
            ["i-have-a-problem-at-work-or-training"] = NeedHelpNow,

            ["understanding-knowledge-skills-and-behaviours-ksbs"] = HowToDoYourApprenticeship,
            ["off-the-job-otj-training"] = HowToDoYourApprenticeship,
            ["apprenticeship-assessments"] = HowToDoYourApprenticeship,

            ["get-student-discounts"] = Benefits,
            ["connect-and-network-with-other-apprentices"] = Benefits,

            ["your-rights-as-an-apprentice"] = Support,
            ["roles-and-responsibilities"] = Support,
            ["support-for-a-learning-difficulty-or-disability"] = Support,
            ["support-for-care-experienced-apprentices"] = Support,
            ["training-provider-feedback"] = Support,
            ["after-your-apprenticeship"] = Support
        };

        public static SupportSection SectionFor(string? slug) =>
            slug != null && SectionBySlug.TryGetValue(slug, out var section) ? section : Fallback;

        /// <summary>
        /// The categories in section order, each keeping the Contentful ordering
        /// within its section. Sections with no categories are left out.
        /// </summary>
        public static IEnumerable<(string Id, string Title, List<ApprenticeAppCategoryPage> Categories)> GroupCategories(
            IEnumerable<ApprenticeAppCategoryPage>? categories)
        {
            var bySection = (categories ?? [])
                .Where(c => c.Slug != null)
                .GroupBy(c => SectionFor(c.Slug))
                .ToDictionary(g => g.Key, g => g.OrderBy(c => c.ArticleOrder).ToList());

            foreach (var section in Order)
            {
                if (bySection.TryGetValue(section, out var inSection))
                {
                    yield return (section.Id, section.Title, inSection);
                }
            }
        }
    }
}
