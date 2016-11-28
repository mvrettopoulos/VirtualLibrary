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
        [EmailAddress]
        [Display(Name = "User Email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(15, MinimumLength = 3)]
        [Display(Name = "User Name")]
        [RegularExpression(@"(\S)+", ErrorMessage = " White Space is not allowed in User Names")]
        [ScaffoldColumn(false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(15, MinimumLength = 3)]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [MinimumAge(18)]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}")]
        public string Date_of_Birth { get; set; }

        [Required(ErrorMessage = "Membership ID is required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Membership ID must be a 12-digit number")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Membership ID must be a 12-digit number")]
        [Display(Name = "Membership ID")]
        public string membership_id_string { get; set; }

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

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(15, MinimumLength = 3)]
        [Display(Name = "User Name")]
        [RegularExpression(@"(\S)+", ErrorMessage = " White Space is not allowed in User Names")]
        [ScaffoldColumn(false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(15, MinimumLength = 3)]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [MinimumAge(18)]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}")]
        public string Date_of_Birth { get; set; }

        [Required(ErrorMessage = "Membership ID is required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Membership ID must be a 12-digit number")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Membership ID must be a 12-digit number")]
        [Display(Name = "Membership ID")]
        public string membership_id_string { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<UserEditRoleView> roles { get; set; }

        public CreateUserView()
        {
            roles = new List<UserEditRoleView>();
        }

    }

}
