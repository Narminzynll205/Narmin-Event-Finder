using System.ComponentModel.DataAnnotations;

namespace EventFinder.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail tələb olunur.")]
        [EmailAddress(ErrorMessage = "Düzgün e-mail daxil edin.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə tələb olunur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifrə")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Məni yadda saxla")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
