using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualLibrary.Models;

namespace VirtualLibrary.Controllers
{
    public class UsersForDashboardController : Controller
    {
        private VirtualLibraryEntities db = new VirtualLibraryEntities();

        // GET: UsersForDashboard
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.AspNetUsers);
            ViewBag.inactive_users = db.Users.Include(u => u.AspNetUsers).Where(u => u.active == false).ToList();
            return View(users.ToList());
        }



        // GET: UsersForDashboard/Activate/5
        public ActionResult Activate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return PartialView(users);
        }

        // POST: UsersForDashboard/Activate/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public ActionResult ActivateConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            users.active = true;
            db.Entry(users).State = EntityState.Modified;
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
