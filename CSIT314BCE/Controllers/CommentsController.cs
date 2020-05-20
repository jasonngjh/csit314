using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace CSIT314BCE.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Comment comment = new Comment();

        // GET: Comments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        public ActionResult PostComment() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostComment(PostCommentViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                var post = db.Posts.Find(model.PostId);
                if (post.ParentId != 0) 
                {
                    return RedirectToAction("Details", "Posts", new { id = post.ParentId });
                }
                return RedirectToAction("Details", "Posts", new { id = model.PostId });
            }

            model.OwnerId = User.Identity.GetUserId();
            var result = comment.PostComment(model);
            if (result != 0) 
            {
                Post post = db.Posts.Find(result);
                if (post.ParentId == 0)
                {
                    return RedirectToAction("Details", "Posts", new { id = result });
                }
                else 
                {
                    return RedirectToAction("Details", "Posts", new { id = post.ParentId });
                }
            }
            return RedirectToAction("Details", "Posts", new { id = model.PostId });
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
