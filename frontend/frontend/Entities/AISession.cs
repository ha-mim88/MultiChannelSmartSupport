using frontend.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace frontend.Entities
{
    public class AISession
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public bool Removed { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<AIChatHistory> ChatHistories { get; set; } = new List<AIChatHistory>();
    }
}
