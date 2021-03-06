﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VirtualLibrary.Models;
namespace VirtualLibrary.Controllers
{
    [Authorize]
    public class AuthorsController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Index()
        {
            var authors = db.Author;
            if (authors != null)
            {
                return View(authors.ToList());
            }
            else
            {
                return View("Index");
            }
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Create()
        {
            return PartialView("_Create");
        }
       
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                Author author = new Author();
                author.author_name = model.AuthorName;
                db.Author.Add(author);
                db.SaveChanges();
                log.Info("Author created.");
                return Json(new { success = true });
            }
            return PartialView("_Create", model);
        }
        [Authorize(Roles = "Admin, Moderator")]
        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Author.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Author.Find(id);

            if (author == null)
            {
                return HttpNotFound();
            }
            var model = new AuthorViewModel();
            model.AuthorName = author.author_name;
            return PartialView("_Edit", model);
        }
        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                Author author = new Author();
                author.id = Convert.ToInt32(model.Id);
                author.author_name = model.AuthorName;
                try
                {
                    db.Entry(author).State = EntityState.Modified;
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
       
        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Author.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", @author);
        }
        // POST: Authors/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var author = db.Author.Find(id);
            author.Books.Clear();
            db.Author.Remove(author);
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