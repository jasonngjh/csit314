using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CSIT314BCE.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public int AcceptedAnswerId { get; set; }
        public int ParentId { get; set; } //Id of Original Post
        public int VoteCount { get; set; }
        public int AnswerCount { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        [Required]
        [ForeignKey("Student")]
        public string OwnerId { get; set; }
        public virtual Student Student { get; set; }

        [Required]
        public string OwnerUsername { get; set; }

        private ApplicationDbContext context = new ApplicationDbContext();

        [NotMapped]
        public List<Comment> comments { get; set; }

        public int Create(AskViewModel model)
        {
            var userManager = new UserManager<Student>(new UserStore<Student>(context));
            var user = userManager.FindById(model.OwnerId);

            if (user == null)
            {
                return 0;
            }

            Post post = new Post
            {
                Title = model.Title,
                Body = model.Body,
                OwnerId = user.Id,
                OwnerUsername = user.UserName,
                CreationDate = DateTime.Now,
                VoteCount = 0,
                AnswerCount = 0,
            };

            context.Posts.Add(post);
            context.SaveChanges();
            context.Entry(post).GetDatabaseValues();

            return post.PostId;
        }

        public DetailsViewModel GetDetails(int id)
        {
            DetailsViewModel model = new DetailsViewModel();
            Post question = context.Posts.Find(id);
            var questionComments = context.Comments.Where(c => c.PostId.Equals(id)).ToList();
            question.comments = questionComments;
            model.Question = question;

            var answers = context.Posts.Where(p => p.ParentId.Equals(id)).OrderBy(p => p.CreationDate).ToList();
            foreach (Post a in answers)
            {
                var answerComments = context.Comments.Where(c => c.PostId.Equals(a.PostId)).ToList();
                a.comments = answerComments;
            }
            model.Answer = answers;
            return model;
        }

        public void PostAns(string body, int PostId, string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = userManager.FindById(userId);

            Post post = new Post()
            {
                ParentId = PostId,
                Body = body,
                CreationDate = DateTime.Now,
                OwnerId = user.Id,
                OwnerUsername = user.UserName,
                VoteCount = 0
            };

            context.Posts.Add(post);
            context.SaveChanges();
        }
    }
}