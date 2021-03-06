﻿using CSIT314BCE.Models;
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
        private ApplicationUser appuser = new ApplicationUser();
        public HomeController()
        {
        }

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserProfile(string userId)
        {
            if (userId != null)
            {
                var user = appuser.RetrieveUserProfile(userId);
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
                    var user = appuser.RetrieveUserProfile(userId);
                    return View(user);
                }
            }
        }
    }
}