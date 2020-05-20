using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class Moderator : ApplicationUser
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Post> RetrieveMostAnsweredQuestions() 
        {
            return db.Posts.Where(g => g.Title != null).OrderByDescending(p => p.AnswerCount).ToList();
        }

        public List<Post> RetrieveMostAnsweredQuestions(DateTime FromDate, DateTime ToDate)
        {
            return db.Posts.Where(g => g.Title != null && g.CreationDate >= FromDate && g.CreationDate < ToDate).OrderByDescending(p => p.AnswerCount).ToList();
        }

        public List<Post> RetrieveMostVotedQuestions()
        {
            return db.Posts.Where(g => g.Title != null).OrderByDescending(p => p.VoteCount).ToList();
        }

        public List<Post> RetrieveMostVotedQuestions(DateTime FromDate, DateTime ToDate)
        {
            return db.Posts.Where(g => g.Title != null && g.CreationDate >= FromDate && g.CreationDate < ToDate).OrderByDescending(p => p.VoteCount).ToList();
        }

        public List<Post> RetrieveMostCommentedQuestion()
        {
            return db.Posts.Where(g => g.Title != null).OrderByDescending(p => p.CommentCount).ToList();
        }

        public List<Post> RetrieveMostCommentedQuestion(DateTime FromDate, DateTime ToDate)
        {
            return db.Posts.Where(g => g.Title != null && g.CreationDate >= FromDate && g.CreationDate < ToDate).OrderByDescending(p => p.CommentCount).ToList();
        }

        public List<Student> RetrieveTopStudents() 
        {
            return db.ApplicationUsers.Where(g => g.Id != null).OrderByDescending(p => p.Ratings).ToList();
        }
    }
}