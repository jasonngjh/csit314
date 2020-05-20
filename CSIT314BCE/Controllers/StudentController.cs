using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;

namespace CSIT314BCE.Controllers
{
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Student student = new Student();

        public ActionResult Statistics(string viewBy, string viewQuestions, string viewAnswers)
        {
            if (viewQuestions != null)
            {
                if (viewBy == "Questions")
                {
                    var userId = User.Identity.GetUserId();
                    var posts = student.RetrieveMyQuestions(userId);
                    return View(posts);
                }
                else if (viewBy == "Answers")
                {
                    var userId = User.Identity.GetUserId();
                    var posts = student.RetrieveMyAnswers(userId);
                    return View("~/Views/Student/Statistics2.cshtml", posts);
                }
                else
                {
                    return View("~/Views/Student/PRatings.cshtml");
                }
            }
            else
            {
                var userId = User.Identity.GetUserId();
                var posts = student.RetrieveMyQuestions(userId);
                return View(posts);
            }
        }

        [HttpPost]
        public ActionResult getRating()
        {
            var CurrentUserId = User.Identity.GetUserId();
            var userRecord = student.GetRating(CurrentUserId);
            return Json(new { success = true, responseText = userRecord }, JsonRequestBehavior.AllowGet);
        }
    }
}