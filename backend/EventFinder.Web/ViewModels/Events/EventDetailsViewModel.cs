using EventFinder.Web.DTOs.Events;

namespace EventFinder.Web.ViewModels.Events
{
    /// <summary>
    /// Everything the Event Details page needs: the event itself, its participants,
    /// and flags describing the current viewer's relationship to the event.
    /// </summary>
    public class EventDetailsViewModel
    {
        public EventDto Event { get; set; } = null!;

        public List<ParticipantDto> Participants { get; set; } = new();

        public bool IsAuthenticated { get; set; }

        public bool IsOrganizer { get; set; }

        public bool HasJoined { get; set; }

        public bool IsFull { get; set; }
    }
}
