using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualLibrary.Models
{
    public class CategoryViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
