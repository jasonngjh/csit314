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

        public System.Data.Entity.DbSet<CSIT314BCE.Models.Post> Posts { get; set; }
        public System.Data.Entity.DbSet<CSIT314BCE.Models.Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           /* modelBuilder.Entity<Post>().ToTable("Posts");
            modelBuilder.Entity<Comment>().ToTable("Comments");*/
            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<CSIT314BCE.Models.Student> ApplicationUsers { get; set; }
    }
}