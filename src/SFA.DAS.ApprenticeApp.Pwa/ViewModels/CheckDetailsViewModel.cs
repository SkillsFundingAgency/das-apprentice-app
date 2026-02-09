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
        public DateModel? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Apprenticeship Course is required")]
        public string? ApprenticeCourse { get; set; }

        [Required(ErrorMessage = "Training provider is required")]
        public string? TrainingProvider { get; set; }
        public Guid? ApprenticeId { get; set; }
    }
}
