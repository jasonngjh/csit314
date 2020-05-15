using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;

namespace CSIT314BCE.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Post post = new Post();

        // GET: Posts
        public ActionResult Index() 
        {
            return View();
        }

        // GET: Posts
        [HttpGet]
        public ActionResult Index(string search, string searchBy)
        {
            if (search != null)
            {
                if (searchBy == "Questions")
                {
                    var posts = db.Posts.Where(x => x.Title.Contains(search) || search == null).ToList();
                    return View(posts);
                }
                else
                {
                    var posts = db.Posts.Where(x => x.Body.Contains(search) || search == null).ToList();
                    return View("~/Views/Posts/AnswerSearch.cshtml", posts);
                }
            }
            else
            {
                var posts = db.Posts.Where(p => p.Title != null).Include(p => p.Student);
                return View(posts.ToList());
            }
        }

        public ActionResult Statistics(string viewBy, string viewQuestions, string viewAnswers)
        {
            if (viewQuestions != null)
            {
                if (viewBy == "Questions")
                {
                    var userId = User.Identity.GetUserId();
                    var posts = db.Posts.Where(g => g.OwnerId == userId && g.Title != null).ToList();
                    return View(posts);
                }
                else if (viewBy == "Answers")
                {
                    var userId = User.Identity.GetUserId();
                    var posts = db.Posts.Where(g => g.OwnerId == userId && g.Title == null).ToList();
                    return View("~/Views/Posts/Statistics2.cshtml", posts);
                }
                else
                {
                    return View("~/Views/Posts/PRatings.cshtml");
                }
            }
            else
            {
                var userId = User.Identity.GetUserId();
                return View(db.Posts.Where(g => g.OwnerId == userId && g.Title != null).ToList());
            }
        }

        [HttpPost]
        public ActionResult getRating()
        {
            var CurrentUserId = User.Identity.GetUserId();
            var userRecord = db.ApplicationUsers.Where(x => x.Id == CurrentUserId).FirstOrDefault().Ratings;

            return Json(new { success = true, responseText = userRecord }, JsonRequestBehavior.AllowGet);
        }


        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }
            DetailsViewModel model = post.GetDetails(post.PostId);
            return View(model);
        }

        // GET: Posts/Ask
        [Authorize(Roles = "Student")]
        public ActionResult Ask()
        {
            var userId = User.Identity.GetUserId();
            AskViewModel model = new AskViewModel
            {
                OwnerId = userId
            };
            return View(model);
        }

        // POST: Posts/Ask
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ask( AskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = post.Create(model);
                if (result != 0)
                {
                    return RedirectToAction("Details", new { id = result });
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostAnswer(PostAnsViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.OwnerId = User.Identity.GetUserId();
                var result = post.PostAns(model);
                if (result != 0)
                {
                    return RedirectToAction("Details", new { id = result });
                }
                else
                {
                    return RedirectToAction("Details", new { id = result });
                }
            }
            TempData["ViewData"] = ViewData;
            return RedirectToAction("Details", new { id = model.ParentId });
        }


        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.ApplicationUsers, "Id", "FullName", post.OwnerId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,AcceptedAnswerId,ParentId,VoteCount,AnswerCount,CreationDate,DeletionDate,ClosedDate,Title,Body,OwnerId,OwnerUsername")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.ApplicationUsers, "Id", "FullName", post.OwnerId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult VoteUp(int id)
        {
            //get current user id
            var userId = User.Identity.GetUserId();

            //update votecount to increase by 1
            Post post = db.Posts.Find(id);
            post.VoteCount += 1;

            //add into user who has voted
            Vote voted = new Vote
            {
                PostId = id,
                UserId = userId,
            };
            db.Votes.Add(voted);

            db.SaveChanges();

            //returns new vote count
            var newVoteCount = post.VoteCount;
            return Json(new { success = true, responseText = newVoteCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult VoteDown(int id)
        {
            //get current user id
            var userId = User.Identity.GetUserId();

            //update votecount to decrease by 1
            Post post = db.Posts.Find(id);
            post.VoteCount -= 1;

            //search records and delete from Voted
            var votedRecord = db.Votes.Where(x => x.PostId == id && x.UserId == userId).Select(x => x.VoteId).FirstOrDefault();
            Vote voted = db.Votes.Find(votedRecord);
            db.Votes.Remove(voted);
            db.SaveChanges();

            //returns new vote count
            var newVoteCount = post.VoteCount;
            return Json(new { success = true, responseText = newVoteCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult checkVoted(int id)
        {
            var CurrentUserId = User.Identity.GetUserId();
            bool contactExists = db.Votes.Where(u => u.PostId == id && u.UserId == CurrentUserId).Any();

            if (contactExists)
            {
                return Json(new { success = true, responseText = "TRUE" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, responseText = "FALSE" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ChooseAnswer(int postId) 
        {
            int result = post.ChooseAnswer(postId);
            if (result != 0) 
            {
                return RedirectToAction("Details", new { id = result });
            }
            return RedirectToAction("Details", new { id = postId });
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
                        var posts = db.Posts.Where(g => g.Title != null).OrderByDescending(p => p.AnswerCount).ToList();
                        return View(posts);
                    }
                    else if (viewBy == "MVotedQ")
                    {
                        var posts = db.Posts.Where(g => g.Title != null).OrderByDescending(p => p.VoteCount).ToList();
                        return View(posts);
                    }
                    else if (viewBy == "MCommentedQ")
                    {
                        var posts = db.Posts.Where(g => g.Title != null).OrderByDescending(p => p.CommentCount).ToList();
                        return View("~/Views/Posts/Reports2.cshtml", posts);
                    }
                    else
                    {
                        var users = db.ApplicationUsers.Where(g => g.Id != null).OrderByDescending(p => p.Ratings).ToList();
                        return View("~/Views/Posts/Reports3.cshtml", users);
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
                        var posts = db.Posts.Where(g => g.Title != null && g.CreationDate >= FromDate && g.CreationDate < ToDate).OrderByDescending(p => p.AnswerCount).ToList();
                        return View(posts);
                    }
                    else if (viewBy == "MVotedQ")
                    {
                        var posts = db.Posts.Where(g => g.Title != null && g.CreationDate >= FromDate && g.CreationDate < ToDate).OrderByDescending(p => p.VoteCount).ToList();
                        return View(posts);
                    }
                    else if (viewBy == "MCommentedQ")
                    {
                        var posts = db.Posts.Where(g => g.Title != null && g.CreationDate >= FromDate && g.CreationDate < ToDate).OrderByDescending(p => p.CommentCount).ToList();
                        return View("~/Views/Posts/Reports2.cshtml", posts);
                    }
                    else
                    {
                        var users = db.ApplicationUsers.Where(g => g.Id != null).OrderByDescending(p => p.Ratings).ToList();
                        return View("~/Views/Posts/Reports3.cshtml", users);
                    }
                }
            }
            else
            {
                var posts = db.Posts.Where(g => g.Title != null).OrderByDescending(p => p.AnswerCount).ToList();
                return View(posts);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
