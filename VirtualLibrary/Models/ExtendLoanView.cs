using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace VirtualLibrary.Models
{
    public class ExtendLoanView
    {
        public int Id { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        [Required]
        [Display(Name = "Reservation Date Expiration")]
        public string return_date { get; set; }
    }

}





