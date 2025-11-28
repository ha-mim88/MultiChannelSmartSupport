using System.ComponentModel.DataAnnotations.Schema;

namespace frontend.Entities
{
    public class AIChatHistory
    {
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TokenUsage { get; set; } = 0;
        public bool Removed { get; set; }


        [ForeignKey("AISession")]
        public int AISessionId { get; set; }
        public virtual AISession? AISession { get; set; }
    }
}
