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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [RequireHttps]
        public ActionResult Index()
        {
            var topBooks = db.Books.OrderByDescending(s =>s.views).Take(8).ToList();
            return View(topBooks);
        }
    }

       
}