using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualLibrary.Models;

namespace VirtualLibrary.Controllers
{
    public class RatingsController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Ratings
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Index()
        {
            var ratings = db.Books_Ratings;
            if (ratings != null)
            {
                return View(ratings.ToList());
            }
            else
            {
                return View("Index");
            }
        }

        [Authorize(Roles = "Admin, Moderator, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            string rating = form["rating"];
            string comment = form["comment"];
            string bookID = form["bookID"];
            if (rating==string.Empty)
            {
                ViewBag.StatusMessage = "You have not rate the book!!!";
                return View("../Search/GetBook", db.Books.SingleOrDefault(x => x.id == Convert.ToInt32(bookID)));
            }
            if (comment == string.Empty)
            {
                ViewBag.StatusMessage = "Comment is empty!!!";
                return View("../Search/GetBook", db.Books.SingleOrDefault(x=>x.id==Convert.ToInt32(bookID)));
            }


            var userName = User.Identity.Name;
            var model = db.Users.SingleOrDefault(s => s.username == userName);

            if(db.Books_Ratings.Where(x=>x.Users.username == userName).Count() > 0)
            {
                ViewBag.StatusMessage = "You have already comment/rate this book!!!";
                return View("../Search/GetBook", db.Books.SingleOrDefault(x => x.id == Convert.ToInt32(bookID)));
            }

            var bookRating = new Books_Ratings();
            bookRating.book_id = Convert.ToInt32(bookID);
            bookRating.comment = comment;
            bookRating.rating = Convert.ToInt32(rating);
            bookRating.timestamp = DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss"); ;
            bookRating.user_id = model.id;
            db.Books_Ratings.Add(bookRating);
            db.SaveChanges();
            log.Info("Review created.");
            return PartialView("Reviews", db.Books_Ratings.Where(x=>x.book_id== Convert.ToInt32(bookID)).ToList());
        }



        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books_Ratings rating = db.Books_Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", rating);
        }

        // POST: Ratingss/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var rating = db.Books_Ratings.Find(id);
            db.Books_Ratings.Remove(rating);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}