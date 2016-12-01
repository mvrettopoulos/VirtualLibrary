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
    public class ReservationsController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Reservations
        public ActionResult Index()
        {
            return View(db.Reservations.ToList());
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
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
            if(reservation.check_in!=reservation.check_out )
            {
                ModelState.AddModelError("error", "You cannot delete this reservation until is completed");
                return PartialView("Delete", reservation);
            }
            return PartialView("Delete", reservation);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var reservation = db.Reservations.Find(id);
            if (reservation.check_in != reservation.check_out)
            {
                ViewBag.ErrorMessage = "You cannot delete this reservation until is completed";
                return View("Index", db.Reservations.ToList());
            }
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

        
        [HttpGet]
        [Authorize(Roles = "User,Admin, Moderator")]
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
            reservation.Book = book.title;
            var librariesList = AvailableLibraries(book.id);
            if (librariesList.Count == 0)
            {
                ViewBag.StatusMessage = "Book is not available yet!!";
                return View("../Search/GetBook", book);
            }

            var userName = User.Identity.Name;
            var user = db.Users.SingleOrDefault(s => s.username == userName);
            if ((bool)user.bad_user)
            {
                ViewBag.StatusMessage = "You can not reserve any book. Please contact your librarian";
                return View("../Search/GetBook", book);
            }
            reservation.Username = user.username;
            ViewBag.Libraries = librariesList;


            return View("Reserve", reservation);
        }

        
        [HttpPost]
        [Authorize(Roles = "User, Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult Reserve(ReservationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string format = "dd-mm-yyyy";
                DateTime dateTime;
                

                var book = db.Books.Single(x => x.id == model.Id);
                if (!DateTime.TryParseExact(model.return_date, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    ModelState.AddModelError("return_date", "Date format is not correct!!!Format expected is dd-mm-yyyy!");
                    ViewBag.Libraries = AvailableLibraries(book.id);
                    return View("Reserve", model);
                }
                if (!DateTime.TryParseExact(model.reserved_date, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    ModelState.AddModelError("reserved_date", "Date format is not correct!!!Format expected is dd-mm-yyyy!");
                    ViewBag.Libraries = AvailableLibraries(book.id);
                    return View("Reserve", model);
                }
                Reservations reservation = new Reservations();
                var library_id = Convert.ToInt32(model.Library);
                reservation.Books = book;
                reservation.check_in = false;
                reservation.check_out = false;
                reservation.Libraries = db.Libraries.Single(x => x.id == library_id);
                reservation.reserved_date =Convert.ToDateTime(model.reserved_date);
                reservation.return_date = Convert.ToDateTime(model.return_date);
                reservation.renewTimes = 3;
                reservation.Users = db.Users.Single(x => x.username == model.Username);

                if (reservation.reserved_date > reservation.return_date || reservation.reserved_date.Value.AddDays(7) <reservation.return_date )
                {
                    ViewBag.StatusMessage = "The dates you selected are invalid!!!";
                    ViewBag.Libraries = AvailableLibraries(book.id);
                    return View("Reserve", model);
                }

                var bookAvailable = db.Books_Availability.Single(x => x.book_id == book.id && x.library_id == library_id);
                if (!CheckIfAvailable(book.id,library_id,bookAvailable.quantity, reservation.reserved_date, reservation.return_date))
                {
                    ViewBag.StatusMessage = "Book is not available the dates you selected!!!";
                    ViewBag.Libraries = AvailableLibraries(book.id);
                    return View("Reserve", model);
                }
                try
                {
                    db.Reservations.Add(reservation);
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Database error:", e);
                }
                log.Info("Reservation created.");
                ViewBag.StatusSuccessMessage = "Book reserved!!";
                return View("../Search/GetBook", book);
            }
            return View("Reserve", model);
        }

       
        private bool CheckIfAvailable(int bookId,int libraryId,int? quantity, DateTime? reservedDate, DateTime? returnDate)
        {
            List<Reservations> reservationsList = db.Reservations.Where(x => x.book_id == bookId && x.library_id == libraryId && (reservedDate >= x.reserved_date && reservedDate <= x.return_date) && (returnDate >= x.reserved_date && returnDate <= x.return_date)).ToList();

            if(reservationsList.Count< quantity)
            {
                return true;
            }

            return false;
        }

        private List<Libraries> AvailableLibraries(int id)
        {
            List<Books_Availability> bookAvailability = db.Books_Availability.Where(x => x.quantity > 0 && x.book_id == id).ToList();
            var librariesList = new List<Libraries>();
            foreach (var available in bookAvailability)
            {
                librariesList.Add(available.Libraries);
            }
            return librariesList;
        }
    }
}