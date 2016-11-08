using VirtualLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace VirtualLibrary.Controllers
{
    public class LibrariesController : Controller
    {
        private VirtualLibraryEntities db = new VirtualLibraryEntities();

        // GET: Libraries
        public ActionResult Index()
        {
            return View(db.Libraries.ToList());
        }

        // GET: Libraries/Details/1
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var library = db.Libraries.Find(id);
            if (library == null)
            {
                return HttpNotFound();
            }
            return View(library);
        }

        // GET: Libraries/Delete/1
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var library = db.Libraries.Find(id);
            if (library == null)
            {
                return HttpNotFound();
            }
            return View(library);
        }

        // POST Libraries/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(FormCollection fcNotUsed, int id)
        {
            var library = db.Libraries.Find(id);
            db.Libraries.Remove(library);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Libraries/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST Libraries/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "University_Name, Building, Location")]VirtualLibrary.Models.Libraries library)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Libraries.Add(library);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (System.Data.DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(library);
        }

        // GET: Libraries/Edit/1
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(FormCollection fcNotUsed, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var library = db.Libraries.Find(id);
            if (library == null)
            {
                return HttpNotFound();
            }
            return View(library);
        }

        // POST: Libraries/Edit/1
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var libraryToUpdate = db.Libraries.Find(id);
            if (TryUpdateModel(libraryToUpdate, "",
               new string[] { "University_Name", "Building", "Location" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (System.Data.DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(libraryToUpdate);
        }

    }
}