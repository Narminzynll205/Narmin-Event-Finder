using EventFinder.Web.DTOs.Events;

namespace EventFinder.Web.Services
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllAsync(EventFilterDto filter);

        Task<EventDto?> GetByIdAsync(int id);

        Task<EventDto> CreateAsync(EventCreateDto dto, string organizerId);

        /// <summary>
        /// Updates the event. Returns null if the event does not exist,
        /// or false-success with a Forbidden reason if the caller is not the organizer.
        /// </summary>
        Task<(bool Found, bool Authorized, EventDto? Result)> UpdateAsync(int id, EventUpdateDto dto, string organizerId);

        Task<(bool Found, bool Authorized)> DeleteAsync(int id, string organizerId);

        Task<JoinEventResult> JoinEventAsync(int eventId, string userId);

        Task<List<ParticipantDto>?> GetParticipantsAsync(int eventId);

        Task<List<EventDto>> GetOrganizedByUserAsync(string userId);

        Task<List<EventDto>> GetJoinedByUserAsync(string userId);
    }
}
