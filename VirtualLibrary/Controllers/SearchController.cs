using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualLibrary.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        // GET: Search/Books
        public ActionResult SearchBooks()
        {
            return View();
        }

        // GET: Search/Book
        public ActionResult GetBook()
        {
            return View();
        }
    }
}