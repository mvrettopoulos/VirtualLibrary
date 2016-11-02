using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VirtualLibrary.Models
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

    public class RoleEditViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "oldName")]
        public string oldName { get; set; }
    }

    public class UserEditViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string userName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "User Email")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<UserEditRoleView> roles { get; set; }

        public UserEditViewModel()
        {
            roles = new List<UserEditRoleView>();
        }
    }

    public class UserEditRoleView
    {
        public UserEditRoleView(string roleName, bool isRole)
        {
            this.isRole = isRole;
            this.roleName = roleName;
        }

        public UserEditRoleView() { }
        public bool isRole { get; set; }
        public string roleName { get; set; }
    }

    public class CreateUserView
    {
        [Required]
        [EmailAddress]
        [Display(Name = "User Email")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<UserEditRoleView> roles { get; set; }

        public CreateUserView()
        {
            roles = new List<UserEditRoleView>();
        }

    }

}
