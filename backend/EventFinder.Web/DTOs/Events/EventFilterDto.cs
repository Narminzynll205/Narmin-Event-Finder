using EventFinder.Web.Models;

namespace EventFinder.Web.DTOs.Events
{
    /// <summary>
    /// Query filter options for searching events (all optional).
    /// </summary>
    public class EventFilterDto
    {
        public EventCategory? Category { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Requester's latitude. Required together with Lng and RadiusKm to filter by proximity.
        /// </summary>
        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public double? RadiusKm { get; set; }
    }
}
