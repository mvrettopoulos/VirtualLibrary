using VirtualLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VirtualLibrary.App_Start;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Drawing;

namespace VirtualLibrary.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AccountController()
        {
        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
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
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return this._roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set { this._roleManager = value; }
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                log.Error("Model is not valid.");
                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            using (var appdb = new ApplicationDbContext())
            {
                model.EmailOrUserName = (appdb.Users.Any(p => p.UserName == model.EmailOrUserName)) ?
                model.EmailOrUserName :
                (appdb.Users.Any(p => p.Email == model.EmailOrUserName)) ?
                appdb.Users.SingleOrDefault(p => p.Email == model.EmailOrUserName).UserName :
                model.EmailOrUserName;
            }

            var result = await SignInManager.PasswordSignInAsync(model.EmailOrUserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    log.Info("Sign in succeded.");
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    log.Error("User locked out.");
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    log.Info("User needs verification.");
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    log.Error("Failed to log in.");
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                log.Error("Could not verify code");
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                log.Error("VerifyCode model is not valid");
                return View(model);
            }
            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    log.Info("Verification succeded.");
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    log.Error("User locked out.");
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    log.Error("Failed to log in.");
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                string role = "Guest";
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    var currentUser = UserManager.FindByName(user.UserName);
                    UserManager.AddToRole(currentUser.Id, role);

                    var NewUser = new Users();

                    byte[] array;
                    Image img = Image.FromFile(Server.MapPath("/Content/images/no_available_image.png"));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        array = ms.ToArray();
                        ms.Close();
                    }
                    img.Dispose();

                    NewUser.aspnet_user_id = currentUser.Id;
                    NewUser.active = false;
                    NewUser.bad_user = false;
                    NewUser.username = model.Username;
                    NewUser.first_name = model.firstName;
                    NewUser.last_name = model.lastName;
                    NewUser.date_of_birth = model.Date_of_Birth;
                    DateTime today = DateTime.Today;
                    NewUser.date_of_registration = Convert.ToString(today);
                    NewUser.image = array;
                    NewUser.membership_id = Convert.ToInt64(model.membership_id_string);

                    db.Users.Add(NewUser);
                    db.SaveChanges();

                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action(
                       "ConfirmEmail", "Account",
                       new { userId = user.Id, code = code },
                       protocol: Request.Url.Scheme);
                    SendMail("Please confirm your account by clicking this link: <a href=\""
                                                       + callbackUrl + "\">link</a>", user.Email, "Confirm your account");
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }
            log.Error("Registration model is not valid");
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                SendMail("Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>", user.Email, "Reset Password");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }
        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }
        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }
        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }
        //
        // POST: 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {

            string role = "Guest";

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }


                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var currentUser = UserManager.FindByName(user.UserName);
                    UserManager.AddToRole(currentUser.Id, role);

                    var NewUser = new Users();

                    byte[] array;
                    Image img = Image.FromFile(Server.MapPath("/Content/images/no_available_image.png"));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        array = ms.ToArray();
                        ms.Close();
                    }
                    img.Dispose();
                    NewUser.aspnet_user_id = currentUser.Id;
                    NewUser.active = false;
                    NewUser.bad_user = false;
                    NewUser.username = model.Username;
                    NewUser.first_name = model.firstName;
                    NewUser.last_name = model.lastName;
                    NewUser.date_of_birth = model.Date_of_Birth;
                    DateTime today = DateTime.Today;
                    NewUser.date_of_registration = Convert.ToString(today);
                    NewUser.image = array;
                    NewUser.membership_id = Convert.ToInt64(model.membership_id_string);

                    db.Users.Add(NewUser);
                    db.SaveChanges();

                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {

                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }



                AddErrors(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Users()
        {
            var users = UserManager.Users;
            if (users != null)
            {
                return View(users.ToList());
            }
            else
            {
                return View("Users");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateUser()
        {
            CreateUserView userModel = new CreateUserView();
            userModel.roles = new List<UserEditRoleView>();
            var roleList = RoleManager.Roles.ToList();
            foreach (var role in roleList)
            {
                userModel.roles.Add(new UserEditRoleView(role.Name, false));
            }
            return PartialView("_CreateUser", userModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateUser(CreateUserView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = model.email;
                user.Email = model.email;
                var pass = Guid.NewGuid().ToString().Substring(0, 12);
                var result = UserManager.Create(user, pass);
                if (!result.Succeeded)
                {
                    log.Error("Could not create user");
                    AddErrors(result);
                    return PartialView("_CreateUser", user);
                }
                var currentUser = UserManager.FindByName(user.UserName);
                var newUser = new Users();
                newUser.active = true;
                newUser.aspnet_user_id = currentUser.Id;
                newUser.bad_user = false;
                DateTime today = DateTime.Today;
                newUser.date_of_registration = Convert.ToString(today);
                newUser.username = currentUser.UserName;
                db.Users.Add(newUser);
                db.SaveChanges();
                var callbackUrl = Url.Action(
                        "Login", "Account",
                        new { userId = user.Id },
                        protocol: Request.Url.Scheme);
                SendMail("<h1>New account is available on Virtual Library.</h1>" +
                    "<p>With credentials:<br>" +
                    "username: " + user.Email + "<br>" +
                    "password: " + pass + "</p>" +
                    "<p>You can log in by clicking this link: <a href=\"" + callbackUrl + "\">link</a></p>" +
                    "<p>Please change your password at your first log in in the profile menu!</p>"
                    , user.Email, "Virtual Library Acoount");
                foreach (var role in model.roles)
                {
                    if (role.isRole && !UserManager.IsInRole(user.Id, role.roleName))
                    {
                        UserManager.AddToRole(user.Id, role.roleName);
                    }
                    else if (!role.isRole && UserManager.IsInRole(user.Id, role.roleName))
                    {
                        UserManager.RemoveFromRole(user.Id, role.roleName);
                    }
                }
                log.Info("User created.");
                return Json(new { success = true });
            }
            return PartialView("_CreateUser", model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindById(id.ToString());
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DeleteUser", user);
        }
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUserConfirmed(string id)
        {
            var user = UserManager.FindById(id.ToString());

            var libraryUser = db.Users.Single(s => s.aspnet_user_id == user.Id);
            libraryUser.Librarians.Clear();
            libraryUser.Books_Ratings.Clear();
            libraryUser.Reservations.Clear();
            db.Users.Remove(libraryUser);
            db.SaveChanges();
            UserManager.Delete(user);
            return RedirectToAction("Users");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindById(id.ToString());
            if (user == null)
            {
                return HttpNotFound();
            }
            UserEditViewModel editUser = new UserEditViewModel();
            editUser.userName = user.UserName;
            editUser.email = user.Email;
            editUser.roles = new List<UserEditRoleView>();
            var userRoles = UserManager.GetRoles(user.Id);
            var roleList = RoleManager.Roles.ToList();
            foreach (var role in roleList)
            {
                if (userRoles.Contains(role.Name))
                {
                    editUser.roles.Add(new UserEditRoleView(role.Name, true));
                }
                else
                {
                    editUser.roles.Add(new UserEditRoleView(role.Name, false));
                }
            }
            return PartialView("_EditUser", editUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(model.userName);
                user.Email = model.email;
                var result = UserManager.Update(user);
                if (!result.Succeeded)
                {
                    log.Error("User update failed");
                    AddErrors(result);
                    return PartialView("_EditRole", user);
                }
                foreach (var role in model.roles)
                {
                    if (role.isRole && !UserManager.IsInRole(user.Id, role.roleName))
                    {
                        UserManager.AddToRole(user.Id, role.roleName);
                        log.Info(role.roleName + "-" + role.isRole);
                    }
                    else if (!role.isRole && UserManager.IsInRole(user.Id, role.roleName))
                    {
                        UserManager.RemoveFromRole(user.Id, role.roleName);
                    }
                }
                log.Info("User updated.");
                return Json(new { success = true });
            }
            return PartialView("_EditUser", model);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Roles()
        {
            var roles = RoleManager.Roles;
            if (roles != null)
            {
                return View(roles.ToList());
            }
            else
            {
                return View("Roles");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateRole()
        {
            return PartialView("_CreateRole");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateRole(RoleViewModel createRole)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(createRole.Name);
                var result = RoleManager.Create(role);
                if (!result.Succeeded)
                {
                    log.Error("Role creation failed");
                    AddErrors(result);
                    return PartialView("_CreateRole", createRole);
                }
                log.Info("Role created.");
                return Json(new { success = true });
            }
            return PartialView("_CreateRole", createRole);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteRole(string id)
        {
            if (id == null)
            {
                log.Error("Role id is null.");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = RoleManager.FindById(id.ToString());
            if (role == null)
            {
                log.Error("Role with id " + id + " is null.");
                return HttpNotFound();
            }
            return PartialView("_DeleteRole", role);
        }
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleConfirmed(string id)
        {
            IdentityRole role = RoleManager.FindById(id.ToString());
            RoleManager.Delete(role);
            return RedirectToAction("Roles");
        }
        public ActionResult EditRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = RoleManager.FindById(id.ToString());
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleEditViewModel roleView = new RoleEditViewModel();
            roleView.oldName = role.Name;
            roleView.Name = role.Name;
            return PartialView("_EditRole", roleView);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole([Bind(Include = "name,oldName")] RoleEditViewModel roleView)
        {
            IdentityRole role = RoleManager.FindByName(roleView.oldName);
            role.Name = roleView.Name;
            if (ModelState.IsValid)
            {
                var result = RoleManager.Update(role);
                if (!result.Succeeded)
                {
                    log.Error("Role update failed");
                    AddErrors(result);
                    return PartialView("_EditRole", roleView);
                }
                log.Info("Role updated.");
                return Json(new { success = true });
            }
            return PartialView("_EditRole", roleView);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            base.Dispose(disposing);
        }
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                log.Error(error);
                ModelState.AddModelError("", error);
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        private void SendMail(string message, string to, string Subject)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("acce.team.3@gmail.com");
            msg.To.Add(new MailAddress(to));
            msg.Subject = Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html));
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(msg);
        }
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }
            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }
            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}