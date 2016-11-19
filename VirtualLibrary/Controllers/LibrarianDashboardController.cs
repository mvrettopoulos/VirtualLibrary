using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualLibrary.Models;

namespace VirtualLibrary.Controllers
{
    public class LibrarianDashboardController : Controller
    {
        private VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        // GET: LibrarianDashboard
        public ActionResult Index()
        {
            var user_id = User.Identity.GetUserId();
            var librarian = db.Users.Where(u => u.aspnet_user_id == user_id).Single();
            var libraries_ids = librarian.Librarians.Select(x => x.library_id);
            var selected_reservations = db.Reservations.Where(r => libraries_ids.Contains(r.library_id));
            var selected_reservations_new = selected_reservations.Where(r => r.check_in == false || r.check_out == false);
            return View(selected_reservations_new.ToList());
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        // GET: LibrarianDashboard/Checkin/4
        public ActionResult Checkin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservations reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return PartialView("Checkin", reservation);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Checkin")]
        [ValidateAntiForgeryToken]
        // POST: LibrarianDashboard/Checkin/4
        public ActionResult CheckinConfirmed(int id)
        {
            var reservation = db.Reservations.Find(id);
            reservation.check_in = true;
            try
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DataException e)
            {
                log.Error("Database error:", e);
            }
            log.Info("Checkin done");
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        // GET: LibrarianDashboard/Checkout/4
        public ActionResult Checkout(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservations reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return PartialView("Checkout", reservation);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Checkout")]
        [ValidateAntiForgeryToken]
        // POST: LibrarianDashboard/Checkout/4
        public ActionResult CheckoutConfirmed(int id)
        {
            var reservation = db.Reservations.Find(id);
            if (reservation.check_in == false)
            {
                log.Info("Cannot checkout before checkin");
                return RedirectToAction("Index");
            }
            reservation.check_out = true;
            CultureInfo culture = new CultureInfo("ISO");
            string today = DateTime.Now.ToString("yyyy-MMMM-dd", culture);
            reservation.return_date = today;
            var bookAvailable = db.Books_Availability.Single(x => x.book_id == reservation.book_id && x.library_id == reservation.library_id);
            bookAvailable.reserved = bookAvailable.reserved - 1;
            bookAvailable.available++;
            try
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.Entry(bookAvailable).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DataException e)
            {
                log.Error("Database error:", e);
            }
            log.Info("Checkout done");
            return RedirectToAction("Index");
        }
    }
}