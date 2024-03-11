using System;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public sealed class Apprentice : IApprenticeAccount
    {
        public Guid ApprenticeId { get; set; }
        public string FirstName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool TermsOfUseAccepted { get; set; }
    }
}
