using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSIT314BCE.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private Post post = new Post();
        public HomeController()
        {
        }

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UserProfile(string userId)
        {
            if (userId != null)
            {
                var user = (new ApplicationDbContext()).Users.FirstOrDefault(s => s.Id == userId);
                return View(user);
            }
            else
            {
                userId = User.Identity.GetUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    var user = (new ApplicationDbContext()).Users.FirstOrDefault(s => s.Id == userId);
                    return View(user);
                }
            }
        }
    }
}