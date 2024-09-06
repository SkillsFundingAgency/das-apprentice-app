using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewHelpers
{
    public static class KsbHelpers
    {
        public static List<KSBStatus> KSBStatuses()
        {
            return Enum.GetValues(typeof(KSBStatus)).Cast<KSBStatus>().ToList();
        }

        public static KsbType GetKsbType(string key)
        {
            char l = key[0];
            switch (l.ToString())
            {
                case "K":
                    return KsbType.Knowledge;

                case "S":
                    return KsbType.Skill;

                case "B":
                    return KsbType.Behaviour;

                default:
                    return KsbType.Knowledge;
            }
        }
    }
}
