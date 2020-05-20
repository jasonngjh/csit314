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
            var posts = post.RetrievePosts(search, searchBy);
            if (searchBy == "Answers")
            {
                return View("~/Views/Posts/AnswerSearch.cshtml", posts);
            }
            return View(posts);
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

        [Authorize(Roles = "Student")]
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

            var newVoteCount = post.VoteUp(id, userId);
            return Json(new { success = true, responseText = newVoteCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult VoteDown(int id)
        {
            //get current user id
            var userId = User.Identity.GetUserId();

            //returns new vote count
            var newVoteCount = post.VoteDown(id, userId);
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
