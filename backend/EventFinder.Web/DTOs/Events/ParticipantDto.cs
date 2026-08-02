namespace EventFinder.Web.DTOs.Events
{
    public class ParticipantDto
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
