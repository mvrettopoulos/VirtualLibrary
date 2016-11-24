using VirtualLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using EntityFramework.Extensions;

namespace VirtualLibrary.Controllers
{
    public class LibrariesController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Authorize(Roles = "Admin")]
        // GET: Libraries
        public ActionResult Index()
        {
            return View(db.Libraries.ToList());
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
            return PartialView(library);
        }

        // POST Libraries/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            var library = db.Libraries.Find(id);
            db.Librarians.Where(c => c.library_id == id).Delete();
            db.Books_Availability.Where(c => c.library_id == id).Delete();
            db.Libraries.Remove(library);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Libraries/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return PartialView("Create");
        }

        // POST Libraries/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(VirtualLibrary.Models.Libraries library)
        {

            if (ModelState.IsValid)
            {
                db.Libraries.Add(library);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("Create", library);
        }

        // GET: Libraries/Edit/1
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
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
            return PartialView(library);
        }

        // POST: Libraries/Edit/1
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult _Edit(int? id)
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
                    return Json(new { success = true });
                }
                catch (System.Data.DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return PartialView(libraryToUpdate);
        }


        // GET: Libraries/Librarians/1
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Librarians(int? id)
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
            var users = GetLibrarians((int)id);

            var model = new UsersForLibraryViewModel { AllLibrarians = users, Library = library, ThisLibrarian = null};

            return PartialView(model);
        }


        // POST: Libraries/Librarians/1
        [HttpPost, ActionName("Librarians")]
        [ValidateAntiForgeryToken]
        public ActionResult Librarians(int? id, UsersForLibraryViewModel model)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                Librarians is_librarian = new Librarians();
                Libraries LibraryLibrarian = db.Libraries.Where(c => c.id == id).Single();
                is_librarian.Libraries = LibraryLibrarian;
                db.Librarians.Where(c => c.library_id == id).Delete();
                db.SaveChanges();
                if (model.ThisLibrarian != null)
                {
                    foreach (int librarian_id in model.ThisLibrarian)
                    {
                        Users UserLibrarian = db.Users.Where(c => c.id == librarian_id).Single();
                        is_librarian.Users = UserLibrarian;
                        db.Librarians.Add(is_librarian);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return PartialView(model);
        }

        private IEnumerable<SelectListItem> GetLibrarians(int libraryid)
        {
            var selected = db.Librarians.Where(c => c.library_id == libraryid).Select(u => u.username_id).ToList();
            var all_librarians = db.Users.Where(u => u.AspNetUsers.AspNetRoles.Any(r => r.Name == "Moderator")).Select(x =>
                                new SelectListItem
                                {
                                    Value = x.id.ToString(),
                                    Text = x.username,
                                }); ;

            return new MultiSelectList(all_librarians, "Value", "Text", selected);
        }


        private IEnumerable<SelectListItem> GetAuthors(int authorid)
        {
            var selected = db.Books.Where(c => c.Author.Any(x=>x.id == authorid)).ToList();
            var all_authors = db.Author.ToList().Select(x =>
                                new SelectListItem
                                {
                                    Value = x.id.ToString(),
                                    Text = x.author_name,
                                }); ;

            return new MultiSelectList(all_authors, "Value", "Text", selected);
        }

    }
}