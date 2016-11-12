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
    public class BooksController : Controller
    {
        private VirtualLibraryEntities db = new VirtualLibraryEntities();

        // GET: Books
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Books.ToList());
        }

        // GET: Books/Details/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.authorColumns = new SelectList(db.Author, "id", "author_name");
            ViewBag.categoryColumns = new SelectList(db.Category, "id", "Description");
            return PartialView("Create");
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Create(VirtualLibrary.Models.Books books)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(books);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView(books);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return PartialView(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit([Bind(Include = "id,title,author_id,description,category_id,image,isbn,publisher")] Books books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(books).State = EntityState.Modified;
                ViewBag.authorColumns = new SelectList(db.Author, "id", "author_name");
                ViewBag.categoryColumns = new SelectList(db.Category, "id", "Description");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(books);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return PartialView(books);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteConfirmed(int id)
        {
            Books books = db.Books.Find(id);
            db.Books.Remove(books);
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
