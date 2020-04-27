namespace CSIT314BCE.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AspNetUserDBModel : DbContext
    {
        public AspNetUserDBModel()
            : base("name=AspNetUserDBModel")
        {
        }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUser>()
                .Property(e => e.FullName)
                .IsUnicode(false);
        }
    }
}
