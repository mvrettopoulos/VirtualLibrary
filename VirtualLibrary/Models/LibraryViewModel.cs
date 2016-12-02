using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualLibrary.Models
{
    public class LibraryViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Text)]
        [Display(Name = "University Name")]
        public string University_Name { get; set; }
        [Required]
        public string Building { get; set; }
        [Required]
        public string Location { get; set; }
    }
}
