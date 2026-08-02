using System.Security.Claims;
using EventFinder.Web.DTOs.Users;
using EventFinder.Web.Services;
using EventFinder.Web.ViewModels.Map;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFinder.Web.Controllers
{
    /// <summary>
    /// Full-screen "nearby users" map. Talks directly to <see cref="IUserService"/>.
    /// </summary>
    [Authorize]
    [Route("Map")]
    public class MapPageController : Controller
    {
        private readonly IUserService _userService;

        public MapPageController(IUserService userService)
        {
            _userService = userService;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("")]
        public IActionResult Index()
        {
            return View(new NearbyMapViewModel());
        }

        /// <summary>AJAX: returns nearby users as JSON for the map markers.</summary>
        [HttpGet("Nearby")]
        public async Task<IActionResult> Nearby(double lat, double lng, double radiusKm = 10)
        {
            var users = await _userService.GetNearbyUsersAsync(CurrentUserId, lat, lng, radiusKm);
            return Json(users);
        }

        /// <summary>AJAX: updates the current user's last known location.</summary>
        [HttpPost("Location")]
        [ValidateAntiForgeryToken]
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

            return Ok();
        }
    }
}
