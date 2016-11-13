﻿using System;
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
    [Authorize]
    public class CategoriesController : Controller
    {
        private VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Index()
        {
            var categories = db.Category;
            if (categories != null)
            {
                return View(categories.ToList());
            }
            else
            {
                return View("Categories");
            }
        }
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public ActionResult Create()
        {
            return PartialView("_Create");
        }
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category();
                category.Description = model.description;
                db.Category.Add(category);
                db.SaveChanges();
                log.Info("Category created.");
                return Json(new { success = true });
            }
            return PartialView("_Create", model);
        }
        [Authorize(Roles = "Admin, Moderator")]
        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Category.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }
            var model = new CategoryViewModel();
            model.description = category.Description;
            return PartialView("_Edit", model);
        }
        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category();
                category.id = Convert.ToInt32(model.id);
                category.Description = model.description;
                try
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Database error:", e);
                }
                log.Info("Category updated.");
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
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", @category);
        }
        // POST: Categories/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Category.Find(id);
            category.Books.Clear();
            db.Category.Remove(category);
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