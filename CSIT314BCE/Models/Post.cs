using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public bool Create(AskViewModel model) {
            var userManager = new UserManager<Student>(new UserStore<Student>(context));
            var user = userManager.FindById(model.OwnerId);

            if (user == null) {
                return false;
            }

            Post post = new Post {
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
            return true;
        }
    }
}