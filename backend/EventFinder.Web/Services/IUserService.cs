using EventFinder.Web.DTOs.Users;

namespace EventFinder.Web.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Finds users (excluding the requester) within radiusKm of the given coordinates,
        /// based on their last known location. Uses the Haversine formula for distance.
        /// </summary>
        Task<List<NearbyUserDto>> GetNearbyUsersAsync(string currentUserId, double lat, double lng, double radiusKm);

        /// <summary>
        /// Updates the current user's last known coordinates and timestamp.
        /// Returns false if the user does not exist.
        /// </summary>
        Task<bool> UpdateLocationAsync(string userId, double lat, double lng);
    }
}
