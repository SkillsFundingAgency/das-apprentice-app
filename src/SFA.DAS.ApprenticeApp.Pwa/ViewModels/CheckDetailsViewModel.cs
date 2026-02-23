using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class CheckDetailsViewModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateModel DateOfBirth { get; set; }       
        public Guid ApprenticeId { get; set; }
    }
}
