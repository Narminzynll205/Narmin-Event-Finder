using EventFinder.Web.DTOs.Events;
using EventFinder.Web.Models;

namespace EventFinder.Web.ViewModels.Events
{
    /// <summary>
    /// Backs the Events list page: holds the current filter selection plus the matching results.
    /// </summary>
    public class EventsIndexViewModel
    {
        public EventCategory? Category { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public double? RadiusKm { get; set; }

        public List<EventDto> Events { get; set; } = new();
    }
}
