using System.Security.Claims;
using EventFinder.Web.DTOs.Events;
using EventFinder.Web.Models;
using EventFinder.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFinder.Web.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id claim not found.");

        /// <summary>
        /// Returns events, optionally filtered by category, date range and/or proximity
        /// (lat/lng/radiusKm - uses the Haversine formula for distance calculation).
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] EventFilterDto filter)
        {
            var events = await _eventService.GetAllAsync(filter);
            return Ok(events);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            return Ok(ev);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Organizer)]
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _eventService.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Organizer)]
        public async Task<IActionResult> Update(int id, [FromBody] EventUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (found, authorized, result) = await _eventService.UpdateAsync(id, dto, CurrentUserId);
            if (!found)
            {
                return NotFound();
            }

            if (!authorized)
            {
                return Forbid();
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Organizer)]
        public async Task<IActionResult> Delete(int id)
        {
            var (found, authorized) = await _eventService.DeleteAsync(id, CurrentUserId);
            if (!found)
            {
                return NotFound();
            }

            if (!authorized)
            {
                return Forbid();
            }

            return NoContent();
        }

        /// <summary>
        /// Allows the current authenticated user to join an event.
        /// </summary>
        [HttpPost("{id:int}/join")]
        [Authorize]
        public async Task<IActionResult> Join(int id)
        {
            var result = await _eventService.JoinEventAsync(id, CurrentUserId);

            return result switch
            {
                JoinEventResult.Success => Ok(new { message = "Successfully joined the event." }),
                JoinEventResult.EventNotFound => NotFound(new { message = "Event not found." }),
                JoinEventResult.AlreadyJoined => BadRequest(new { message = "You have already joined this event." }),
                JoinEventResult.EventFull => BadRequest(new { message = "This event has reached its maximum number of participants." }),
                JoinEventResult.CannotJoinOwnEvent => BadRequest(new { message = "Organizers cannot join their own event as a participant." }),
                _ => BadRequest()
            };
        }

        /// <summary>
        /// Returns the list of participants who joined the event. Requires authentication
        /// since it exposes participant names/profile pictures.
        /// </summary>
        [HttpGet("{id:int}/participants")]
        [Authorize]
        public async Task<IActionResult> GetParticipants(int id)
        {
            var participants = await _eventService.GetParticipantsAsync(id);
            if (participants == null)
            {
                return NotFound();
            }

            return Ok(participants);
        }
    }
}
