using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

[assembly: OwinStartupAttribute(typeof(CSIT314BCE.Startup))]
namespace CSIT314BCE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRootUserAndRoles();
            /*AddQuestions();
            AddAnswers();*/
        }

        public void AddComments() { }

        public string ReturnQuestionId(string answerQuestionId) {
            string[] qtoDb = File.ReadAllLines(@"C:\Users\Jason\Desktop\questionIdToDB.txt");
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
            string[] alines = File.ReadAllLines(@"C:\Users\Jason\Desktop\answer.txt");
            string folderPath = @"C:\Users\Jason\Desktop\answer\id_";
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
                    answerBody = File.ReadAllText(folderPath);
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

                    using (StreamWriter sw = File.AppendText(@"C:\Users\Jason\Desktop\answerIdToDB.txt"))
                    {
                        sw.WriteLine(answer[1] + ";" + (post.PostId).ToString());
                    }
                    folderPath = @"C:\Users\Jason\Desktop\answer\id_";
                }
                
            }
        }

        public void AddQuestions() 
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string folderPath = @"C:\Users\Jason\Desktop\question\id_";
            string[] lines = File.ReadAllLines(@"C:\Users\Jason\Desktop\question.txt");
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
                    questionBody = File.ReadAllText(folderPath);
                    //DateTime odate = DateTime.ParseExact(question[2], "yyyy-MM-dd hh:mm:ss tt", null);
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

                    using (StreamWriter sw = File.AppendText(@"C:\Users\Jason\Desktop\questionIdToDB.txt"))
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
                    folderPath = @"C:\Users\Jason\Desktop\question\id_";
                }
            }
        }

        public void AddUser() 
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            string[] lines = File.ReadAllLines(@"C:\Users\Jason\Desktop\users.txt");
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
