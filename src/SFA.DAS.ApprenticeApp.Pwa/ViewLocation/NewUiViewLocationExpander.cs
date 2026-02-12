using Microsoft.AspNetCore.Mvc.Razor;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.ViewHelpers;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewLocation;

public class NewUiViewLocationExpander : IViewLocationExpander
{
    private const string NewUiFolder = "NewUI";
    private const string IsNewUiKey = "IsNewUi";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        var httpContext = context.ActionContext.HttpContext;
        var isNewUi = httpContext.Session?.IsSpecialUser() ?? false;
        
        // Store as string for the key/value dictionary
        context.Values[IsNewUiKey] = isNewUi.ToString();
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        if (context.Values.TryGetValue(IsNewUiKey, out var isNewUiValue) 
            && bool.TryParse(isNewUiValue, out var isNewUi) 
            && isNewUi)
        {
            // For new UI users, first try the NewUI folder, then fallback to original
            foreach (var location in viewLocations)
            {
                yield return location.Replace("/Views/", $"/Views/{NewUiFolder}/");
                yield return location;
            }
        }
        else
        {
            // For regular users, only original locations
            foreach (var location in viewLocations)
            {
                yield return location;
            }
        }
    }
}