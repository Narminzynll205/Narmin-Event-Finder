namespace EventFinder.Web.DTOs.Users
{
    public class NearbyUserDto
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }

        public double DistanceKm { get; set; }

        public DateTime? LastLocationUpdate { get; set; }
    }
}
