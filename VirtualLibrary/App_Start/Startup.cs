using VirtualLibrary.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(VirtualLibrary.App_Start.Startup))]
namespace VirtualLibrary.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            InitializeAdministrator();
        }

        public async void InitializeAdministrator()
        {
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
                var roleresult = RoleManager.Create(new IdentityRole(roleAdmin));
            }
            if (!RoleManager.RoleExists(roleGuest))
            {
                var roleresult = RoleManager.Create(new IdentityRole(roleGuest));
            }
            if (!RoleManager.RoleExists(roleModerator))
            {
                var roleresult = RoleManager.Create(new IdentityRole(roleModerator));
            }
            if (!RoleManager.RoleExists(roleUser))
            {
                var roleresult = RoleManager.Create(new IdentityRole(roleUser));
            }
            //Create User=Admin with password=123456
            var user = new ApplicationUser { UserName = email, Email = email };
            var adminresult = await UserManager.CreateAsync(user, password);
            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var currentUser = UserManager.FindByName(user.UserName);
                var result = UserManager.AddToRole(currentUser.Id, roleAdmin);
            }
        }
    }
}
