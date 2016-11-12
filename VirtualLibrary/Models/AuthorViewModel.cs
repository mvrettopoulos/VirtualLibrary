using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualLibrary.Models
{
    public class AuthorViewModel
    {
        public string id { get; set; }
        [Required]
        [Display(Name = "Author Name")]
        public string authorName { get; set; }
    }
}
