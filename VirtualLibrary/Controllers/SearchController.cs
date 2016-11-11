using System;
using System.Collections.Generic;
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
        // GET: Search
        public ActionResult Index()
        {
            return View(db.Books.OrderByDescending(s => s.views).ToList());
        }

        // GET: Search/Books
        public ActionResult SearchBooks()
        {
            return View();
        }

        // GET: Search/Book/Details/5
        [AllowAnonymous]
        public ActionResult GetBook(int? id)
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
    }
}