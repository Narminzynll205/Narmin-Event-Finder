using EventFinder.Web.Models;

namespace EventFinder.Web.Repositories
{
    public interface IChatRepository
    {
        Task AddAsync(ChatMessage message);

        Task<bool> SaveChangesAsync();

        /// <summary>
        /// Direct message history between two users, ordered by SentAt ascending.
        /// </summary>
        Task<List<ChatMessage>> GetDirectHistoryAsync(string userId1, string userId2);

        /// <summary>
        /// Group chat history for a given event, ordered by SentAt ascending.
        /// </summary>
        Task<List<ChatMessage>> GetEventHistoryAsync(int eventId);
    }
}
