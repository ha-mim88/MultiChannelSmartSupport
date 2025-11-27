using frontend.Entities;
using Microsoft.AspNetCore.Identity;

namespace frontend.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public virtual ICollection<AISession> AISessions { get; set; } = new List<AISession>();
}

