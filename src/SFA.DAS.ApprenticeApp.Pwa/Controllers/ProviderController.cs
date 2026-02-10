using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Domain.Interfaces;
using SFA.DAS.ApprenticeApp.Domain.Models;
using SFA.DAS.ApprenticeApp.Pwa.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.Controllers
{
    [Route("providers", Name = RouteNames.RegisteredProviders)]
    public class ProviderController : Controller
    {
        private readonly IOuterApiClient _client;

        public ProviderController(IOuterApiClient client) 
        {
            _client = client;
        }

        [HttpGet("standards")]        
        public async Task<IActionResult> GetActiveStandards([FromQuery] string query,
            CancellationToken cancellationToken)
        {
            query ??= string.Empty;
            var searchTerm = query.Trim();
            if (searchTerm.Length < 3) return Ok(new List<Courses>());

            var courses = await _client.GetActiveStandards();

            var matchedCourses = courses.Where(courses => courses.Title.Contains(query, StringComparison.OrdinalIgnoreCase));

            return Ok(matchedCourses);
        }

        [HttpGet("registeredProviders")]
        public async Task<IActionResult> GetRegisteredProviders([FromQuery] string query, 
         CancellationToken cancellationToken)
        {
            query ??= string.Empty;
            var searchTerm = query.Trim();
            if (searchTerm.Length < 3) return Ok(new List<RegisteredProviders>());

            var providers = await _client.GetRegisteredProviders();

            var matchedProviders = providers
                .Where(provider => provider.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                                                                || provider.Ukprn.ToString().Contains(query, StringComparison.OrdinalIgnoreCase));

            return Ok(matchedProviders);
        }
    }
}
