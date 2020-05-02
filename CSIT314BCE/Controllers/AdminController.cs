using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CSIT314BCE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        Admin admin = new Admin();
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin
        public ActionResult Index(string search)
        {
            return View(_context.Users.Where(x => x.UserName.Contains(search) || search == null).ToList());
        }

        // GET: Admin/CreateUser
        public ActionResult CreateUser()
        {
            ViewBag.Roles = _context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }

        // GET: Admin/CreateUser
        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = admin.CreateUser(model);
                if (user != null) 
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(user.Id,model.Role);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrors(result);
                        ViewBag.Roles = _context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
                        return View(model);
                    }
                }
            }
            ViewBag.Roles = _context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        [HttpPost]
        public ActionResult NewRole(FormCollection form)
        {
            string rolename = form["RoleName"];
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
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
            ViewBag.Users = _context.Users.Select(r => new SelectListItem { Value = r.UserName, Text = r.UserName }).ToList();
            ViewBag.Roles = _context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AssignRole(FormCollection form)
        {
            string usrname = form["txtUserName"];
            string rolname = form["RoleName"];
            ApplicationUser user = _context.Users.Where(u => u.UserName.Equals(usrname, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            userManager.AddToRole(user.Id, rolname);

            return View("Index");
        }

        public ActionResult ListUsers()
        {
            return View(_context.Users.ToList());
        }

        public ActionResult ViewDetails(string id)
        {
            return View(_context.Users.Where(x => x.Id == id).FirstOrDefault());
        }

        public ActionResult Edit(string id, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.EditUserSuccess ? "User has been updated!"
              : message == ManageMessageId.ResetUserPasswordSuccess ? "User's password has been reset!"
              : "";

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var user = userManager.FindById(id);
            if (user == null)
            {
                return View("Index");
            }
            return View(admin.EditUser(user));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await admin.EditUser(model) == true)
            {
                return RedirectToAction("Edit", new { id = model.Id, Message = ManageMessageId.EditUserSuccess });
            }
            else
            {
                return View(model);
            }
        }

        // GET: /Admin/ResetUserPassword/Id
        public ActionResult ResetUserPassword(string id)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var user = userManager.FindById(id);
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
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
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