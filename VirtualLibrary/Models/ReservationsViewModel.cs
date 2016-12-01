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

        public int Id { get; set; }
        public string Book { get; set; }
        public string Username { get; set; }
        [Required]
        [Display(Name = "Reservation Date")]
        public string reserved_date { get; set; }
        [Required]
        [Display(Name = "Reservation Date Expiration")]
        public string return_date { get; set; }

        [Required]
        [Display(Name = "Library Building")]
        public string Library { get; set; }

    }
  
    public class LibrarianCreateReservationViewModel
    {
        [Required]
        [Display(Name = "ISBN")]
        public string Isbn { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Reservation Date")]
        public string reserved_date { get; set; }
        [Required]
        [Display(Name = "Reservation Date Expiration")]
        public string return_date { get; set; }

        [Required]
        [Display(Name = "Library Building")]
        public string Library { get; set; }
    }
}
