using System.Security.Claims;
using EventFinder.Web.DTOs.Chat;
using EventFinder.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFinder.Web.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id claim not found.");

        /// <summary>
        /// Sends a chat message (direct or event group chat) over REST. Also broadcasts
        /// the message in real-time to connected SignalR clients (see /hubs/chat).
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.EventId == null && string.IsNullOrEmpty(dto.ReceiverId))
            {
                return BadRequest(new { message = "Either ReceiverId or EventId must be provided." });
            }

            var message = await _chatService.SendMessageAsync(CurrentUserId, dto);
            return StatusCode(StatusCodes.Status201Created, message);
        }

        /// <summary>
        /// Returns chat history. Use type=direct with a userId, or type=event with an eventId.
        /// </summary>
        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetHistory(string id, [FromQuery] string type = "direct")
        {
            if (type.Equals("event", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(id, out var eventId))
                {
                    return BadRequest(new { message = "id must be a valid eventId when type=event." });
                }

                var eventHistory = await _chatService.GetEventHistoryAsync(eventId);
                return Ok(eventHistory);
            }

            var directHistory = await _chatService.GetDirectHistoryAsync(CurrentUserId, id);
            return Ok(directHistory);
        }
    }
}
