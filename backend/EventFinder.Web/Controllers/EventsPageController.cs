using System.Security.Claims;
using EventFinder.Web.DTOs.Events;
using EventFinder.Web.Models;
using EventFinder.Web.Services;
using EventFinder.Web.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFinder.Web.Controllers
{
    /// <summary>
    /// Server-rendered (Razor) event pages: list/filter, details, create/edit.
    /// Talks directly to <see cref="IEventService"/> - no HTTP round-trip to the JSON API.
    /// </summary>
    [Route("Events")]
    public class EventsPageController : Controller
    {
        private readonly IEventService _eventService;

        public EventsPageController(IEventService eventService)
        {
            _eventService = eventService;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(EventCategory? category, DateTime? dateFrom, DateTime? dateTo, double? lat, double? lng, double? radiusKm)
        {
            var filter = new EventFilterDto
            {
                Category = category,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Lat = lat,
                Lng = lng,
                RadiusKm = radiusKm
            };

            var vm = new EventsIndexViewModel
            {
                Category = category,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Lat = lat,
                Lng = lng,
                RadiusKm = radiusKm,
                Events = await _eventService.GetAllAsync(filter)
            };

            return View(vm);
        }

        [HttpGet("Details/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            var participants = await _eventService.GetParticipantsAsync(id) ?? new List<ParticipantDto>();
            var userId = CurrentUserId;

            var vm = new EventDetailsViewModel
            {
                Event = ev,
                Participants = participants,
                IsAuthenticated = User.Identity?.IsAuthenticated == true,
                IsOrganizer = userId != null && userId == ev.OrganizerId,
                HasJoined = userId != null && participants.Any(p => p.UserId == userId),
                IsFull = ev.CurrentParticipants >= ev.MaxParticipants
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.Organizer)]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(new EventFormViewModel());
        }

        [Authorize(Roles = Roles.Organizer)]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var dto = new EventCreateDto
            {
                Title = vm.Title,
                Description = vm.Description,
                Category = vm.Category,
                StartDateTime = vm.StartDateTime,
                Address = vm.Address,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                ImageUrl = vm.ImageUrl,
                MaxParticipants = vm.MaxParticipants
            };

            var created = await _eventService.CreateAsync(dto, CurrentUserId!);
            TempData["SuccessMessage"] = "Tədbir uğurla yaradıldı.";
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }

        [Authorize(Roles = Roles.Organizer)]
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            if (ev.OrganizerId != CurrentUserId)
            {
                return Forbid();
            }

            var vm = new EventFormViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Category = ev.Category,
                StartDateTime = ev.StartDateTime,
                Address = ev.Address,
                Latitude = ev.Latitude,
                Longitude = ev.Longitude,
                ImageUrl = ev.ImageUrl,
                MaxParticipants = ev.MaxParticipants
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.Organizer)]
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var dto = new EventUpdateDto
            {
                Title = vm.Title,
                Description = vm.Description,
                Category = vm.Category,
                StartDateTime = vm.StartDateTime,
                Address = vm.Address,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                ImageUrl = vm.ImageUrl,
                MaxParticipants = vm.MaxParticipants
            };

            var (found, authorized, result) = await _eventService.UpdateAsync(id, dto, CurrentUserId!);
            if (!found)
            {
                return NotFound();
            }

            if (!authorized)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] = "Tədbir yeniləndi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = Roles.Organizer)]
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (found, authorized) = await _eventService.DeleteAsync(id, CurrentUserId!);
            if (!found)
            {
                return NotFound();
            }

            if (!authorized)
            {
                return Forbid();
            }

            TempData["SuccessMessage"] = "Tədbir silindi.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// AJAX endpoint used by the Details page's "Qoşul" button.
        /// </summary>
        [Authorize]
        [HttpPost("{id:int}/Join")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            var result = await _eventService.JoinEventAsync(id, CurrentUserId!);

            return result switch
            {
                JoinEventResult.Success => Json(new { success = true, message = "Tədbirə uğurla qoşuldunuz." }),
                JoinEventResult.EventNotFound => NotFound(new { success = false, message = "Tədbir tapılmadı." }),
                JoinEventResult.AlreadyJoined => BadRequest(new { success = false, message = "Siz artıq bu tədbirə qoşulmusunuz." }),
                JoinEventResult.EventFull => BadRequest(new { success = false, message = "Bu tədbirdə yer qalmayıb." }),
                JoinEventResult.CannotJoinOwnEvent => BadRequest(new { success = false, message = "Öz tədbirinizə qoşula bilməzsiniz." }),
                _ => BadRequest(new { success = false, message = "Xəta baş verdi." })
            };
        }
    }
}
