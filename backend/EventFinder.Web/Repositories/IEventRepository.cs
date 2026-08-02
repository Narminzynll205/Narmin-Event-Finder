using EventFinder.Web.Models;

namespace EventFinder.Web.Repositories
{
    /// <summary>
    /// Data access abstraction for Event and EventParticipant entities.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Returns a queryable of events with the Organizer and Participants included,
        /// intended for building further filters (category, date range, etc.) in the service layer.
        /// </summary>
        IQueryable<Event> Query();

        Task<Event?> GetByIdAsync(int id);

        Task AddAsync(Event ev);

        void Update(Event ev);

        void Remove(Event ev);

        Task<EventParticipant?> GetParticipantAsync(int eventId, string userId);

        Task<List<EventParticipant>> GetParticipantsAsync(int eventId);

        Task AddParticipantAsync(EventParticipant participant);

        Task<int> GetParticipantsCountAsync(int eventId);

        Task<bool> SaveChangesAsync();
    }
}
