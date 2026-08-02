using System.ComponentModel.DataAnnotations;
using EventFinder.Web.Models;

namespace EventFinder.Web.DTOs.Events
{
    public class EventCreateDto
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public EventCategory Category { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        public string? ImageUrl { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxParticipants { get; set; } = 1;
    }
}
