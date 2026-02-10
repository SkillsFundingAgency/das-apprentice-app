using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class Confirmations
    {
        public bool? EmployerCorrect { get; set; }
        public bool? TrainingProviderCorrect { get; set; }
        public bool? ApprenticeshipDetailsCorrect { get; set; }
        public RolesAndResponsibilitiesConfirmations? RolesAndResponsibilitiesConfirmations { get; set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; set; }
        public bool? ApprenticeshipCorrect { get; set; }
    }
}
