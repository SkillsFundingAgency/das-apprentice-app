using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using System.Globalization;

public class CommitmentsService : ICommitmentsService
{
    private readonly IOuterApiClient _client;
    private readonly ILogger<CommitmentsService> _logger;

    public CommitmentsService(IOuterApiClient client, ILogger<CommitmentsService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ConfirmApprenticeshipDetailsViewModel?> CreateApprenticeshipAndBuildViewModelAsync(
        Guid registrationId,
        Guid apprenticeId,
        string uln,
        string lastName,
        string? dobIso)
    {
        try
        {
            // create apprenticeship from registration (same behavior as controller)
            await _client.CreateApprenticeshipFromRegistration(registrationId, apprenticeId, lastName, dobIso);

            // refresh apprentice details and find the apprenticeship + revision
            var apprenticeDetails = await _client.GetApprenticeDetails(apprenticeId);
            var apprenticeship = apprenticeDetails?.Apprenticeship?.Apprenticeships?.SingleOrDefault();
            if (apprenticeship == null) return null;

            var revision = await _client.GetRevisionById(apprenticeDetails.Apprentice.ApprenticeId, apprenticeship.Id, apprenticeship.RevisionId);
            if (revision == null) return null;

            var apprentice = await _client.GetApprentice(apprenticeId);
            return ConstructViewModel(apprentice, revision, apprenticeship, uln);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating apprenticeship and building view model (registrationId: {RegistrationId}, apprenticeId: {ApprenticeId})",
                registrationId, apprenticeId);            
            return null;
        }
    }

    public ConfirmApprenticeshipDetailsViewModel ConstructViewModel(
        Apprentice apprentice,
        Revision revision,
        Apprenticeship apprenticeship,
        string uln)
    {
        if (revision == null || apprenticeship == null)
            return new ConfirmApprenticeshipDetailsViewModel();

        return new ConfirmApprenticeshipDetailsViewModel
        {
            ApprenticeId = apprenticeship.ApprenticeId,
            ApprenticeshipId = apprenticeship.Id,
            CommitmentsApprenticeshipId = revision.CommitmentsApprenticeshipId,
            Uln = long.Parse(uln, CultureInfo.InvariantCulture),
            RevisionId = revision.RevisionId,
            FullName = $"{apprentice.FirstName} {apprentice.LastName}",
            EmployerName = revision.EmployerName,
            TrainingProviderName = revision.TrainingProviderName,
            TrainingProviderId = revision.TrainingProviderId,
            Apprenticeship = revision.CourseName,
            Level = revision.CourseLevel.ToString(),
            Type = revision.ApprenticeshipType.HasValue ? revision.ApprenticeshipType.Value.ToString() : string.Empty,
            StartDate = revision.PlannedStartDate.ToString(),
            EndDate = revision.PlannedEndDate.ToString(),
        };
    }

    public async Task EnsureApprenticeHasBasicFields(Apprentice apprentice, CheckDetailsViewModel model, DateTime dob)
    {
        if (apprentice == null) return;

        var needsPatch = string.IsNullOrWhiteSpace(apprentice.FirstName)
                         || string.IsNullOrWhiteSpace(apprentice.LastName)
                         || !apprentice.DateOfBirth.HasValue;

        if (!needsPatch) return;

        var patchDoc = new JsonPatchDocument<Apprentice>();
        if (string.IsNullOrWhiteSpace(apprentice.FirstName))
            patchDoc.Replace(x => x.FirstName, model.FirstName);
        if (string.IsNullOrWhiteSpace(apprentice.LastName))
            patchDoc.Replace(x => x.LastName, model.LastName);
        if (!apprentice.DateOfBirth.HasValue)
            patchDoc.Replace(x => x.DateOfBirth, dob);

        if (patchDoc.Operations.Any())
        {
            await _client.UpdateApprentice(model.ApprenticeId, patchDoc);
        }
    }
}