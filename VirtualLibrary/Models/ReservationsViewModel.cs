using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualLibrary.Models
{
    public class ReservationsViewModel
    {

        public int id { get; set; }
        public string book { get; set; }
        public string username { get; set; }
        [Required]
        [Display(Name = "Reservation Date")]
        public string reserved_date { get; set; }
        [Required]
        [Display(Name = "Reservation Date Expiration")]
        public string return_date { get; set; }

        public bool check_in { get; set; }
        public bool check_out { get; set; }
        public int renewTimes { get; set; }
        [Required]
        [Display(Name = "Library Building")]
        public string library { get; set; }
    }
}
