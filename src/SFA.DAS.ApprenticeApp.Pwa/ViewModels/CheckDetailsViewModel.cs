using SFA.DAS.ApprenticeApp.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class CheckDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateModel DateOfBirth { get; set; }

        [Required(ErrorMessage = "Apprenticeship Course is required")]
        public string? ApprenticeCourse { get; set; }

        [Required(ErrorMessage = "Training provider is required")]
        public string? TrainingProvider { get; set; }
        public Guid? ApprenticeId { get; set; }
    }
}
