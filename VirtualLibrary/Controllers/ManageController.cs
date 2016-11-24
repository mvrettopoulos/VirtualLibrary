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

namespace VirtualLibrary.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
  (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private VirtualLibraryEntities db = new VirtualLibraryEntities();

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
            //new IndexViewModel
            //{
            //   HasPassword = HasPassword(),
            //    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
            //    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
            //    Logins = await UserManager.GetLoginsAsync(userId),
            //    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            //};
            return View(model);
        }

        //
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
        public ActionResult GetFileUpload()
        {

            return PartialView("FileUpload"); // toDo
        }
        // POST:Upload Profile Picture
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            ManageMessageId? message;
            var userId = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(s => s.aspnet_user_id == userId);

            if (file != null)
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png" };

                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (supportedTypes.Contains(fileExt))
                {
                    //file.FileName.Contains(".png")
                    // save the image path path to the database or you can send image
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
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
                    message = ManageMessageId.FileUploadSuccess;
                }
                else
                {
                    message = ManageMessageId.FileTypeError;
                }
            }
            else
            {
                message = ManageMessageId.FileUploadError;
            }


            // after successfully uploading redirect the user
            return RedirectToAction("Index", new { Message = message });
        }

        //GET:EditProfile
        [HttpGet]
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

            return PartialView("EditProfile",model);
        }
        //POST:EditProfile
        [HttpPost]
        public ActionResult EditProfile(UserProfileEdit userprofile)
        {
            if (!ModelState.IsValid)
            {
                ManageMessageId? message;
                string username = User.Identity.Name;
                // Get the userprofile
                Users user = db.Users.FirstOrDefault(u => u.username.Equals(username));
                AspNetUsers aspNetUser = db.AspNetUsers.FirstOrDefault(m => m.UserName.Equals(username));

                // Update fields
                user.first_name = userprofile.firstName;
                user.last_name = userprofile.lastName;
                user.date_of_birth = userprofile.Date_of_Birth;
                aspNetUser.Email = userprofile.Email;
                
                

                db.Entry(user).State = EntityState.Modified;
                db.Entry(aspNetUser).State = EntityState.Modified;

                db.SaveChanges();
                message = ManageMessageId.EditProfile;

                return RedirectToAction("Index", new { Message = message }); // or whatever
            }
            return View(userprofile);
        }


        //GET: Extend Loan
        [HttpGet]
        public ActionResult GetExtendLoan(int id)
        {
            Reservations reservation = db.Reservations.FirstOrDefault(m => m.id==(id));
            DateTime returnDate = Convert.ToDateTime(reservation.return_date);
            Books_Availability availableBook = db.Books_Availability.FirstOrDefault(b => b.book_id==reservation.book_id && b.library_id==reservation.library_id);


            List<Reservations> reservationList = db.Reservations.ToList().Where(x => x.book_id == reservation.book_id 
            && x.library_id == reservation.library_id 
            && x.reserved_date > returnDate && x.reserved_date <= returnDate.AddDays(7))
                               .OrderBy(x => x.reserved_date).ToList();
            ExtendLoanView model = new ExtendLoanView();
            model.id = reservation.id;
            if (reservationList.Count < availableBook.available)
            {
                model.minDate = returnDate.AddDays(1);
                model.maxDate = returnDate.AddDays(7);
                //model.returnDate = reservation.return_date;

                //imerominia max_reservation_date kai id_reservation 
            }
            else
            {
                var reservedDate=reservationList.Last().reserved_date;
                model.minDate = returnDate.AddDays(1);
                model.maxDate = Convert.ToDateTime(reservedDate).AddDays(-1);
                //model.returnDate = reservation.return_date;


                //imerominia max_reservation_date kai id_reservation  apla reservedDate-1
            }

            return PartialView("Loan",model);
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
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