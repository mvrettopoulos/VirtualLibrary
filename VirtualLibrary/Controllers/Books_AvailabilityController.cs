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
    public class Books_AvailabilityController : Controller
    {
        private VirtualLibraryEntities db = new VirtualLibraryEntities();

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        // GET: Books_Availability
        public ActionResult Index()
        {
            var books_Availability = db.Books_Availability.Include(b => b.Libraries).Include(b => b.Books);
            return View(books_Availability.ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        // GET: Books_Availability/Create
        public ActionResult Create()
        {
            ViewBag.library_id = new SelectList(db.Libraries, "id", "University_Name");
            ViewBag.book_id = new SelectList(db.Books, "id", "title");
            return PartialView("Create");
        }

        [HttpPost, ActionName("Create")]
        [Authorize(Roles = "Admin, Moderator")]
        // POST: Books_Availability/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        public ActionResult Create(Books_Availability books_Availability)
        {
            if (ModelState.IsValid)
            {
                db.Books_Availability.Add(books_Availability);
                db.SaveChanges();
                return Json(new { success = true });
            }

            ViewBag.library_id = new SelectList(db.Libraries, "id", "University_Name", books_Availability.library_id);
            ViewBag.book_id = new SelectList(db.Books, "id", "title", books_Availability.book_id);
            return PartialView("Create", books_Availability);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        // GET: Books_Availability/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books_Availability books_Availability = db.Books_Availability.Find(id);
            if (books_Availability == null)
            {
                return HttpNotFound();
            }
            ViewBag.library_id = new SelectList(db.Libraries, "id", "University_Name", books_Availability.library_id);
            ViewBag.book_id = new SelectList(db.Books, "id", "title", books_Availability.book_id);
            return PartialView("Edit", books_Availability);
        }

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin, Moderator")]
        // POST: Books_Availability/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Books_Availability books_Availability)
        {
            if (ModelState.IsValid)
            {
                db.Entry(books_Availability).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.library_id = new SelectList(db.Libraries, "id", "University_Name", books_Availability.library_id);
            ViewBag.book_id = new SelectList(db.Books, "id", "title", books_Availability.book_id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        // GET: Books_Availability/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books_Availability books_Availability = db.Books_Availability.Find(id);
            if (books_Availability == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", books_Availability);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Moderator")]
        // POST: Books_Availability/Delete/5
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Books_Availability books_Availability = db.Books_Availability.Find(id);
            db.Books_Availability.Remove(books_Availability);
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
