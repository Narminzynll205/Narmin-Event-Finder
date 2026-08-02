using EventFinder.Web.DTOs.Events;
using EventFinder.Web.Models;
using EventFinder.Web.Repositories;
using EventFinder.Web.Utils;
using Microsoft.EntityFrameworkCore;

namespace EventFinder.Web.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<List<EventDto>> GetAllAsync(EventFilterDto filter)
        {
            var query = _eventRepository.Query();

            if (filter.Category.HasValue)
            {
                query = query.Where(e => e.Category == filter.Category.Value);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(e => e.StartDateTime >= filter.DateFrom.Value);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(e => e.StartDateTime <= filter.DateTo.Value);
            }

            var events = await query.ToListAsync();

            // Location-based filtering is done in-memory since Haversine distance
            // cannot be translated to SQL directly.
            var useLocationFilter = filter.Lat.HasValue && filter.Lng.HasValue && filter.RadiusKm.HasValue;
            if (useLocationFilter)
            {
                return events
                    .Select(e => new
                    {
                        Event = e,
                        Distance = GeoUtils.HaversineDistanceKm(filter.Lat!.Value, filter.Lng!.Value, e.Latitude, e.Longitude)
                    })
                    .Where(x => x.Distance <= filter.RadiusKm!.Value)
                    .OrderBy(x => x.Distance)
                    .Select(x => MapToDto(x.Event, x.Distance))
                    .ToList();
            }

            return events
                .OrderBy(e => e.StartDateTime)
                .Select(e => MapToDto(e))
                .ToList();
        }

        public async Task<EventDto?> GetByIdAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            return ev == null ? null : MapToDto(ev);
        }

        public async Task<EventDto> CreateAsync(EventCreateDto dto, string organizerId)
        {
            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                StartDateTime = dto.StartDateTime,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                ImageUrl = dto.ImageUrl,
                MaxParticipants = dto.MaxParticipants,
                OrganizerId = organizerId,
                CreatedAt = DateTime.UtcNow
            };

            await _eventRepository.AddAsync(ev);
            await _eventRepository.SaveChangesAsync();

            // Reload with Organizer navigation populated for the response DTO.
            var created = await _eventRepository.GetByIdAsync(ev.Id);
            return MapToDto(created!);
        }

        public async Task<(bool Found, bool Authorized, EventDto? Result)> UpdateAsync(int id, EventUpdateDto dto, string organizerId)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                return (false, true, null);
            }

            if (ev.OrganizerId != organizerId)
            {
                return (true, false, null);
            }

            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.Category = dto.Category;
            ev.StartDateTime = dto.StartDateTime;
            ev.Address = dto.Address;
            ev.Latitude = dto.Latitude;
            ev.Longitude = dto.Longitude;
            ev.ImageUrl = dto.ImageUrl;
            ev.MaxParticipants = dto.MaxParticipants;

            _eventRepository.Update(ev);
            await _eventRepository.SaveChangesAsync();

            return (true, true, MapToDto(ev));
        }

        public async Task<(bool Found, bool Authorized)> DeleteAsync(int id, string organizerId)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                return (false, true);
            }

            if (ev.OrganizerId != organizerId)
            {
                return (true, false);
            }

            _eventRepository.Remove(ev);
            await _eventRepository.SaveChangesAsync();

            return (true, true);
        }

        public async Task<JoinEventResult> JoinEventAsync(int eventId, string userId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null)
            {
                return JoinEventResult.EventNotFound;
            }

            var existingParticipant = await _eventRepository.GetParticipantAsync(eventId, userId);
            if (existingParticipant != null)
            {
                return JoinEventResult.AlreadyJoined;
            }

            var currentCount = await _eventRepository.GetParticipantsCountAsync(eventId);
            if (currentCount >= ev.MaxParticipants)
            {
                return JoinEventResult.EventFull;
            }

            await _eventRepository.AddParticipantAsync(new EventParticipant
            {
                EventId = eventId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            });
            await _eventRepository.SaveChangesAsync();

            return JoinEventResult.Success;
        }

        public async Task<List<ParticipantDto>?> GetParticipantsAsync(int eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null)
            {
                return null;
            }

            var participants = await _eventRepository.GetParticipantsAsync(eventId);
            return participants.Select(p => new ParticipantDto
            {
                UserId = p.UserId,
                FullName = p.User != null ? $"{p.User.FirstName} {p.User.LastName}".Trim() : string.Empty,
                ProfilePictureUrl = p.User?.ProfilePictureUrl,
                JoinedAt = p.JoinedAt
            }).ToList();
        }

        internal static EventDto MapToDto(Event ev, double? distanceKm = null)
        {
            return new EventDto
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Category = ev.Category,
                StartDateTime = ev.StartDateTime,
                Address = ev.Address,
                Latitude = ev.Latitude,
                Longitude = ev.Longitude,
                ImageUrl = ev.ImageUrl,
                MaxParticipants = ev.MaxParticipants,
                CurrentParticipants = ev.Participants?.Count ?? 0,
                OrganizerId = ev.OrganizerId,
                OrganizerName = ev.Organizer != null ? $"{ev.Organizer.FirstName} {ev.Organizer.LastName}".Trim() : string.Empty,
                CreatedAt = ev.CreatedAt,
                DistanceKm = distanceKm
            };
        }
    }
}
