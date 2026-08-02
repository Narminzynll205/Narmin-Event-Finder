using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventFinder.Web.Models
{
    /// <summary>
    /// Represents a chat message. Supports either a direct message (ReceiverId set)
    /// or an event group chat message (EventId set).
    /// </summary>
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser? Sender { get; set; }

        /// <summary>
        /// Set when this is a direct (1-to-1) message.
        /// </summary>
        public string? ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public ApplicationUser? Receiver { get; set; }

        /// <summary>
        /// Set when this is a group chat message tied to an event.
        /// </summary>
        public int? EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
