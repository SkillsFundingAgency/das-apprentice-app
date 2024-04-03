using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeApp.Pwa.MockServer.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeApp.Pwa.MockServer.Controllers
{
    [ExcludeFromCodeCoverage]
    #region snippetDI
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region snippet
        [HttpGet]
        public IActionResult List()
        {
            return Ok(_userRepository.All);
        }

        [HttpGet("{id}")]
        public IActionResult Find(long id)
        {
            var user = _userRepository.Find(id);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();

        }
        #endregion


    }
}
