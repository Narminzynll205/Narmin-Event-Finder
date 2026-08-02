using EventFinder.Web.DTOs.Chat;
using EventFinder.Web.Hubs;
using EventFinder.Web.Models;
using EventFinder.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EventFinder.Web.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatService(IChatRepository chatRepository, IHubContext<ChatHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _chatRepository = chatRepository;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<ChatMessageDto> SendMessageAsync(string senderId, SendMessageDto dto)
        {
            if (dto.EventId == null && string.IsNullOrEmpty(dto.ReceiverId))
            {
                throw new ArgumentException("Either ReceiverId or EventId must be provided.");
            }

            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                EventId = dto.EventId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            await _chatRepository.AddAsync(message);
            await _chatRepository.SaveChangesAsync();

            var sender = await _userManager.FindByIdAsync(senderId);
            var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}".Trim() : string.Empty;

            var result = new ChatMessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = senderName,
                ReceiverId = message.ReceiverId,
                EventId = message.EventId,
                Content = message.Content,
                SentAt = message.SentAt
            };


            // Broadcast in real-time via SignalR so REST-based sends also reach connected clients.
            if (dto.EventId.HasValue)
            {
                await _hubContext.Clients.Group($"event-{dto.EventId.Value}").SendAsync("ReceiveMessage", result);
            }
            else if (!string.IsNullOrEmpty(dto.ReceiverId))
            {
                await _hubContext.Clients.User(dto.ReceiverId).SendAsync("ReceiveMessage", result);
            }

            return result;
        }

        public async Task<List<ChatMessageDto>> GetDirectHistoryAsync(string userId1, string userId2)
        {
            var messages = await _chatRepository.GetDirectHistoryAsync(userId1, userId2);
            return messages.Select(MapToDto).ToList();
        }

        public async Task<List<ChatMessageDto>> GetEventHistoryAsync(int eventId)
        {
            var messages = await _chatRepository.GetEventHistoryAsync(eventId);
            return messages.Select(MapToDto).ToList();
        }

        public Task<List<string>> GetDirectContactIdsAsync(string userId)
        {
            return _chatRepository.GetDirectContactIdsAsync(userId);
        }

        private static ChatMessageDto MapToDto(ChatMessage m) => new()
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender != null ? $"{m.Sender.FirstName} {m.Sender.LastName}".Trim() : string.Empty,
            ReceiverId = m.ReceiverId,
            EventId = m.EventId,
            Content = m.Content,
            SentAt = m.SentAt
        };
    }
}
