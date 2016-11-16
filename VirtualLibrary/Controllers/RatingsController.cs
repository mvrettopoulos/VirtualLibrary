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
        private VirtualLibraryEntities db = new VirtualLibraryEntities();
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