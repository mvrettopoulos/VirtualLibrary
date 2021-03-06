﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualLibrary.App_Start;
using VirtualLibrary.Models;

namespace VirtualLibrary.Controllers
{
    public class UsersForDashboardController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        // GET: UsersForDashboard
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.AspNetUsers);
            ViewBag.inactive_users = db.Users.Include(u => u.AspNetUsers).Where(u => !u.active).ToList();
            return View(users.ToList());
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
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
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult ActivateConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            users.active = true;
            db.Entry(users).State = EntityState.Modified;

            var currentUser = UserManager.FindByName(users.username);
            UserManager.RemoveFromRole(currentUser.Id, "Guest");
            UserManager.AddToRole(currentUser.Id, "User");
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        // GET: UsersForDashboard/Switch_State/5
        public ActionResult Switch_State(int? id)
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

        // POST: UsersForDashboard/Switch_State/5
        [HttpPost, ActionName("Switch_State")]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult Switch_StateConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            users.bad_user = !users.bad_user;
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
