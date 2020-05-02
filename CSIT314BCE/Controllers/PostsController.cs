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
            var posts = db.Posts.Where(p => p.Title != null).Include(p => p.Student);
            return View(posts.ToList());
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
        public ActionResult Ask(AskViewModel model)
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

        public ActionResult PostAnswer(string body, int PostId)
        {
            var userId = User.Identity.GetUserId();
            return View();
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
