using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticeApp.Pwa.ViewModels;
using System.Text.Json;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    public class CmadController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IOuterApiClient _client;

        public CmadController(ILogger<AccountController> logger, IOuterApiClient client)
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

            try
            {
                ApprenticeDetails apprenticeDetails;
                var apprenticeAccount = await _client.GetApprenticeAccountByName(model.FirstName, model.LastName, model.DateOfBirth.Date.Value);

                // No Account matched                
                if (!apprenticeAccount.Any()) return RedirectToAction("AccountNotFound", "Account");

                // Further filter if more than one account is returned               
                if (apprenticeAccount.Count >= 2) return View("CheckUln");

                var apprentice = apprenticeAccount.SingleOrDefault();

                if (apprentice == null)
                {
                    return RedirectToAction("AccountNotFound", "Account");
                }

                if (model.ApprenticeId == Guid.Empty)
                {
                    apprenticeDetails = await HandleExistingApprenticeAccount(apprenticeAccount.SingleOrDefault());
                }
                else
                {
                    apprenticeDetails = await HandleNonExitingApprenticeAccount((Guid)model.ApprenticeId, apprenticeAccount.SingleOrDefault());
                }

                if (apprenticeDetails?.MyApprenticeship != null)
                {
                    var viewModel = ConstructViewModel(apprenticeDetails);
                    ModelState.Clear();
                    return View("ConfirmApprenticeshipDetails", viewModel);
                }
                else
                {
                    return RedirectToAction("AccountNotFound", "Account");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error finding apprentice");
                return RedirectToAction("AccountNotFound", "Account");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ConfirmUln(CheckUlnViewModel model)
        {
            var apprenticeship = await _client.GetApprenticeshipByUln(model.Uln);

            if (apprenticeship == null) return View("AccountNotFound");

            var apprenticeDetails = await _client.GetApprenticeDetails(apprenticeship.ApprenticeId);

            var viewModel = ConstructViewModel(apprenticeDetails);
            ModelState.Clear();            

            return View("ConfirmApprenticeshipDetails", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmApprenticeshipDetails(Guid apprenticeId, long apprenticeshipId, long revisionId)
        {
            var confs = new Confirmations()
            {
                ApprenticeshipCorrect = true,
                ApprenticeshipDetailsCorrect = true,
                EmployerCorrect = true,
                RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed
                | RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed
                | RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
                HowApprenticeshipDeliveredCorrect = true,
                TrainingProviderCorrect = true
            };            

            // Update Claims Context
            HttpContext.Session.SetString("_currentApprenticeId", apprenticeId.ToString());

            try
            {                
                await _client.ConfirmApprenticeshipDetails(apprenticeId, apprenticeshipId, revisionId, confs);
                return RedirectToAction("Index", "Terms");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Error confirming apprenticeship details for apprenticeId: {apprenticeId}");
                return RedirectToAction("CmadError", "Account");
            }
        }

        private async Task<ApprenticeDetails> HandleExistingApprenticeAccount(Apprentice apprenticeAccount)
        {
            var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeAccount.ApprenticeId.ToByteArray()));

            // Update Claims Context
            HttpContext.Session.SetString("_currentApprenticeId", apprenticeAccount.ApprenticeId.ToString());

            return apprenticeDetails;

        }

        private async Task<ApprenticeDetails> HandleNonExitingApprenticeAccount(Guid apprenticeId, Apprentice apprenticeAccount)
        {
            var existingApprenticeAccount = await _client.GetApprentice(apprenticeId);

            var pathDoc = new JsonPatchDocument<Apprentice>();
            pathDoc.Replace(x => x.Email, existingApprenticeAccount.Email);
            pathDoc.Replace(x => x.GovUkIdentifier, existingApprenticeAccount.GovUkIdentifier);

            var apprenticeDetails = await _client.GetApprenticeDetails(new Guid(apprenticeAccount.ApprenticeId.ToByteArray()));

            if (apprenticeDetails == null)
            {
                _logger.LogInformation($"Apprentice Details not found for {apprenticeId}");
                return new ApprenticeDetails();
            }

            // Update Provider Apprentice Account with User OneLogin credentials              
            await _client.DeleteApprenticeAccount(existingApprenticeAccount.ApprenticeId);
            await _client.UpdateApprentice(new Guid(apprenticeDetails.Apprentice.ApprenticeId.ToByteArray()), pathDoc);

            // Update Claims Context
            HttpContext.Session.SetString("_currentApprenticeId", apprenticeId.ToString());

            return apprenticeDetails;
        }

        private static ConfirmApprenticeshipDetailsViewModel ConstructViewModel(ApprenticeDetails apprenticeDetails)
        {                        
            var model = new ConfirmApprenticeshipDetailsViewModel()
            {
                ApprenticeId = apprenticeDetails.Apprentice.ApprenticeId,
                ApprenticeshipId = apprenticeDetails.MyApprenticeship.ApprenticeshipId,
                RevisionId = 20034,
                FullName = $"{apprenticeDetails.Apprentice.FirstName} {apprenticeDetails.Apprentice.LastName}",
                Employer = apprenticeDetails.Apprenticeship.Apprenticeships.SingleOrDefault()?.EmployerName,
                Provider = apprenticeDetails.MyApprenticeship.TrainingProviderName,
                Apprenticeship = apprenticeDetails.MyApprenticeship.Title,
                Level = apprenticeDetails.MyApprenticeship.Level.ToString(),
                Type = "Foundation", // GetDescription method to get description of enum
                StartDate = apprenticeDetails.MyApprenticeship.StartDate.ToString(),
                EndDate = apprenticeDetails.MyApprenticeship.EndDate.ToString(),
            };

            return model;
        }       

        // Views
        [HttpGet]
        public IActionResult ConfirmDetails(Guid apprenticeId)
        {
            return View(new CheckDetailsViewModel { ApprenticeId = apprenticeId });
        }

        [HttpGet]
        public IActionResult CheckUln()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmApprenticeshipDetails(ConfirmApprenticeshipDetailsViewModel model)
        {          
            return View(model);
        }
    }
}
