using EntityFramework.Extensions;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualLibrary.Models;

namespace VirtualLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Books
        [Authorize(Roles = "Admin, Moderator")]
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
       
        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Create()
        {
            
            ViewBag.Authors = new MultiSelectList(db.Author.ToList(), "id", "author_name", null);
            ViewBag.Categories = new MultiSelectList(db.Category.ToList(), "id", "Description", null);
            var model = new BooksViewModel { ThisAuthor = null, ThisCategory = null, AllAuthors = ViewBag.Authors, AllCategories = ViewBag.Categories };
            return PartialView(model);
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Create(BooksViewModel model,HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                VirtualLibrary.Models.Books book = new Books();
                //map obj BooksViewModel to ModelDataBase book
                book.title = model.Title;
                book.description = model.Description;
                book.isbn = model.Isbn;
                book.publisher = model.Publisher;
                if (model.ThisAuthor != null)
                {
                    foreach (var author in model.ThisAuthor)
                    {
                        book.Author.Add(db.Author.Find(author));
                    }
                }
                if (model.ThisCategory != null)
                {
                    foreach (var category in model.ThisCategory)
                    {
                        book.Category.Add(db.Category.Find(category));
                    }
                }

                if (file != null)
                {
                    var supportedTypes = new[] { "jpg", "jpeg", "png" };

                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                    if (supportedTypes.Contains(fileExt))
                    {
                        //file.FileName.Contains(".png")
                        // save the image path path to the database or you can send image
                        // directly to database
                        // in-case if you want to store byte[] ie. for DB
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            book.image = array;
                          
                            log.Info("Image updated.");
                            ms.Close();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("image", "The image format is not supported! The following format are supported: jpg, jpeg, png");
                        return PartialView(model);
                    }
                }


                ////assign values at new book
                book.views = 0;
                db.Books.Add(book);
                db.SaveChanges();


                return Json(new { success = true });
            }

            return PartialView(model);
        }

        // GET: Books/Edit/5
        [HttpGet]
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
            ViewBag.Authors = new MultiSelectList(db.Author.ToList(), "id", "author_name", null, books.Author.Select(u => u.id).ToList());
            ViewBag.Categories = new MultiSelectList(db.Category.ToList(), "id", "Description", null, books.Category.Select(u => u.id).ToList());
            var model = new BooksViewModel { Id = books.id, Description = books.description, Isbn = books.isbn, Title = books.title, Publisher = books.publisher, ThisAuthor = null, ThisCategory = null, AllAuthors = ViewBag.Authors, AllCategories = ViewBag.Categories };
  
            return PartialView(model);
        }

        // POST: Books/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(BooksViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
               
                Books bookToUpdate = db.Books.Where(c => c.id == model.Id).Single();
                bookToUpdate.title = model.Title;
                bookToUpdate.description = model.Description;
                bookToUpdate.isbn = model.Isbn;
                bookToUpdate.publisher = model.Publisher;
                bookToUpdate.Author.Clear();
                bookToUpdate.Category.Clear();
                if (model.ThisAuthor != null)
                {
                    foreach (var author in model.ThisAuthor)
                    {
                        bookToUpdate.Author.Add(db.Author.Find(author));
                    }
                }
                if (model.ThisCategory != null)
                {
                    foreach (var category in model.ThisCategory)
                    {
                        bookToUpdate.Category.Add(db.Category.Find(category));
                    }
                }

                if (file != null)
                {
                    var supportedTypes = new[] { "jpg", "jpeg", "png" };

                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                    if (supportedTypes.Contains(fileExt))
                    {
                        //file.FileName.Contains(".png")
                        // save the image path path to the database or you can send image
                        // directly to database
                        // in-case if you want to store byte[] ie. for DB
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            bookToUpdate.image = array;

                            log.Info("Image updated.");
                            ms.Close();
                        }
                    }else
                    {
                        ModelState.AddModelError("image","The image format is not supported! The following format are supported: jpg, jpeg, png");
                        return PartialView(model);
                    }
                }


                try
                {
                    db.Entry(bookToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Error",e);
                }

                return Json(new { success = true });
            }
            return View(model);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Books books = db.Books.Find(id);
            db.Books_Availability.Where(c => c.book_id == books.id).Delete();
            db.Books_Ratings.Where(c => c.book_id == books.id).Delete();
            db.Reservations.Where(c => c.book_id == books.id).Delete();
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
