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


        // GET: Reservations/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
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
            var model = new ReservationsViewModel();
            model.book = reservation.Books.title;
            model.check_in = Convert.ToBoolean(reservation.check_in);
            model.check_out = Convert.ToBoolean(reservation.check_out);
            model.library = reservation.Libraries.University_Name;
            model.renewTimes = Convert.ToInt32(reservation.renewTimes);
            model.reserved_date = reservation.reserved_date;
            model.return_date = reservation.return_date;
            model.username = reservation.Users.username;
            model.id = reservation.id;


            return PartialView("_Edit", model);
        }
        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReservationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                Reservations reservation = new Reservations();
                reservation.id = Convert.ToInt32(model.id);
                reservation.check_in = model.check_in;
                reservation.check_out = model.check_out;

                try
                {
                    db.Entry(reservation).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Database error:", e);
                }
                log.Info("Author updated.");
                return Json(new { success = true });
            }
            return PartialView("_Edit", model);
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
            var reservation = db.Author.Find(id);
            db.Author.Remove(reservation);
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



    }
}