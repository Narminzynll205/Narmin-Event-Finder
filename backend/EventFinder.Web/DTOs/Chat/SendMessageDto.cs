using System.ComponentModel.DataAnnotations;

namespace EventFinder.Web.DTOs.Chat
{
    /// <summary>
    /// Payload for sending a chat message. Provide either ReceiverId (direct message)
    /// or EventId (event group chat), not both.
    /// </summary>
    public class SendMessageDto
    {
        public string? ReceiverId { get; set; }

        public int? EventId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;
    }
}
