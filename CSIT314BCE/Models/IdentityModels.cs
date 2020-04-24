using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CSIT314BCE.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
    }
}