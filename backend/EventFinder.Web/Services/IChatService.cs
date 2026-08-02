using EventFinder.Web.DTOs.Chat;

namespace EventFinder.Web.Services
{
    public interface IChatService
    {
        Task<ChatMessageDto> SendMessageAsync(string senderId, SendMessageDto dto);

        Task<List<ChatMessageDto>> GetDirectHistoryAsync(string userId1, string userId2);

        Task<List<ChatMessageDto>> GetEventHistoryAsync(int eventId);
    }
}
