using LoginTemplateExample.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace LoginTemplateExample.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private Templa db = new DashboardEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {

            return View();
        }
    }

       
}