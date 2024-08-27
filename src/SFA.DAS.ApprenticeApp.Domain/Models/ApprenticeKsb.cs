using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class ApprenticeKsb
    {
        public KsbType Type { get; set; }
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Detail { get; set; }
        
        public ApprenticeKsbProgressData? Progress { get; set; }
    }

    public enum KsbType
    {
        Knowledge = 1,
        Skill = 2,
        Behaviour = 3,
    } 
}
