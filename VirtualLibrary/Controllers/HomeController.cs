using VirtualLibrary.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace VirtualLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();

        [RequireHttps]
        public ActionResult Index()
        {
            var topBooks = db.Books.OrderByDescending(s =>s.views).Take(8).ToList();
            return View(topBooks);
        }
    }

       
}