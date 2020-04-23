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
using System.Threading.Tasks;
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
            return View(context.Users.ToList());
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
            ViewBag.Users = context.Users.Select(r => new SelectListItem { Value = r.UserName, Text = r.UserName }).ToList();
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
            return View(context.Users.ToList());
        }

        public ActionResult ViewDetails(string id)
        {
            return View(context.Users.Where(x => x.Id == id).FirstOrDefault());
        }

        public ActionResult Edit(string id, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.EditUserSuccess ? "User has been updated!"
              : message == ManageMessageId.ResetUserPasswordSuccess ? "User's password has been reset!"
              : "";

            ApplicationUser user = new ApplicationUser();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            user = userManager.FindById(id);
            if (user == null)
            {
                return View("Index");
            }

            EditUserViewModel editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc
            };
            return View(editUserViewModel);

        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = userManager.FindById(model.Id);
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.LockoutEnabled = model.LockoutEnabled;
            user.LockoutEndDateUtc = model.LockoutEndDateUtc;

            await userManager.UpdateAsync(user);
            var ctx = store.Context;
            ctx.SaveChanges();

            return RedirectToAction("Edit", new { id = model.Id, Message = ManageMessageId.EditUserSuccess });
        }

        // GET: /Admin/ResetUserPassword/Id
        public ActionResult ResetUserPassword(string id)
        {
            ApplicationUser user = new ApplicationUser();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            user = userManager.FindById(id);
            if (user == null)
            {
                return RedirectToAction("Edit", new { id = id });
            }

            ResetUserPasswordViewModel reset = new ResetUserPasswordViewModel
            {
                Id = id
            };
            return View(reset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetUserPassword(ResetUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var user = userManager.FindById(model.Id);
                if (user == null)
                {
                    return View(model);
                }

                await userManager.RemovePasswordAsync(model.Id);
                var result = await userManager.AddPasswordAsync(model.Id, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Edit", new { id = model.Id, Message = ManageMessageId.ResetUserPasswordSuccess });
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public enum ManageMessageId
        {
            EditUserSuccess,
            ResetUserPasswordSuccess,
            Error
        }
    }
}