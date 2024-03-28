namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public class DomainHelper
    {
        public string ParentDomain { get; }
        public bool Secure { get; }

        public DomainHelper(string parentDomain)
        {
            ParentDomain = parentDomain;
            Secure = parentDomain != ".localhost";
        }
    }
}