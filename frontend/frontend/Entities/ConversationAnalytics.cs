namespace frontend.Entities
{
    public class ConversationAnalytics
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int MessageCount { get; set; }
        public int ToolCalls { get; set; }
        public bool Resolved { get; set; } = true;   // Assume true unless handoff
        public bool HandoffToHuman { get; set; } = false;
        public double? SatisfactionScore { get; set; } // Future: 5-star
    }
}
