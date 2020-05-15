using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSIT314BCE.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        [Required]
        [ForeignKey("Student")]
        public string OwnerId { get; set; }
        public virtual Student Student { get; set; }

        [Required]
        public string OwnerUsername { get; set; }

        private ApplicationDbContext context = new ApplicationDbContext();

        public int PostComment(PostCommentViewModel model) 
        {
            var userManager = new UserManager<Student>(new UserStore<Student>(context));
            var user = userManager.FindById(model.OwnerId);

            if (user != null)
            {
                Comment comment = new Comment() {
                    PostId = model.PostId,
                    Text = model.Text,
                    CreationDate = DateTime.Now,
                    OwnerId = user.Id,
                    OwnerUsername = user.UserName
                };

                context.Comments.Add(comment);
                context.SaveChanges();

                /*using (var db = new ApplicationDbContext())
                {
                    Post post = context.Posts.Find(model.PostId);
                    if (post != null)
                    {
                        post.CommentCount += 1;
                        db.SaveChanges();
                    }
                        
                }*/

                return model.PostId;
            }   
            return 0;
        }
    }
}