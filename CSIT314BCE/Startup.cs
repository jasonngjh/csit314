using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.IO;
using System.Linq;
using System.Web;

[assembly: OwinStartupAttribute(typeof(CSIT314BCE.Startup))]
namespace CSIT314BCE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRootUserAndRoles();
            //AddDataIntoDb();
        }

        public void AddDataIntoDb() 
        {
            AddUser();
            AddQuestions();
            AddAnswers();
            AddComments();
        }

        public void AddComments() 
        {
            string path = HttpContext.Current.Server.MapPath(@"~\TestData\comments.txt");
            string[] commentsFile = File.ReadAllLines(path);
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string[] token = { "><" };
            string[] commentArray;
            string postid;
            foreach (string comment in commentsFile) 
            {
                commentArray = comment.Split(token, System.StringSplitOptions.RemoveEmptyEntries);
                postid = ReturnQuestionId(commentArray[0]);
                if (postid == null) 
                {
                    postid = ReturnAnswerId(commentArray[0]);
                }

                var user = userManager.FindByName(commentArray[3]);
                DateTime odate = Convert.ToDateTime(commentArray[1]);

                if (user != null && postid != "" && commentArray[2] != "") 
                {
                    Comment c = new Comment()
                    {
                        PostId = Int32.Parse(postid),
                        Text = commentArray[2],
                        CreationDate = odate,
                        OwnerId = user.Id,
                        OwnerUsername = user.UserName
                    };

                    context.Comments.Add(c);
                    context.SaveChanges();
                }
                postid = "";
            }
        }

        public string ReturnQuestionId(string answerQuestionId) {
            string path = HttpContext.Current.Server.MapPath(@"~\TestData\questionIdToDB.txt");
            string[] qtoDb = File.ReadAllLines(path);
            string[] qDB;
            foreach (string q in qtoDb)
            {
                qDB = q.Split(';');
                if (answerQuestionId == qDB[0])
                {
                    return qDB[2];
                }
            }
            return null;
        }
        public void AddAnswers() 
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string answerPath = HttpContext.Current.Server.MapPath(@"~\TestData\answer.txt");
            string[] alines = File.ReadAllLines(answerPath);
            string folderPath = @"~\TestData\answer\id_";
            string[] answer;
            string[] token = { "><" };

            string questionId, answerBody;
            foreach (string aline in alines) 
            {
                answer = aline.Split(token, System.StringSplitOptions.RemoveEmptyEntries);
                questionId = ReturnQuestionId(answer[0]);
                var user = userManager.FindByName(answer[2]);
                if (user != null)
                {
                    folderPath += answer[1];
                    string fp = HttpContext.Current.Server.MapPath(folderPath);
                    answerBody = File.ReadAllText(fp);
                    DateTime odate = Convert.ToDateTime(answer[3]);

                    Post post = new Post() {
                        OwnerId = user.Id,
                        OwnerUsername = user.UserName,
                        Body = answerBody,
                        CreationDate = odate,
                        VoteCount = Int32.Parse(answer[4]),
                        ParentId = Int32.Parse(questionId)
                    };

                    context.Posts.Add(post);
                    context.SaveChanges();
                    context.Entry(post).GetDatabaseValues();

                    string answerDb = HttpContext.Current.Server.MapPath(@"~\TestData\answerIdToDB.txt");
                    using (StreamWriter sw = File.AppendText(answerDb))
                    {
                        sw.WriteLine(answer[1] + ";" + (post.PostId).ToString());
                    }
                    folderPath = @"~\TestData\answer\id_";
                }
            }

            string qToDb = HttpContext.Current.Server.MapPath(@"~\TestData\questionIdToDB.txt");
            string[] questionToDb = File.ReadAllLines(qToDb);
            string[] questionLine;
            string acceptedAnsId;
            var posts = context.Posts.Where(p => p.Title != null).ToList();
            foreach(Post post in posts) {
                var count = context.Posts
                            .Where(p => p.ParentId == post.PostId)
                            .Count();
                post.AnswerCount = count;

                foreach (string q in questionToDb)
                {
                    questionLine = q.Split(';');
                    if (questionLine[1] != "") 
                    {
                        if (Int32.Parse(questionLine[2]) == post.PostId)
                        {
                            acceptedAnsId = ReturnAnswerId(questionLine[1]);
                            post.AcceptedAnswerId = Int32.Parse(acceptedAnsId);
                        }
                    }
                }
                context.SaveChanges();
            }
        }


        public string ReturnAnswerId(string acceptedAnswer)
        {
            string path = HttpContext.Current.Server.MapPath(@"~\TestData\answerIdToDB.txt");
            string[] atoDb = File.ReadAllLines(path);
            string[] aDB;
            foreach (string a in atoDb)
            {
                aDB = a.Split(';');
                if (acceptedAnswer == aDB[0])
                {
                    return aDB[1];
                }
            }
            return null;
        }

        public void AddQuestions() 
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string folderPath = @"~\TestData\question\id_";
            string path = HttpContext.Current.Server.MapPath(@"~\TestData\question.txt");
            string[] lines = File.ReadAllLines(path);
            string[] question;
            string[] token = { "><" };
            string questionBody;
            foreach (string line in lines) 
            {
                question = line.Split(token, System.StringSplitOptions.RemoveEmptyEntries);
                var user = userManager.FindByName(question[1]);
                if (user != null) 
                {
                    folderPath += question[0];
                    string fp = HttpContext.Current.Server.MapPath(folderPath);
                    questionBody = File.ReadAllText(fp);
                    DateTime odate = Convert.ToDateTime(question[2]);
                    Post post = new Post() {
                        OwnerId = user.Id,
                        OwnerUsername = user.UserName,
                        Title = question[4],
                        Body = questionBody,
                        CreationDate = odate,
                        VoteCount = Int32.Parse(question[3]),
                        AnswerCount = 0
                    };

                    context.Posts.Add(post);
                    context.SaveChanges();
                    context.Entry(post).GetDatabaseValues();

                    string qidtoDB = HttpContext.Current.Server.MapPath(@"~\TestData\questionIdToDB.txt");
                    using (StreamWriter sw = File.AppendText(qidtoDB))
                    {
                        if (question.Length == 6)
                        {
                            sw.WriteLine(question[0] + ";" + question[5] + ";" + (post.PostId).ToString());
                        }
                        else 
                        {
                            sw.WriteLine(question[0] + ";;" + (post.PostId).ToString());
                        }
                        
                    }
                    folderPath = @"~\TestData\question\id_";
                }
            }
        }

        public void AddUser() 
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            string path = HttpContext.Current.Server.MapPath(@"~\TestData\users.txt");
            string[] lines = File.ReadAllLines(path);
            string[] user;
            string pwd = "@Abc123";
            foreach (string line in lines)
            {
                user = line.Split(';');
                Student student = new Student() {
                    UserName = user[0],
                    FullName = user[1],
                    Email = user[2],
                    Ratings = Int32.Parse(user[5])
                };
                var result = userManager.Create(student,pwd);
                if (result.Succeeded) 
                {
                    userManager.AddToRole(student.Id,"Student");
                }
            }
        }

        public void CreateRootUserAndRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<Admin>(new UserStore<Admin>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                //create admin role
                var role = new IdentityRole("Admin");
                roleManager.Create(role);

                //create root user
                Admin user = new Admin {
                    UserName = "root",
                    Email = "root@domain.com",
                };
                string pwd = "@Abc123";

                var newuser = userManager.Create(user, pwd);
                if (newuser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("Student")) {
                var role = new IdentityRole("Student");
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Moderator"))
            {
                var role = new IdentityRole("Moderator");
                roleManager.Create(role);
            }
        }
    }
}
