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
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Authorize(Roles = "Moderator,Admin ")]
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

        [HttpGet]
        [Authorize(Roles = "Moderator,Admin ")]
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

        
        [HttpPost, ActionName("Checkin")]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        // POST: LibrarianDashboard/Checkin/4
        public ActionResult CheckinConfirmed(int id)
        {
            var reservation = db.Reservations.Find(id);
            reservation.check_in = true;
            var bookAvailable = db.Books_Availability.Single(x => x.book_id == reservation.book_id && x.library_id == reservation.library_id);
            bookAvailable.reserved++;
            bookAvailable.available = bookAvailable.available - 1;
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
            log.Info("Checkin done");
            return RedirectToAction("Index");
        }


        
        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
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

        
        [HttpPost, ActionName("Checkout")]
        [Authorize(Roles = "Admin, Moderator")]
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
            DateTime today = DateTime.Now;
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

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult CreateReservation()
        {
            ViewBag.Libraries = AvailableLibraries();
            return PartialView("CreateReservation");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult CreateReservation(LibrarianCreateReservationViewModel model)
        {
            if (ModelState.IsValid)
            {

                string format = "dd-mm-yyyy";
                DateTime dateTime;


                var book = db.Books.SingleOrDefault(x => x.isbn == model.Isbn);
                if (book == null)
                {
                    ModelState.AddModelError("isbn", "Book does not exist with ISBN: " + model.Isbn + " !!!");
                    ViewBag.Libraries = AvailableLibraries();
                    return PartialView("CreateReservation", model);
                }

                var user = db.Users.SingleOrDefault(x => x.username == model.Username);
                if (user == null)
                {
                    ModelState.AddModelError("username", "User does not exists with usernaem: " + model.Username + "!!!");
                    ViewBag.Libraries = AvailableLibraries();
                    return PartialView("CreateReservation", model);
                }


                if (!DateTime.TryParseExact(model.return_date, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    ModelState.AddModelError("return_date", "Date format is not correct!!!Format expected is dd-mm-yyyy!");
                    ViewBag.Libraries = AvailableLibraries();
                    return PartialView("CreateReservation", model);
                }
                if (!DateTime.TryParseExact(model.reserved_date, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    ModelState.AddModelError("reserved_date", "Date format is not correct!!!Format expected is dd-mm-yyyy!");
                    ViewBag.Libraries = AvailableLibraries();
                    return PartialView("CreateReservation", model);
                }
                Reservations reservation = new Reservations();
                var library_id = Convert.ToInt32(model.Library);
                reservation.Books = book;
                reservation.check_in = true;
                reservation.check_out = false;
                reservation.Libraries = db.Libraries.Single(x => x.id == library_id);
                reservation.reserved_date = Convert.ToDateTime(model.reserved_date);
                reservation.return_date = Convert.ToDateTime(model.return_date);
                reservation.renewTimes = 3;
                reservation.Users = user;

                if (reservation.reserved_date > reservation.return_date || reservation.reserved_date.Value.AddDays(7) < reservation.return_date)
                {
                    ModelState.AddModelError("", "The dates you selected are invalid!!!");
                    ViewBag.Libraries = AvailableLibraries();
                    return PartialView("CreateReservation", model);
                }

                var bookAvailable = db.Books_Availability.Single(x => x.book_id == book.id && x.library_id == library_id);
                if (!CheckIfAvailable(book.id, library_id, bookAvailable.quantity, reservation.reserved_date, reservation.return_date))
                {
                    ModelState.AddModelError("", "Book is not available the dates you selected!!!");
                    ViewBag.Libraries = AvailableLibraries();
                    return PartialView("CreateReservation", model);
                }
                try
                {
                    db.Reservations.Add(reservation);
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Database error:", e);
                    ModelState.AddModelError("", "Book is not available with the setting you selected!!!");
                    ViewBag.Libraries = AvailableLibraries();
                    return PartialView("CreateReservation", model);
                }
                log.Info("Reservation created.");

                return Json(new { success = true });
            }
            return PartialView("CreateReservation", model);
        }

        private bool CheckIfAvailable(int bookId, int libraryId, int? quantity, DateTime? reservedDate, DateTime? returnDate)
        {
            List<Reservations> reservationsList = db.Reservations.Where(x => x.book_id == bookId && x.library_id == libraryId && (reservedDate >= x.reserved_date && reservedDate <= x.return_date) && (returnDate >= x.reserved_date && returnDate <= x.return_date)).ToList();

            if (reservationsList.Count < quantity)
            {
                return true;
            }

            return false;
        }

        private List<Libraries> AvailableLibraries()
        {
            string username = User.Identity.Name;
            var librarianLibraries = db.Librarians.Where(x => x.Users.username == username).ToList();
            var libraries = new List<Libraries>();
            foreach (var library in librarianLibraries)
            {
                libraries.Add(library.Libraries);
            }
            return libraries;
        }
    }
}