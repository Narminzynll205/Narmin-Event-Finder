using System.ComponentModel.DataAnnotations;
using EventFinder.Web.Models;

namespace EventFinder.Web.ViewModels.Events
{
    /// <summary>
    /// Shared form model for both Create and Edit event pages.
    /// </summary>
    public class EventFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad tələb olunur.")]
        [MaxLength(150)]
        [Display(Name = "Ad")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Təsvir tələb olunur.")]
        [Display(Name = "Təsvir")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Kateqoriya")]
        public EventCategory Category { get; set; }

        [Required(ErrorMessage = "Tarix/saat tələb olunur.")]
        [Display(Name = "Tarix və saat")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Ünvan tələb olunur.")]
        [MaxLength(300)]
        [Display(Name = "Ünvan")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xəritədən yer seçin.")]
        [Range(-90, 90)]
        [Display(Name = "Enlik (Latitude)")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Xəritədən yer seçin.")]
        [Range(-180, 180)]
        [Display(Name = "Uzunluq (Longitude)")]
        public double Longitude { get; set; }

        [Display(Name = "Şəkil URL")]
        public string? ImageUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Ən azı 1 iştirakçı yeri olmalıdır.")]
        [Display(Name = "Maksimum iştirakçı sayı")]
        public int MaxParticipants { get; set; } = 20;
    }
}
