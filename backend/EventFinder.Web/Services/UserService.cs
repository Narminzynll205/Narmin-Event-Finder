using EventFinder.Web.Data;
using EventFinder.Web.DTOs.Users;
using EventFinder.Web.Utils;
using Microsoft.EntityFrameworkCore;

namespace EventFinder.Web.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NearbyUserDto>> GetNearbyUsersAsync(string currentUserId, double lat, double lng, double radiusKm)
        {
            var candidates = await _context.Users
                .Where(u => u.Id != currentUserId && u.CurrentLat != null && u.CurrentLng != null)
                .ToListAsync();

            return candidates
                .Select(u => new
                {
                    User = u,
                    Distance = GeoUtils.HaversineDistanceKm(lat, lng, u.CurrentLat!.Value, u.CurrentLng!.Value)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Select(x => new NearbyUserDto
                {
                    UserId = x.User.Id,
                    FullName = $"{x.User.FirstName} {x.User.LastName}".Trim(),
                    ProfilePictureUrl = x.User.ProfilePictureUrl,
                    DistanceKm = x.Distance,
                    LastLocationUpdate = x.User.LastLocationUpdate
                })
                .ToList();
        }

        public async Task<bool> UpdateLocationAsync(string userId, double lat, double lng)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.CurrentLat = lat;
            user.CurrentLng = lng;
            user.LastLocationUpdate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
