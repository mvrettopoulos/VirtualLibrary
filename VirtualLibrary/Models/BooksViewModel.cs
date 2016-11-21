using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VirtualLibrary.Models
{
    public class BooksViewModel
    {
        public int id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Text)]
        [Display(Name = "Book Title")]
        public string title { get; set; }

        [Required]
        [StringLength(int.MaxValue, ErrorMessage = "Please enter a descrption of the book.", MinimumLength = 50)]
        [DataType(DataType.Text)]
        [Display(Name = "Description")]

        public string description { get; set; }
        [Display(Name = "Image")]
        public HttpPostedFileBase image { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "The ISBN should be at least 5 characters", MinimumLength = 5)]
        [Display(Name = "ISBN")]
        public string isbn { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Publisher")]
        public string publisher { get; set; }

        public int views { get; set; }

        public IEnumerable<SelectListItem> AllAuthors { get; set; }

        public int[] ThisAuthor { get; set; }
        public Books book { get; set; }

        public IEnumerable<SelectListItem> AllCategories { get; set; }
        public int[] ThisCategory { get; set; }
       


        public BooksViewModel()
        {
            AllAuthors = new List<SelectListItem>();
            AllCategories = new List<SelectListItem>();
        }

    }
}