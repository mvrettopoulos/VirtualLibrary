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
                book.title = model.title;
                book.description = model.description;
                book.isbn = model.isbn;
                book.publisher = model.publisher;
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
                }


                ////assign values at new book
                book.views = 0;
                db.Books.Add(book);
                db.SaveChanges();


                return Json(new { success = true });
            }

            return PartialView(model);
        }

        // POST:Upload Profile Picture
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            return PartialView(file);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
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
            int[] currentAuthorsIdAsArray = new int[books.Author.Count()];
            var x = 0;
            var y = 0;
            foreach( var author2 in books.Author.ToList() )
            {
                currentAuthorsIdAsArray[x] = author2.id;
                x++;
            }
            
            int[] currentCategoriesIdAsArray = new int[books.Category.Count()];
            foreach (var category2 in books.Category.ToList())
            {
                currentCategoriesIdAsArray[y] = category2.id;
                y++;
            }
            ViewBag.Authors = new MultiSelectList(db.Author.ToList(), "id", "author_name", null, currentAuthorsIdAsArray);
            ViewBag.Categories = new MultiSelectList(db.Category.ToList(), "id", "Description", null, currentCategoriesIdAsArray);
            var model = new BooksViewModel { id = books.id, description = books.description, isbn = books.isbn, title = books.title, publisher = books.publisher, ThisAuthor = null, ThisCategory = null, AllAuthors = ViewBag.Authors, AllCategories = ViewBag.Categories };
  
            return PartialView(model);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(BooksViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
               
                Books bookToUpdate = db.Books.Where(c => c.id == model.id).Single();
                bookToUpdate.title = model.title;
                bookToUpdate.description = model.description;
                bookToUpdate.isbn = model.isbn;
                bookToUpdate.publisher = model.publisher;
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
                db.SaveChanges();

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
