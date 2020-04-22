using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSIT314BCE.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateUser()
        {
            ViewBag.Roles = context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(FormCollection form)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string UserName = form["txtUsername"];
            string email = form["txtEmail"];
            string pwd = form["txtPassword"];
            string name = form["txtFullname"];

            //create default user
            var user2 = new ApplicationUser();
            user2.UserName = UserName;
            user2.Email = email;
            user2.FullName = name;


            var newuser = userManager.Create(user2, pwd);

            string usrname = form["txtUsername"];
            string rolname = form["RoleName"];
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(usrname, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();          
            userManager.AddToRole(user.Id, rolname);
         

            return View("Index");
        }

        [HttpPost]
        public ActionResult NewRole(FormCollection form)
        {
            string rolename = form["RoleName"];
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists(rolename))
            {
                //create super admin role
                var role = new IdentityRole(rolename);
                roleManager.Create(role);
            }

            return View("Index");
        }

        public ActionResult AssignRole()
        {
            ViewBag.Roles = context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AssignRole(FormCollection form)
        {
            string usrname = form["txtUserName"];
            string rolname = form["RoleName"];
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(usrname, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.AddToRole(user.Id, rolname);

            return View("Index");
        }

        public ActionResult ListUsers()
        {
            using (dbModel dbModel = new dbModel())
            {
                return View(dbModel.AspNetUsers.ToList());
            }
        }

        public ActionResult ViewDetails(string id)
        {
            using (dbModel dbModel = new dbModel())
            {
                return View(dbModel.AspNetUsers.Where(x => x.Id ==id).FirstOrDefault());
            }
        }

        public ActionResult Edit(string id)
        {
            using (dbModel dbModel = new dbModel())
            {
                return View(dbModel.AspNetUsers.Where(x => x.Id == id).FirstOrDefault());
            }
        }

        [HttpPost]
        public ActionResult Edit(AspNetUser users)
        {
            try
            {
                using (dbModel dbModel = new dbModel())
                {
                    dbModel.Entry(users).State = EntityState.Modified;
                    dbModel.SaveChanges();
                }
                return RedirectToAction("ListUsers");
            }
            catch
            {
                return View();
            }
        }

    }
}