using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class Student : ApplicationUser
    {
        StudentDbContext context = new StudentDbContext();
        public int? Ratings { get; set; }
        
        public override string Discriminator { get; }

        public List<Student> GetStudentList()
        {
            return context.Users.ToList();
        }
    }

    public class StudentDbContext : IdentityDbContext<Student>
    {
        public StudentDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static StudentDbContext Create()
        {
            return new StudentDbContext();
        }
    }
}