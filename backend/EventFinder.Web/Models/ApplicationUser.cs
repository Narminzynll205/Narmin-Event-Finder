using Microsoft.AspNetCore.Identity;

namespace EventFinder.Web.Models
{
    /// <summary>
    /// Application user extending ASP.NET Core Identity's IdentityUser.
    /// A user can hold both the Organizer and Participant roles.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// Current latitude of the user, used for the "nearby users" feature.
        /// </summary>
        public double? CurrentLat { get; set; }

        /// <summary>
        /// Current longitude of the user, used for the "nearby users" feature.
        /// </summary>
        public double? CurrentLng { get; set; }

        /// <summary>
        /// Timestamp of the last time the user's location was updated.
        /// </summary>
        public DateTime? LastLocationUpdate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();

        public ICollection<EventParticipant> ParticipatedEvents { get; set; } = new List<EventParticipant>();
    }
}
