namespace EventFinder.Web.DTOs.Chat
{
    public class ChatMessageDto
    {
        public int Id { get; set; }

        public string SenderId { get; set; } = string.Empty;

        public string SenderName { get; set; } = string.Empty;

        public string? ReceiverId { get; set; }

        public int? EventId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }
    }
}
