using Microsoft.AspNetCore.Mvc.Razor;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewLocation;

public class NewUiViewLocationExpander : IViewLocationExpander
{
    private const string NewUiFolder = "NewUI";
    private const string IsNewUiKey = "IsNewUi";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        var httpContext = context.ActionContext.HttpContext;
        var isNewUi = Claims.GetClaim(httpContext, Constants.NewUiEnabledClaimKey);
        context.Values[IsNewUiKey] = isNewUi;
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        if (context.Values.TryGetValue(IsNewUiKey, out var isNewUi)
            && string.Equals(isNewUi, "true", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var location in viewLocations)
            {
                // NewUI variant first
                yield return location.Replace("/Views/", $"/Views/{NewUiFolder}/");
                // Original as fallback
                yield return location;
            }
        }
        else
        {
            foreach (var location in viewLocations)
            {
                yield return location;
            }
        }
    }
}
