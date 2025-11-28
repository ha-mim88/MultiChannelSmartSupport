using frontend.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace frontend.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<AISession> AISession => Set<AISession>();
    public DbSet<AIChatHistory> AIChatHistory => Set<AIChatHistory>();
    public DbSet<ConversationAnalytics> ConversationAnalytics => Set<ConversationAnalytics>();

}
