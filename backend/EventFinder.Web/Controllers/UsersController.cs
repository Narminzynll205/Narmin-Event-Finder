using System.Security.Claims;
using EventFinder.Web.DTOs.Users;
using EventFinder.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFinder.Web.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id claim not found.");

        /// <summary>
        /// Returns other users within the given radius (km) of the provided coordinates,
        /// based on their last reported location. Distance calculated via the Haversine formula.
        /// </summary>
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radiusKm = 5)
        {
            var nearbyUsers = await _userService.GetNearbyUsersAsync(CurrentUserId, lat, lng, radiusKm);
            return Ok(nearbyUsers);
        }

        /// <summary>
        /// Updates the current authenticated user's coordinates (used for the nearby-users feature).
        /// </summary>
        [HttpPut("location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _userService.UpdateLocationAsync(CurrentUserId, dto.Latitude, dto.Longitude);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
