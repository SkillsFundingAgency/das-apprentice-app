using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using System.Globalization;
using System.Text.Json;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class CmadController : Controller
    {
        private readonly ILogger<CmadController> _logger;
        private readonly IOuterApiClient _client;

        public CmadController(ILogger<CmadController> logger, IOuterApiClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> CheckDetails(CheckDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfirmDetails", model);
            }

            if (model.DateOfBirth == null || !model.DateOfBirth.IsValid || !model.DateOfBirth.TryGetDate(out var dob))
            {
                ModelState.AddModelError(nameof(model.DateOfBirth), "Enter a valid date of birth");
                return View("ConfirmDetails", model);
            }

            try
            {
                var dobIso = ToIsoDate(dob);

                // fetch registrations and apprentice
                var registrations = await _client.GetRegistrationByAccountDetails(model.FirstName, model.LastName, dobIso);
                var apprentice = await _client.GetApprentice(model.ApprenticeId);

                // ensure apprentice has basic fields populated
                await EnsureApprenticeHasBasicFields(apprentice, model, dob);

                // No Account matched
                if (registrations == null || registrations.Count == 0)
                    return RedirectToAction("AccountNotFound", "Account");

                // Save for next page
                TempData["Registrations"] = JsonSerializer.Serialize(registrations);

                // Multiple -> ask for ULN
                if (registrations.Count >= 2)
                    return RedirectToAction("CheckUln", new { model.ApprenticeId });

                var registration = registrations.SingleOrDefault();
                if (registration == null)
                    return RedirectToAction("AccountNotFound", "Account");


                var commitmentsApprenticeship = await _client.GetCommitmentsApprenticeshipById(registration.CommitmentsApprenticeshipId);

                // Create apprenticeship and prepare view model
                var viewModel = await CreateApprenticeshipAndBuildViewModel(registration.RegistrationId, model.ApprenticeId, commitmentsApprenticeship.Uln, apprentice.LastName, dobIso);
                if (viewModel != null)
                {
                    ModelState.Clear();
                    return View("ConfirmApprenticeshipDetails", viewModel);
                }

                return RedirectToAction("AccountNotFound", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error finding apprentice {ApprenticeId}", model.ApprenticeId);
                return RedirectToAction("AccountNotFound", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUln(CheckUlnViewModel model)
        {
            // Guard
            if (model?.RegistrationIds == null || model.RegistrationIds.Count == 0)
            {
                return View("AccountNotFound", "Account");
            }

            foreach (var item in model.RegistrationIds)
            {
                if (!item.CommitmentApprenticeshipIds.HasValue) continue;

                var commitment = await _client.GetCommitmentsApprenticeshipById((long)item.CommitmentApprenticeshipIds);
                if (commitment?.Uln == model.Uln.ToString())
                {
                    var apprentice = await _client.GetApprentice(model.ApprenticeId);
                    var dobIso = apprentice.DateOfBirth.HasValue
                        ? ToIsoDate(apprentice.DateOfBirth.Value)
                        : null;

                    // Create apprenticeship and build the confirm view model
                    var viewModel = await CreateApprenticeshipAndBuildViewModel(item.RegistrationId, model.ApprenticeId, commitment.Uln, apprentice.LastName, dobIso);
                    if (viewModel != null)
                    {
                        ModelState.Clear();
                        return View("ConfirmApprenticeshipDetails", viewModel);
                    }

                    return RedirectToAction("AccountNotFound", "Account");
                }
            }

            return View("AccountNotFound");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmApprenticeshipDetails(ConfirmApprenticeshipDetailsViewModel model, Guid apprenticeId, long apprenticeshipId, long revisionId)
        {
            var revision = await _client.GetRevisionById(apprenticeId, apprenticeshipId, revisionId);

            var confs = new Confirmations()
            {
                ApprenticeshipCorrect = true,
                ApprenticeshipDetailsCorrect = true,
                EmployerCorrect = true,
                RolesAndResponsibilitiesConfirmations =
                    RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed
                    | RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed
                    | RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
                HowApprenticeshipDeliveredCorrect = true,
                TrainingProviderCorrect = true
            };          
            
            try
            {
                await _client.ConfirmApprenticeshipDetails(apprenticeId, apprenticeshipId, revisionId, confs);
                await _client.CreateMyApprenticeship(apprenticeId, new CreateMyApprenticeshipRequest
                {
                    ApprenticeshipId = revision.CommitmentsApprenticeshipId,
                    Uln = model.Uln,
                    EmployerName = revision.EmployerName,
                    StartDate = ToIsoDate(revision.PlannedStartDate),
                    EndDate = ToIsoDate(revision.PlannedEndDate),
                    TrainingProviderId = revision.TrainingProviderId,
                    TrainingProviderName = revision.TrainingProviderName,                    
                });
                return RedirectToAction("Index", "Welcome");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error confirming apprenticeship details for apprenticeId: {ApprenticeId}", apprenticeId);
                return RedirectToAction("CmadError", "Account");
            }
        }

        private static ConfirmApprenticeshipDetailsViewModel ConstructViewModel(Apprentice apprentice, Revision revision, Apprenticeship apprenticeship, string uln)
        {
            if (revision == null || apprenticeship == null) return new ConfirmApprenticeshipDetailsViewModel();

            return new ConfirmApprenticeshipDetailsViewModel()
            {
                ApprenticeId = apprenticeship.ApprenticeId,
                ApprenticeshipId = apprenticeship.Id,
                CommitmentsApprenticeshipId = revision.CommitmentsApprenticeshipId,
                Uln = long.Parse(uln),
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

        // Views
        [HttpGet]
        public IActionResult ConfirmDetails(Guid apprenticeId)
        {
            return View(new CheckDetailsViewModel { ApprenticeId = apprenticeId });
        }

        [HttpGet]
        public async Task<IActionResult> CheckUln(Guid apprenticeId, string token)
        {
            var model = new CheckUlnViewModel();
            var registrations = new List<Registration>();

            if (TempData["Registrations"] is string json)
            {
                registrations = JsonSerializer.Deserialize<List<Registration>>(json) ?? new List<Registration>();
            }

            var registrationIds = registrations.Select(r => new RegistrationDetails
            {
                CommitmentApprenticeshipIds = r.CommitmentsApprenticeshipId,
                RegistrationId = r.RegistrationId
            }).ToList();            

            model.ApprenticeId = apprenticeId;
            model.RegistrationIds = registrationIds;

            return View(model);
        }

        [HttpGet]
        public IActionResult ConfirmApprenticeshipDetails(ConfirmApprenticeshipDetailsViewModel model)
        {
            return View(model);
        }

        // ---------------------------
        // Helpers
        // ---------------------------
        private static string ToIsoDate(DateTime date) =>
            date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private async Task EnsureApprenticeHasBasicFields(Apprentice apprentice, CheckDetailsViewModel model, DateTime dob)
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

        /// <summary>
        /// Creates an apprenticeship from a registration and returns the constructed ConfirmApprenticeshipDetailsViewModel if successful; otherwise null.
        /// </summary>
        private async Task<ConfirmApprenticeshipDetailsViewModel?> CreateApprenticeshipAndBuildViewModel(Guid registrationId, Guid apprenticeId, string uln, string lastName, string dobIso)
        {
            // Create apprenticeship from registration
            await _client.CreateApprenticeshipFromRegistration(registrationId, apprenticeId, lastName, dobIso);

            // Refresh apprentice details & find apprenticeship + revision
            var apprenticeDetails = await _client.GetApprenticeDetails(apprenticeId);
            var apprenticeship = apprenticeDetails?.Apprenticeship?.Apprenticeships?.SingleOrDefault();
            if (apprenticeship == null) return null;

            var revision = await _client.GetRevisionById(apprenticeDetails.Apprentice.ApprenticeId, apprenticeship.Id, apprenticeship.RevisionId);
            if (revision == null) return null;

            var apprentice = await _client.GetApprentice(apprenticeId);
            return ConstructViewModel(apprentice, revision, apprenticeship, uln);
        }
    }
}