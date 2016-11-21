using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VirtualLibrary.Models
{
    public class UserProfileEdit
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(15, MinimumLength = 3)]
        [Display(Name = "User Name")]
        [RegularExpression(@"(\S)+", ErrorMessage = " White Space is not allowed in User Names")]
        [ScaffoldColumn(false)]
        public string username { get; set; }

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

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        public class MinimumAgeAttribute : ValidationAttribute
        {
            int _minimumAge;
            public MinimumAgeAttribute(int minimumAge)
            {
                _minimumAge = minimumAge;
            }
            public override bool IsValid(object value)
            {
                DateTime date;
                if (DateTime.TryParse(value.ToString(), out date))
                {
                    return date.AddYears(_minimumAge) < DateTime.Now;
                }

                return false;
            }
        }
    }
}
