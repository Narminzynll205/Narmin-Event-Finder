using EventFinder.Web.Data;
using EventFinder.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EventFinder.Web.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<List<ChatMessage>> GetDirectHistoryAsync(string userId1, string userId2)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.EventId == null &&
                    ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                     (m.SenderId == userId2 && m.ReceiverId == userId1)))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<ChatMessage>> GetEventHistoryAsync(int eventId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.EventId == eventId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
