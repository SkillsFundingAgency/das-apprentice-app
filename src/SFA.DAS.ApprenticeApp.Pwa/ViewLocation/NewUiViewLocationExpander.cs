using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewLocation
{
    public class NewUiViewLocationExpander : IViewLocationExpander
    {
        private const string NewUiFolder = "NewUI";
        private const string IsNewUiKey = "IsNewUi";
        private const string NewUiOptInCookieName = "SFA.ApprenticeApp.NewUiOptIn";
        private const string CohortUserSessionKey = "CohortUser";
        private const string OptInUserSessionKey = "OptInUser";
        private const string ForceOldUISessionKey = "ForceOldUI";
        private const string UserTypeSessionKey = "UserType";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var httpContext = context.ActionContext.HttpContext;

            // 1. Read opt‑in cookie
            bool optInCookie = httpContext.Request.Cookies[NewUiOptInCookieName] == "true";

            // 2. Update session opt‑in flag to match cookie (sync on every request)
            if (optInCookie)
                httpContext.Session?.SetString(OptInUserSessionKey, "true");
            else
                httpContext.Session?.SetString(OptInUserSessionKey, "false");

            // 3. Read cohort flag from session (set during login)
            bool cohort = httpContext.Session?.GetString(CohortUserSessionKey) == "true";

            // 4. Read forced old‑UI flag (set when user opts out)
            bool forceOldUI = httpContext.Session?.GetString(ForceOldUISessionKey) == "true";

            // 5. Determine final new‑UI eligibility
            bool isNewUi = !forceOldUI && (optInCookie || cohort);

            // 6. Update the legacy UserType session value for backward compatibility
            if (isNewUi)
                httpContext.Session?.SetString(UserTypeSessionKey, "SpecialUser");
            else
                httpContext.Session?.SetString(UserTypeSessionKey, "RegularUser");

            // 7. Store the decision for view location expansion
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
}