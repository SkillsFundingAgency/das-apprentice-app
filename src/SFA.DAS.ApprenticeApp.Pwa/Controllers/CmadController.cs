using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.Services;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using System.Text.Json;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class CmadController : Controller
    {
        private readonly ICommitmentsService _commitmentsService;
        private readonly ILogger<CmadController> _logger;
        private readonly IOuterApiClient _client;
        private readonly IApprenticeContext _apprenticeContext;

        public CmadController(ICommitmentsService commitmentsService, ILogger<CmadController> logger, IOuterApiClient client, IApprenticeContext apprenticeContext)
        {
            _commitmentsService = commitmentsService;
            _logger = logger;
            _client = client;
            _apprenticeContext = apprenticeContext;
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
                // fetch registrations and apprentice
                var registrations = await _client.GetRegistrationByAccountDetails(model.FirstName, model.LastName, dob.ToIsoDate());
                var apprentice = await _client.GetApprentice(model.ApprenticeId);

                // ensure apprentice has basic fields populated
                await _commitmentsService.EnsureApprenticeHasBasicFields(apprentice, model, dob);                

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
                var viewModel = await _commitmentsService.CreateApprenticeshipAndBuildViewModelAsync(
                    registration.RegistrationId,
                    model.ApprenticeId,
                    commitmentsApprenticeship.Uln,
                    apprentice.LastName,
                    dob.ToIsoDate());
                if (viewModel != null)
                {
                    ModelState.Clear();
                    return View("ConfirmApprenticeshipDetails", viewModel);
                }

                return RedirectToAction("AccountNotFound", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error finding apprentice");
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

            try
            {
                foreach (var item in model.RegistrationIds)
                {
                    if (!item.CommitmentApprenticeshipIds.HasValue) continue;
                                        
                    var commitment = await _client.GetCommitmentsApprenticeshipById((long)item.CommitmentApprenticeshipIds);                    

                    if (commitment.StopDate.HasValue || commitment.EndDate <= DateTime.Now) continue;
                    
                    _logger.LogInformation("User inputted ULN: {UserUln} | Commitment ULN: {CommitmentUln}", model.Uln, commitment.Uln);

                    if (commitment?.Uln == model.Uln)
                    {
                        var apprentice = await _client.GetApprentice(model.ApprenticeId);
                        var dobIso = apprentice.DateOfBirth.HasValue
                            ? apprentice.DateOfBirth.Value.ToIsoDate()
                            : null;

                        // Create apprenticeship and build the confirm view model                                            
                        var viewModel = await _commitmentsService.CreateApprenticeshipAndBuildViewModelAsync(
                        item.RegistrationId,
                        model.ApprenticeId,
                        commitment.Uln,
                        apprentice.LastName,
                        dobIso);

                        _logger.LogInformation("Created Apprenticeship from populated commitment: {CommitmentApprenticeshipId}", item.CommitmentApprenticeshipIds.Value);

                        if (viewModel != null)
                        {
                            ModelState.Clear();
                            return View("ConfirmApprenticeshipDetails", viewModel);
                        }
                        
                        return RedirectToAction("AccountNotFound", "Account");
                    } else
                    {
                        _logger.LogInformation("ULN did not match for commitment apprenticeship with id: {CommitmentApprenticeshipId}", item.CommitmentApprenticeshipIds.Value);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {                
                return RedirectToAction("AccountNotFound", "Account");
            }
            
            return RedirectToAction("AccountNotFound", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmApprenticeshipDetails(ConfirmApprenticeshipDetailsViewModel model, Guid apprenticeId, long apprenticeshipId, long revisionId)
        {
            var revision = await _client.GetRevisionById(apprenticeId, apprenticeshipId, revisionId);
            var commitmentsApprenticeship = await _client.GetCommitmentsApprenticeshipById(revision.CommitmentsApprenticeshipId);
            var apprenticeDetails = await _client.GetApprenticeDetails(apprenticeId);

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
                if (apprenticeDetails.MyApprenticeship == null)
                {
                    await _client.CreateMyApprenticeship(apprenticeId, new CreateMyApprenticeshipRequest
                    {
                        ApprenticeshipId = revision.CommitmentsApprenticeshipId,
                        Uln = model.Uln,
                        EmployerName = revision.EmployerName,
                        StartDate = revision.PlannedStartDate.ToIsoDate(),
                        EndDate = revision.PlannedEndDate.ToIsoDate(),
                        TrainingProviderId = revision.TrainingProviderId,
                        TrainingProviderName = revision.TrainingProviderName,
                        StandardUId = commitmentsApprenticeship.StandardUId
                    });
                }
               
                await _client.ConfirmApprenticeshipDetails(apprenticeId, apprenticeshipId, revisionId, confs);               
                return RedirectToAction("Index", "Welcome");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error confirming apprenticeship details for apprenticeId: {ApprenticeId}", apprenticeId);
                return RedirectToAction("CmadError", "Account");
            }
        }
        
        // Views
        [HttpGet]
        public IActionResult ConfirmDetails()
        {
            var authenticatedApprenticeId = _apprenticeContext.ApprenticeId;
            if (authenticatedApprenticeId != null)
            {
                return View(new CheckDetailsViewModel { ApprenticeId = Guid.Parse(authenticatedApprenticeId) });
            }

            return RedirectToAction("AccountNotFound", "Account");
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
        public IActionResult UlnError()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmApprenticeshipDetails()
        {
            var json = TempData.Peek("ConfirmModel") as string;

            if (!string.IsNullOrEmpty(json))
            {
                var tempDataModel = JsonSerializer.Deserialize<ConfirmApprenticeshipDetailsViewModel>(json);
                return View(tempDataModel);
            }

            return RedirectToAction("AccountNotFound", "Account");
        }              
    }
}