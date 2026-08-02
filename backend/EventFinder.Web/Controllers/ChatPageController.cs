using System.Security.Claims;
using EventFinder.Web.Models;
using EventFinder.Web.Services;
using EventFinder.Web.ViewModels.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventFinder.Web.Controllers
{
    /// <summary>
    /// Chat shell page: sidebar of event/direct conversations plus the active thread.
    /// Real-time delivery happens over SignalR (see ChatHub); this controller only
    /// renders the shell and exposes a JSON history endpoint for the JS client to load.
    /// </summary>
    [Authorize]
    [Route("Chat")]
    public class ChatPageController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IChatService _chatService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatPageController(IEventService eventService, IChatService chatService, UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _chatService = chatService;
            _userManager = userManager;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var vm = new ChatIndexViewModel
            {
                Conversations = await BuildConversationsAsync(),
                CurrentUserId = CurrentUserId
            };

            return View(vm);
        }

        [HttpGet("Event/{eventId:int}")]
        public async Task<IActionResult> Event(int eventId)
        {
            var ev = await _eventService.GetByIdAsync(eventId);
            if (ev == null)
            {
                return NotFound();
            }

            var participants = await _eventService.GetParticipantsAsync(eventId) ?? new();
            var allowed = ev.OrganizerId == CurrentUserId || participants.Any(p => p.UserId == CurrentUserId);
            if (!allowed)
            {
                return Forbid();
            }

            var vm = new ChatIndexViewModel
            {
                Conversations = await BuildConversationsAsync(),
                ActiveConversation = new ChatConversationViewModel { IsEvent = true, TargetId = eventId.ToString(), DisplayName = ev.Title },
                CurrentUserId = CurrentUserId
            };

            return View("Index", vm);
        }

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> Direct(string userId)
        {
            if (userId == CurrentUserId)
            {
                return BadRequest();
            }

            var otherUser = await _userManager.FindByIdAsync(userId);
            if (otherUser == null)
            {
                return NotFound();
            }

            var conversations = await BuildConversationsAsync();
            var displayName = $"{otherUser.FirstName} {otherUser.LastName}".Trim();
            if (!conversations.Any(c => !c.IsEvent && c.TargetId == userId))
            {
                conversations.Insert(0, new ChatConversationViewModel { IsEvent = false, TargetId = userId, DisplayName = displayName });
            }

            var vm = new ChatIndexViewModel
            {
                Conversations = conversations,
                ActiveConversation = new ChatConversationViewModel { IsEvent = false, TargetId = userId, DisplayName = displayName },
                CurrentUserId = CurrentUserId
            };

            return View("Index", vm);
        }

        /// <summary>AJAX: loads message history for the currently open thread.</summary>
        [HttpGet("History")]
        public async Task<IActionResult> History(string type, string id)
        {
            if (type.Equals("event", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(id, out var eventId))
                {
                    return BadRequest();
                }

                return Json(await _chatService.GetEventHistoryAsync(eventId));
            }

            return Json(await _chatService.GetDirectHistoryAsync(CurrentUserId, id));
        }

        private async Task<List<ChatConversationViewModel>> BuildConversationsAsync()
        {
            var organized = await _eventService.GetOrganizedByUserAsync(CurrentUserId);
            var joined = await _eventService.GetJoinedByUserAsync(CurrentUserId);

            var eventConversations = organized.Concat(joined)
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .OrderBy(e => e.Title)
                .Select(e => new ChatConversationViewModel { IsEvent = true, TargetId = e.Id.ToString(), DisplayName = e.Title, ImageUrl = e.ImageUrl })
                .ToList();

            var contactIds = await _chatService.GetDirectContactIdsAsync(CurrentUserId);
            var directConversations = new List<ChatConversationViewModel>();
            foreach (var contactId in contactIds)
            {
                var contact = await _userManager.FindByIdAsync(contactId);
                if (contact == null) continue;

                directConversations.Add(new ChatConversationViewModel
                {
                    IsEvent = false,
                    TargetId = contactId,
                    DisplayName = $"{contact.FirstName} {contact.LastName}".Trim(),
                    ImageUrl = contact.ProfilePictureUrl
                });
            }

            var result = new List<ChatConversationViewModel>();
            result.AddRange(eventConversations);
            result.AddRange(directConversations.OrderBy(c => c.DisplayName));
            return result;
        }
    }
}
