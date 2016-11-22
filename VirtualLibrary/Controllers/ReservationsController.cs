using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualLibrary.Models;

namespace VirtualLibrary.Controllers
{
    public class ReservationsController : Controller
    {
        private VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Reservations
        public ActionResult Index()
        {
            return View(db.Reservations.ToList());
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public ActionResult Delete(int? id)
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
            return PartialView("Delete", reservation);
        }
        // POST: Authors/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "User,Admin, Moderator")]
        [HttpGet]
        public ActionResult Reserve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ReservationsViewModel reservation = new ReservationsViewModel();
            reservation.book = book.title;
            List<Books_Availability> bookAvailable = db.Books_Availability.Where(x => x.quantity > 0 && x.book_id == id).ToList();
            var librariesList = new List<Libraries>();
            foreach (var available in bookAvailable)
            {
                librariesList.Add(available.Libraries);
            }
            if (librariesList.Count == 0)
            {
                ViewBag.StatusMessage = "Book is not available yet!!";
                return View("../Search/GetBook", book);
            }

            reservation.reservationDatesLibraries = getDateLibraries(librariesList, book.id);

            var userName = User.Identity.Name;
            var user = db.Users.SingleOrDefault(s => s.username == userName);
            reservation.username = user.username;
            ViewBag.Libraries = librariesList;
            ViewBag.AvailableDate = "";


            return View("Reserve", reservation);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reserve(ReservationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var book = db.Books.Single(x => x.title == model.book);
                Reservations reservation = new Reservations();
                var library_id = Convert.ToInt32(model.library);
                reservation.Books = book;
                reservation.check_in = false;
                reservation.check_out = false;
                reservation.Libraries = db.Libraries.Single(x => x.id == library_id);
                reservation.reserved_date = model.reserved_date;
                reservation.return_date = model.return_date;
                reservation.renewTimes = 3;
                reservation.Users = db.Users.Single(x => x.username == model.username);

                var bookAvailable = db.Books_Availability.Single(x => x.book_id == book.id && x.library_id == library_id);
                if (bookAvailable.available == 0)
                {
                    ViewBag.StatusMessage = "Book is not available yet!!";
                    return View("../Search/GetBook", book);
                }
                bookAvailable.quantity = --bookAvailable.available;
                bookAvailable.reserved = ++bookAvailable.reserved;
                try
                {
                    db.Entry(bookAvailable).State = EntityState.Modified;
                    db.Reservations.Add(reservation);
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Database error:", e);
                }
                log.Info("Reservation created.");
                ViewBag.StatusSuccessMessage = "Book reserved!!";
                return View("SuccessReserve", reservation);
            }
            return View("Reserve", model);
        }

        private List<ReservationDatesLibraries> getDateLibraries(List<Libraries> librariesList, int bookid)
        {
            List<ReservationDatesLibraries> reservationDatesLibrariesList = new List<ReservationDatesLibraries>();
            foreach (var library in librariesList)
            {
                ReservationDatesLibraries reservationDatesLibraries = new ReservationDatesLibraries();
                reservationDatesLibraries.library = library.id;
                List<Reservations> reservationsList = db.Reservations.Where(x => x.library_id == library.id && x.book_id == bookid).ToList();
                List<ReservationDatesRange> dates = new List<ReservationDatesRange>();
                foreach (var reservation in reservationsList)
                {
                    ReservationDatesRange range = new ReservationDatesRange();
                    range.startDate = reservation.reserved_date;
                    range.endDate = reservation.return_date;
                    dates.Add(range);
                }
                reservationDatesLibraries.datesRange = dates;
                reservationDatesLibrariesList.Add(reservationDatesLibraries);
            }
            return reservationDatesLibrariesList;

        }

    }
}