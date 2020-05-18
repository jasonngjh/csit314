using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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
        public int CommentCount { get; set; }
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
            user.Ratings += 5;
            context.SaveChanges();
            context.Entry(post).GetDatabaseValues();

            return post.PostId;
        }

        public int PostAns(PostAnsViewModel model) 
        {
            var userManager = new UserManager<Student>(new UserStore<Student>(context));
            var user = userManager.FindById(model.OwnerId);

            if (user == null)
            {
                return 0;
            }

            Post question = context.Posts.Find(model.ParentId);

            Post post = new Post() 
            {
                Body = model.Body,
                ParentId = model.ParentId,
                OwnerId = user.Id,
                OwnerUsername = user.UserName,
                CreationDate = DateTime.Now,
                VoteCount = 0
            };

            context.Posts.Add(post);
            user.Ratings += 5;
            question.AnswerCount += 1;
            context.SaveChanges();
            context.Entry(post).GetDatabaseValues();

            return post.ParentId;
        }

        public DetailsViewModel GetDetails(int id)
        {
            DetailsViewModel model = new DetailsViewModel();
            Post question = context.Posts.Find(id);
            var questionComments = context.Comments.Where(c => c.PostId.Equals(id)).ToList();
            question.comments = questionComments;
            model.Question = question;

            var answers = context.Posts
                .Where(p => p.ParentId.Equals(id))
                .OrderByDescending(p => p.VoteCount)
                .ToList();
            
            if(question.AcceptedAnswerId != 0) 
            {
                Post acceptedAns = new Post();
                foreach (Post ans in answers)
                {
                    if (ans.PostId == question.AcceptedAnswerId) 
                    {
                        acceptedAns = ans;
                    }
                }
                answers.Remove(acceptedAns);
                answers.Insert(0, acceptedAns);
            }

            foreach (Post a in answers)
            {
                var answerComments = context.Comments.Where(c => c.PostId.Equals(a.PostId)).ToList();
                a.comments = answerComments;
            }
            model.Answer = answers;
            return model;
        }

        private int BodyLimit = 300;
        [Display(Name = "Body")]
        public string BodyTrimmed
        {
            get
            {
                if (this.Body.Length > this.BodyLimit)
                    return this.Body.Substring(0, this.BodyLimit) + "...";
                else
                    return this.Body;
            }
        }

        public int ChooseAnswer(int postId)
        {
            Post answer = context.Posts.Find(postId);
            var userManager = new UserManager<Student>(new UserStore<Student>(context));
            if (answer != null)
            {
                using (var db = new ApplicationDbContext())
                {
                    Post question = db.Posts.Find(answer.ParentId);
                    if (question != null)
                    {
                        question.AcceptedAnswerId = postId;
                        var user = userManager.FindById(answer.OwnerId);
                        user.Ratings += 10;
                        question.ClosedDate = DateTime.Now;
                        context.SaveChanges();
                        db.SaveChanges();
                        return question.PostId;
                    }
                }
            }
            return 0;
        }
    }
}