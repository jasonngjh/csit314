using CSIT314BCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSIT314BCE.Controllers
{
    public class ModeratorController : Controller
    {
        Moderator mod = new Moderator();

        // GET: Moderator
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Moderator")]
        public ActionResult Reports(string viewBy, string viewAnswered, string timespan, string StartDate, string EndDate)
        {
            if (viewAnswered != null)
            {
                if (timespan == "alltime")
                {
                    if (viewBy == "MAnsweredQ")
                    {
                        var posts = mod.RetrieveMostAnsweredQuestions();
                        return View(posts);
                    }
                    else if (viewBy == "MVotedQ")
                    {
                        var posts = mod.RetrieveMostVotedQuestions();
                        return View(posts);
                    }
                    else if (viewBy == "MCommentedQ")
                    {
                        var posts = mod.RetrieveMostCommentedQuestion();
                        return View("~/Views/Moderator/Reports2.cshtml", posts);
                    }
                    else
                    {
                        var users = mod.RetrieveTopStudents();
                        return View("~/Views/Moderator/Reports3.cshtml", users);
                    }
                }
                else
                {
                    //time span
                    StartDate = StartDate + " 0:00 AM";
                    DateTime FromDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                    EndDate = EndDate + " 0:00 AM";
                    DateTime ToDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                    ToDate = ToDate.AddDays(1);

                    if (viewBy == "MAnsweredQ")
                    {
                        var posts = mod.RetrieveMostAnsweredQuestions(FromDate,ToDate);
                        return View(posts);
                    }
                    else if (viewBy == "MVotedQ")
                    {
                        var posts = mod.RetrieveMostVotedQuestions(FromDate, ToDate);
                        return View(posts);
                    }
                    else if (viewBy == "MCommentedQ")
                    {
                        var posts = mod.RetrieveMostCommentedQuestion(FromDate, ToDate);
                        return View("~/Views/Moderator/Reports2.cshtml", posts);
                    }
                    else
                    {
                        var users = mod.RetrieveTopStudents();
                        return View("~/Views/Moderator/Reports3.cshtml", users);
                    }
                }
            }
            else
            {
                var posts = mod.RetrieveMostAnsweredQuestions();
                return View(posts);
            }
        }
    }
}