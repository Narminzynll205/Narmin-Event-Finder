using EventFinder.Web.DTOs.Events;
using EventFinder.Web.Models;
using EventFinder.Web.Repositories;
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

        public async Task<List<EventDto>> GetAllAsync()
        {
            var events = await _eventRepository.Query().ToListAsync();
            return events.Select(e => MapToDto(e)).ToList();
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
