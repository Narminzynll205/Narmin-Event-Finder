namespace EventFinder.Web.DTOs.Users
{
    public class NearbyUserDto
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }

        public double DistanceKm { get; set; }

        public DateTime? LastLocationUpdate { get; set; }

        /// <summary>
        /// Last known coordinates, exposed only to authenticated nearby-search callers
        /// so the "Nearby users" map can plot a marker for this user.
        /// </summary>
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
