using EventFinder.Web.DTOs.Events;

namespace EventFinder.Web.ViewModels.Account
{
    /// <summary>
    /// Profile page data: basic info plus the user's organized and joined events.
    /// </summary>
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public IList<string> Roles { get; set; } = new List<string>();

        public List<EventDto> OrganizedEvents { get; set; } = new();

        public List<EventDto> JoinedEvents { get; set; } = new();
    }
}
