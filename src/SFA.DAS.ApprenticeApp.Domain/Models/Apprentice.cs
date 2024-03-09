﻿using System;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public sealed class Apprentice : IApprenticeAccount
    {
        public Guid ApprenticeId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool TermsOfUseAccepted { get; set; }
    }
}
