using System.ComponentModel.DataAnnotations;

namespace EventFinder.Web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad tələb olunur.")]
        [MaxLength(100)]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad tələb olunur.")]
        [MaxLength(100)]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail tələb olunur.")]
        [EmailAddress(ErrorMessage = "Düzgün e-mail daxil edin.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə tələb olunur.")]
        [MinLength(6, ErrorMessage = "Şifrə ən azı 6 simvol olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifrə")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Organizer (tədbir yarada bilər)")]
        public bool IsOrganizer { get; set; }

        [Display(Name = "Participant (tədbirlərə qoşula bilər)")]
        public bool IsParticipant { get; set; } = true;
    }
}
