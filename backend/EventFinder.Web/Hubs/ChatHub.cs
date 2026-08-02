using System.Security.Claims;
using EventFinder.Web.DTOs.Chat;
using EventFinder.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EventFinder.Web.Hubs
{
    /// <summary>
    /// Real-time chat hub. Supports direct (1-to-1) messaging and event group chat.
    /// Clients connect with a JWT access_token (?access_token=...) since browser
    /// WebSocket/SSE transports cannot set the Authorization header.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        private string CurrentUserId => Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new HubException("User id claim not found.");

        /// <summary>
        /// Joins the SignalR group for a specific event's group chat.
        /// </summary>
        public async Task JoinEventGroup(int eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(eventId));
        }

        public async Task LeaveEventGroup(int eventId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(eventId));
        }

        /// <summary>
        /// Sends a message (direct or event group chat) and persists it. Broadcasting to the
        /// event group / receiver is handled inside <see cref="IChatService.SendMessageAsync"/>
        /// (shared with the REST endpoint), so it must NOT be repeated here to avoid duplicate
        /// messages being delivered to clients.
        /// </summary>
        public async Task SendMessage(SendMessageDto dto)
        {
            var message = await _chatService.SendMessageAsync(CurrentUserId, dto);

            // For direct messages, the service only notifies the receiver - push the message
            // back to the sender's own connection too so they see it immediately.
            if (!dto.EventId.HasValue && !string.IsNullOrEmpty(dto.ReceiverId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message);
            }
        }

        private static string GroupName(int eventId) => $"event-{eventId}";
    }
}
