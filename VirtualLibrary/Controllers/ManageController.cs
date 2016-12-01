using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VirtualLibrary.Models;
using VirtualLibrary.App_Start;
using System.IO;
using System.Data.Entity;
using System.Data;
using System.Collections.Generic;
using System.Globalization;

namespace VirtualLibrary.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
  (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.ChangeUserNameSuccess ? "Your username has been changed."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : message == ManageMessageId.FileUploadSuccess ? "Your image has been changed."
                : message == ManageMessageId.FileUploadError ? "You have not selected an image."
                : message == ManageMessageId.FileTypeError ? "You need to upload a .png file."
                : message == ManageMessageId.EditProfile ? "Your profile's infos has been changed."
                : "";


            var userId = User.Identity.GetUserId();
            var model = db.Users.SingleOrDefault(s => s.aspnet_user_id == userId);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/ChangeUserName
        public ActionResult ChangeUserName()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ChangeUserNameViewModel model = new ChangeUserNameViewModel();
            model.UserName = user.UserName;
            return View(model);
        }

        //
        // POST: /Manage/ChangeUserName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUserName(ChangeUserNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                user.UserName = model.UserName;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangeUserNameSuccess });
                }
                AddErrors(result);
                return View(model);

            }
            return View(model);
        }


        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);

        }

        //GET: Upload Profile Picture
        [HttpGet]
        [Authorize]
        public ActionResult GetFileUpload()
        {

            return PartialView("FileUpload");
        }
        // POST:Upload Profile Picture
        [HttpPost]
        [Authorize]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(s => s.aspnet_user_id == userId);

            if (file != null)
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png" };

                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (supportedTypes.Contains(fileExt))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        user.image = array;
                        try
                        {
                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (DataException e)
                        {
                            ms.Close();
                            log.Error("Database error:", e);
                        }
                        log.Info("Image updated.");
                        ms.Close();

                    }
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("", "The image format is not supported! The following format are supported: jpg, jpeg, png");
                    return PartialView("FileUpload");
                }
            }
            ModelState.AddModelError("", "You have not selected any file");
            return PartialView("FileUpload");
        }

        //GET:EditProfile
        [HttpGet]
        [Authorize]
        public ActionResult GetEditProfile()
        {
            string username = User.Identity.Name;

            // Fetch the userprofile
            Users user = db.Users.FirstOrDefault(u => u.username.Equals(username));
            AspNetUsers aspNetUser = db.AspNetUsers.FirstOrDefault(m => m.UserName.Equals(username));

            // Construct the viewmodel
            UserProfileEdit model = new UserProfileEdit();
            model.firstName = user.first_name;
            model.lastName = user.last_name;
            model.Date_of_Birth = user.date_of_birth;
            model.Email = aspNetUser.Email;
            model.username = username;

            return PartialView("EditProfile", model);
        }
        
        //POST:EditProfile
        [HttpPost]
        [Authorize]
        public ActionResult EditProfile(UserProfileEdit model)
        {

            if (ModelState.IsValid)
            {
                // Get the userprofile
                Users user = db.Users.FirstOrDefault(u => u.username.Equals(model.username));
                AspNetUsers aspNetUser = db.AspNetUsers.FirstOrDefault(m => m.UserName.Equals(model.username));

                // Update fields
                user.first_name = model.firstName;
                user.last_name = model.lastName;
                user.date_of_birth = model.Date_of_Birth;
                aspNetUser.Email = model.Email;

                try
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.Entry(aspNetUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Database error:", e);
                }
                log.Info("User updated.");

                return Json(new { success = true });
            }
            return PartialView("EditProfile", model);
        }


        //GET: Extend Loan
        [HttpGet]
        [Authorize]
        public ActionResult GetExtendLoan(int id)
        {
            Reservations reservation = db.Reservations.FirstOrDefault(m => m.id == (id));

            if (reservation.renewTimes == 0)
            {
                ViewBag.ErrorMessage = "You cannot extend anymore your reservation!!!";
                return PartialView("ExtendLoan", null);
            }
            ExtendLoanView model = new ExtendLoanView();
            model.id = reservation.id;
            var newModel = GetAvailable(model);
            if (newModel == null)
            {
                ViewBag.ErrorMessage = "Extend loan is not available!!!";
                return PartialView("ExtendLoan", null);
            }

            return PartialView("ExtendLoan", newModel);

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ExtendLoan(ExtendLoanView model)
        {
            if (ModelState.IsValid)
            {
                string format = "dd-mm-yyyy";
                DateTime dateTime;

                if (!DateTime.TryParseExact(model.return_date, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    ModelState.AddModelError("return_date", "Date format is not correct!!!Format expected is dd-mm-yyyy!");
                    return PartialView("ExtendLoan", model);
                }
                var newModel = GetAvailable(model);
                if (newModel == null)
                {
                    ViewBag.ErrorMessage = "Extend loan is not available!!!";
                    return PartialView("ExtendLoan", model);
                }

                Reservations reservation = db.Reservations.FirstOrDefault(m => m.id == (model.id));
                var newReturnDate = Convert.ToDateTime(model.return_date);
                if (reservation.return_date > newReturnDate || reservation.return_date.Value.AddDays(7) < newReturnDate)
                {
                    ViewBag.StatusMessage = "The dates you selected are invalid!!!";
                    ModelState.AddModelError("return_date", "The date you selected is invalid!!!");
                   
                    return PartialView("ExtendLoan", newModel);
                }
                reservation.return_date = newReturnDate;
                reservation.renewTimes = --reservation.renewTimes;
                try
                {
                    db.Entry(reservation).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DataException e)
                {
                    log.Error("Database error:", e);
                }
                log.Info("Reservation updated.");
                return Json(new { success = true });
            }
            return PartialView("ExtendLoan", model);
        }

        private ExtendLoanView GetAvailable(ExtendLoanView model)
        {
            Reservations reservation = db.Reservations.FirstOrDefault(m => m.id == (model.id));
            Books_Availability availableBook = db.Books_Availability.FirstOrDefault(b => b.book_id == reservation.book_id && b.library_id == reservation.library_id);
            List<Reservations> reservationsList = db.Reservations.Where(x => x.book_id == reservation.book_id && x.library_id == reservation.library_id && reservation.reserved_date >= x.reserved_date && reservation.return_date <= x.return_date).OrderBy(x => x.reserved_date).ToList();
            if (reservationsList.Count <= availableBook.quantity)
            {
                model.minDate = reservation.return_date.Value.AddDays(1);
                model.maxDate = reservation.return_date.Value.AddDays(7);
            }
            else
            {
                var reservationLast = reservationsList.Last();
                if (reservation.return_date.Value.AddDays(1) == reservationLast.reserved_date)
                {
                    return null;
                }

                model.minDate = reservation.return_date.Value.AddDays(1);
                model.maxDate = reservationLast.reserved_date.Value.AddDays(-1);
            }
            return model;
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
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            ChangeUserNameSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            FileUploadSuccess,
            FileUploadError,
            FileTypeError,
            EditProfile,
            Error

        }

        #endregion
    }
}