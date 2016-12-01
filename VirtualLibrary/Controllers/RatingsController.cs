using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
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

        
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            var bookID = Convert.ToInt32(form["bookID"]);
            var userName = User.Identity.Name;
            var model = db.Users.SingleOrDefault(s => s.username == userName);
            if (db.Books_Ratings.Where(x => x.Users.username == userName && x.book_id == bookID).Any())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "You have already comment/rate this book!!!" });
            }
            string rating = form["rating"];
            string comment = form["comment"];
            
            if (rating==string.Empty)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "You have not rate the book!!!" });
            }
            if (comment == string.Empty)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Comment is empty!!!" });
            }

            var bookRating = new Books_Ratings();
            bookRating.book_id = Convert.ToInt32(bookID);
            bookRating.comment = comment;
            bookRating.rating = Convert.ToInt32(rating);
            bookRating.timestamp = DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss");
            bookRating.user_id = model.id;
            db.Books_Ratings.Add(bookRating);
            db.SaveChanges();
            log.Info("Review created.");
            return PartialView("Reviews", db.Books_Ratings.Where(x=>x.book_id== bookID).ToList());
        }



        
        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
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
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Moderator")]
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