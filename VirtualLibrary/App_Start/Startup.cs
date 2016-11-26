using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Configuration;
using Microsoft.Owin.Security.Twitter;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using VirtualLibrary.Models;

[assembly: OwinStartup(typeof(VirtualLibrary.App_Start.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace VirtualLibrary.App_Start
{
    public class Startup
    {
        private readonly VirtualLibraryEntities db = new VirtualLibraryEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Configuration(IAppBuilder app)
        {
            log.Info("Initialization");
            ConfigureAuth(app);
            InitializeAdministrator();
        }
        public void ConfigureAuth(IAppBuilder app)
        {
            log.Info("Initialize Authentication");
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            // Uncomment the following lines to enable logging in with third party login providers
            app.UseMicrosoftAccountAuthentication(
                clientId: ConfigurationManager.AppSettings["MicrosoftClientID"],
                clientSecret: ConfigurationManager.AppSettings["MicrosoftClientSecret"]);
            app.UseTwitterAuthentication(new TwitterAuthenticationOptions
            {
                ConsumerKey = ConfigurationManager.AppSettings["TwiterClientID"],
                ConsumerSecret = ConfigurationManager.AppSettings["TwiterClientSecret"],
                BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(new[]
               {
                    "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                    "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                    "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                    "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                    "5168FF90AF0207753CCCD9656462A212B859723B", //DigiCert SHA2 High Assurance Server C‎A 
                    "B13EC36903F8BF4701D498261A0802EF63642BC3" //DigiCert High Assurance EV Root CA
                })
            });
            app.UseFacebookAuthentication(
               appId: ConfigurationManager.AppSettings["FacebookClientID"],
               appSecret: ConfigurationManager.AppSettings["FacebookClientSecret"]);
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["GoogleClientID"],
                ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"]
            });
        }
        public void InitializeAdministrator()
        {
            log.Info("Initialize Administrator");
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            string roleAdmin = "Admin";
            string roleGuest = "Guest";
            string roleModerator = "Moderator";
            string roleUser = "User";
            string password = ConfigurationManager.AppSettings["AdminPass"];
            string email = ConfigurationManager.AppSettings["AdminUser"];

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists(roleAdmin))
            {
                RoleManager.Create(new IdentityRole(roleAdmin));
            }
            if (!RoleManager.RoleExists(roleGuest))
            {
                RoleManager.Create(new IdentityRole(roleGuest));
            }
            if (!RoleManager.RoleExists(roleModerator))
            {
                RoleManager.Create(new IdentityRole(roleModerator));
            }
            if (!RoleManager.RoleExists(roleUser))
            {
                RoleManager.Create(new IdentityRole(roleUser));
            }
            //Create User=Admin with password=123456
            var user = new ApplicationUser { UserName = email, Email = email };
            var adminresult = UserManager.Create(user, password);
            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var currentUser = UserManager.FindByName(user.UserName);
                var libraryUser = new Users();
                libraryUser.active = true;
                libraryUser.username = email;
                libraryUser.aspnet_user_id = currentUser.Id;
                db.Users.Add(libraryUser);
                db.SaveChanges();

                UserManager.AddToRole(currentUser.Id, roleAdmin);
            }
        }
    }
}