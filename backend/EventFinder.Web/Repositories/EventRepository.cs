using EventFinder.Web.Data;
using EventFinder.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EventFinder.Web.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Event> Query()
        {
            return _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .AsQueryable();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Event ev)
        {
            await _context.Events.AddAsync(ev);
        }

        public void Update(Event ev)
        {
            _context.Events.Update(ev);
        }

        public void Remove(Event ev)
        {
            _context.Events.Remove(ev);
        }

        public async Task<EventParticipant?> GetParticipantAsync(int eventId, string userId)
        {
            return await _context.EventParticipants
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);
        }

        public async Task<List<EventParticipant>> GetParticipantsAsync(int eventId)
        {
            return await _context.EventParticipants
                .Include(p => p.User)
                .Where(p => p.EventId == eventId)
                .ToListAsync();
        }

        public async Task AddParticipantAsync(EventParticipant participant)
        {
            await _context.EventParticipants.AddAsync(participant);
        }

        public async Task<int> GetParticipantsCountAsync(int eventId)
        {
            return await _context.EventParticipants.CountAsync(p => p.EventId == eventId);
        }

        public async Task<List<Event>> GetOrganizedByUserAsync(string userId)
        {
            return await Query()
                .Where(e => e.OrganizerId == userId)
                .OrderByDescending(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<List<Event>> GetJoinedByUserAsync(string userId)
        {
            return await Query()
                .Where(e => e.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
