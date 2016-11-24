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
        public int id { get; set; }
        public string minDate { get; set; }
        public string maxDate { get; set; }

        [Required]
        [Display(Name = "Reservation Date Expiration")]
        public string return_date { get; set; }
    }

}





