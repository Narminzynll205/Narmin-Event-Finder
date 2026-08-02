using EventFinder.Web.DTOs.Auth;
using EventFinder.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventFinder.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user with the given roles (Organizer and/or Participant).
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, result) = await _authService.RegisterAsync(dto);
            if (!succeeded)
            {
                return BadRequest(new { errors });
            }

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT access token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, result) = await _authService.LoginAsync(dto);
            if (!succeeded)
            {
                return Unauthorized(new { errors });
            }

            return Ok(result);
        }
    }
}
