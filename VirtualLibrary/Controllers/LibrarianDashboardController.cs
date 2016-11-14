using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualLibrary.Controllers
{
    public class LibrarianDashboardController : Controller
    {
        // GET: LibrarianDashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}