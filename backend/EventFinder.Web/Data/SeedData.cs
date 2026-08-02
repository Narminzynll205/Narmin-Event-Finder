using EventFinder.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace EventFinder.Web.Data
{
    /// <summary>
    /// Seeds the database with roles and sample data for local development/testing.
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            foreach (var role in new[] { Roles.Organizer, Roles.Participant })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var organizer = await userManager.FindByEmailAsync("organizer@eventfinder.dev");
            if (organizer == null)
            {
                organizer = new ApplicationUser
                {
                    UserName = "organizer@eventfinder.dev",
                    Email = "organizer@eventfinder.dev",
                    FirstName = "Aysel",
                    LastName = "Organizer",
                    EmailConfirmed = true,
                    CurrentLat = 40.3777,
                    CurrentLng = 49.8920,
                    LastLocationUpdate = DateTime.UtcNow
                };
                await userManager.CreateAsync(organizer, "Passw0rd!");
                await userManager.AddToRolesAsync(organizer, new[] { Roles.Organizer, Roles.Participant });
            }

            var participant = await userManager.FindByEmailAsync("participant@eventfinder.dev");
            if (participant == null)
            {
                participant = new ApplicationUser
                {
                    UserName = "participant@eventfinder.dev",
                    Email = "participant@eventfinder.dev",
                    FirstName = "Kamran",
                    LastName = "Participant",
                    EmailConfirmed = true,
                    CurrentLat = 40.3900,
                    CurrentLng = 49.8700,
                    LastLocationUpdate = DateTime.UtcNow
                };
                await userManager.CreateAsync(participant, "Passw0rd!");
                await userManager.AddToRoleAsync(participant, Roles.Participant);
            }

            if (!context.Events.Any())
            {
                context.Events.AddRange(
                    new Event
                    {
                        Title = "Baku Tech Meetup",
                        Description = "A community meetup for developers to share knowledge and network.",
                        Category = EventCategory.Technology,
                        StartDateTime = DateTime.UtcNow.AddDays(7),
                        Address = "28 May, Baku, Azerbaijan",
                        Latitude = 40.3777,
                        Longitude = 49.8920,
                        MaxParticipants = 100,
                        OrganizerId = organizer.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Title = "Open Air Music Festival",
                        Description = "Live music from local bands in the city park.",
                        Category = EventCategory.Music,
                        StartDateTime = DateTime.UtcNow.AddDays(14),
                        Address = "Sahil Park, Baku, Azerbaijan",
                        Latitude = 40.3650,
                        Longitude = 49.8360,
                        MaxParticipants = 500,
                        OrganizerId = organizer.Id,
                        CreatedAt = DateTime.UtcNow
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
