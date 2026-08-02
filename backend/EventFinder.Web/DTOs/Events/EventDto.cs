using EventFinder.Web.Models;

namespace EventFinder.Web.DTOs.Events
{
    public class EventDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public EventCategory Category { get; set; }

        public DateTime StartDateTime { get; set; }

        public string Address { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? ImageUrl { get; set; }

        public int MaxParticipants { get; set; }

        public int CurrentParticipants { get; set; }

        public string OrganizerId { get; set; } = string.Empty;

        public string OrganizerName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Distance in kilometers from the requester's location, when a location filter was supplied.
        /// Null when no location filter was used.
        /// </summary>
        public double? DistanceKm { get; set; }
    }
}
