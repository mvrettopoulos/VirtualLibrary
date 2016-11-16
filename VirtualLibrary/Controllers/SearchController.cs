using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualLibrary.Models;

namespace VirtualLibrary.Controllers
{
    public class SearchController : Controller
    {
        VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
  (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Search
        public ActionResult Index()
        {
            ViewBag.Categories = new MultiSelectList(db.Category.ToList(), "ID", "Description", null);
            return View();
        }

        // GET: Search/Books
        public ActionResult Search()
        {
            return PartialView("Search", db.Books.OrderByDescending(s => s.views).ToList());
        }

        // GET: Search/Books
        [HttpPost]
        public ActionResult SearchBooks(FormCollection form)
        {
            string command = form["command"];
            log.Info(command);
            if (command == "Search")
            {
                string search = form["search"];
                if (search == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                log.Info("Search");
                var books = db.Books.Where(b => b.title.Contains(search)
                || b.isbn.Contains(search) || b.publisher.Contains(search)
                || b.description.Contains(search)).ToList();
                if (books.Count == 0)
                {
                    ViewBag.StatusMessage = "No Books found!!!";
                }

                return PartialView("Search", books);
            }
            else if (command == "AdvancedSearch")
            {
                log.Info("AdvancedSearch");
                string authorName = form["author"];
                string words = form["words"];
                string categories = form["category"];
                var booksList = new List<Books>();
                if (authorName != string.Empty && categories == null && words == string.Empty)
                {
                    log.Info("Search only for author");
                    booksList.AddRange(db.Books.Where(b => b.Author.Any(a => a.author_name == authorName)).ToList());
                }
                else if (authorName != string.Empty && categories != null && words == string.Empty)
                {
                    log.Info("Search for author and categories");
                    string[] categoriesList = categories.Split(',');
                    foreach (var cat in categoriesList)
                    {
                        int catId = Convert.ToInt32(cat);
                        booksList.AddRange(db.Books.Where(b => b.Category.Any(c => c.id == catId) && b.Author.Any(a => a.author_name == authorName)).ToList());
                    }
                }
                else if (authorName != string.Empty && categories != null && words != string.Empty)
                {
                    log.Info("Search for author and categories and words");
                    string[] categoriesList = categories.Split(',');
                    foreach (var cat in categoriesList)
                    {
                        int catId = Convert.ToInt32(cat);
                        booksList.AddRange(db.Books.Where(b => b.Category.Any(c => c.id == catId) && b.Author.Any(a => a.author_name == authorName) && (b.description.Contains(words) || b.title.Contains(words))).ToList());
                    }
                }
                else if (authorName == string.Empty && categories != null && words == string.Empty)
                {
                    log.Info("Search only for categories");
                    string[] categoriesList = categories.Split(',');
                    foreach (var cat in categoriesList)
                    {
                        int catId = Convert.ToInt32(cat);
                        booksList.AddRange(db.Books.Where(b => b.Category.Any(c => c.id == catId)).ToList());
                    }
                }
                else if (authorName == string.Empty && categories == null && words != string.Empty)
                {
                    log.Info("Search only for words");
                    booksList.AddRange(db.Books.Where(b => b.title.Contains(words)
                || b.isbn.Contains(words) || b.publisher.Contains(words)
                || b.description.Contains(words)).ToList());
                }
                else if (authorName == string.Empty && categories != null && words != string.Empty)
                {
                    log.Info("Search for categories and words");
                    string[] categoriesList = categories.Split(',');
                    foreach (var cat in categoriesList)
                    {
                        int catId = Convert.ToInt32(cat);
                        booksList.AddRange(db.Books.Where(b => b.Category.Any(c => c.id == catId) && (b.description.Contains(words) || b.title.Contains(words))).ToList());
                    }

                }
                booksList = booksList.OrderByDescending(b => b.views).ToList();
                if (booksList.Count == 0)
                {
                    ViewBag.StatusMessage = "No Books found!!!";
                }
                return PartialView("Search", booksList);
            }
            return PartialView("Search");
        }

        // GET: Search/Book/Details/5
        [AllowAnonymous]
        public ActionResult GetBook(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            book.views = ++book.views;
            try
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                   log.Error("Entity of type "+eve.Entry.Entity.GetType().Name+" in state " + eve.Entry.State+" has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        log.Error("- Property: "+ ve.PropertyName + ", Error: "+ve.ErrorMessage);
                    }
                }
            }
            return View(book);
        }
    }
}