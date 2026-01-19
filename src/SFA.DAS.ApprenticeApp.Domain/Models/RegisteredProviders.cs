using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class RegisteredProviders
    {
        [Required]
        public string Name { get; set; } = null!;
        public int Ukprn { get; set; }
    }
}
