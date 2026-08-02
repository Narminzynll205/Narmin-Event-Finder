namespace EventFinder.Web.ViewModels.Chat
{
    /// <summary>
    /// A single entry in the chat sidebar - either an event group chat or a direct conversation.
    /// </summary>
    public class ChatConversationViewModel
    {
        public bool IsEvent { get; set; }

        /// <summary>Event id (when IsEvent) or the other user's id (direct message).</summary>
        public string TargetId { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }

    public class ChatIndexViewModel
    {
        public List<ChatConversationViewModel> Conversations { get; set; } = new();

        /// <summary>Set when a specific event or user thread is currently open.</summary>
        public ChatConversationViewModel? ActiveConversation { get; set; }

        public string CurrentUserId { get; set; } = string.Empty;
    }
}
