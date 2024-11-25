namespace SFA.DAS.ApprenticeApp.Pwa.Authentication
{
    public static class AppDomainExtensions
    {
        public static string GetDomain(string environment)
        {
            string text = environment.ToLower();
            if ((text != "local"))
            {
                if (text == "prd")
                {
                    return "my-apprenticeship.apprenticeships.education.gov.uk";
                }

                return environment.ToLower() + "-apprentice-app.apprenticeships.education.gov.uk";
            }

            return "";
        }
    }
}
