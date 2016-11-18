using Microsoft.AspNet.Identity;
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
            var user_id = User.Identity.GetUserId();
            var librarian = db.Users.Where(u => u.aspnet_user_id == user_id).Single();
            var libraries_ids = librarian.Librarians.Select(x => x.library_id);
            
            var books_Availability = db.Books_Availability.Include(b => b.Libraries).Include(b => b.Books).Where(ba => libraries_ids.Contains(ba.library_id));

            return View(books_Availability.ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        // GET: Books_Availability/Create
        public ActionResult Create()
        {
            var user_id = User.Identity.GetUserId();
            var librarian = db.Users.Where(u => u.aspnet_user_id == user_id).Single();
            var libraries_ids = librarian.Librarians.Select(x => x.library_id);

            ViewBag.library_id = new SelectList(db.Libraries.Where(ba => libraries_ids.Contains(ba.id)), "id", "University_Name");
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
            if (books_Availability.quantity != books_Availability.reserved + books_Availability.available)
            {
                ModelState.AddModelError("", "Reserved and Available MUST be equal to Quantity");
            }
            if (db.Books_Availability.Any(ba => ba.book_id == books_Availability.book_id && ba.library_id == books_Availability.library_id))
            {
                ModelState.AddModelError("", "Book already exists in this library");
            }
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
            if (books_Availability.quantity != books_Availability.reserved + books_Availability.available)
            {
                ModelState.AddModelError("", "Reserved and Available MUST be equal to Quantity");
            }
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
