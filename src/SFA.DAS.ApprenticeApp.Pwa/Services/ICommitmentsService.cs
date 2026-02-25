using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;

namespace SFA.DAS.ApprenticeApp.Pwa.Services
{
    public interface ICommitmentsService
    {
        /// <summary>
        /// Creates an apprenticeship from a registration and returns the ConfirmApprenticeshipDetailsViewModel if successful; otherwise null.
        /// </summary>
        Task<ConfirmApprenticeshipDetailsViewModel?> CreateApprenticeshipAndBuildViewModelAsync(
            Guid registrationId,
            Guid apprenticeId,
            string uln,
            string lastName,
            string? dobIso);

        /// <summary>
        /// Maps domain objects into the confirm apprenticeship view model.
        /// Public so it can be reused directly if callers already have domain objects.
        /// </summary>
        ConfirmApprenticeshipDetailsViewModel ConstructViewModel(
            Apprentice apprentice,
            Revision revision,
            Apprenticeship apprenticeship,
            string uln);

        Task EnsureApprenticeHasBasicFields(
            Apprentice apprentice,
            CheckDetailsViewModel model,
            DateTime dob);
    }
}
