using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class Student : ApplicationUser
    {
        public int? Ratings { get; set; }

        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Post> RetrieveMyQuestions(string userId) 
        {
            return db.Posts.Where(g => g.OwnerId == userId && g.Title != null).ToList();
        }

        public List<Post> RetrieveMyAnswers(string userId)
        {
            return db.Posts.Where(g => g.OwnerId == userId && g.Title == null).ToList();
        }

        public int? GetRating(string userId) 
        {
            return db.ApplicationUsers.Where(x => x.Id == userId).FirstOrDefault().Ratings;
        }
    }
}