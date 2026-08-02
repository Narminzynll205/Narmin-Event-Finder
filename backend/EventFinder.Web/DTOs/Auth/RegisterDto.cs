using System.ComponentModel.DataAnnotations;

namespace EventFinder.Web.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Roles to assign to the new user. Defaults to Participant if empty.
        /// Valid values: "Organizer", "Participant".
        /// </summary>
        public List<string> Roles { get; set; } = new();
    }
}
