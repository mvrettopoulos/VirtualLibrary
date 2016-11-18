using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VirtualLibrary.Models
{

    public class MetaData
    {
        public partial class LibrariesMetaData
        {
            public int id { get; set; }
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

        public partial class BooksMetaData
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
            public byte[] image { get; set; }

            [StringLength(60, ErrorMessage = "The ISBN should be at least 5 characters", MinimumLength = 5)]
            [Display(Name = "ISBN")]
            public string isbn { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
            [Display(Name = "Publisher")]
            public string publisher { get; set; }

            public int views { get; set; }

        }

        public partial class Books_AvailabilityMetaData
        {
            public int id { get; set; }

            public int book_id { get; set; }

            public int library_id { get; set; }

            [Required]
            [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
            public Nullable<int> quantity { get; set; }

            [Required]
            [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
            public Nullable<int> reserved { get; set; }

            [Required]
            [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
            public Nullable<int> available { get; set; }

        }


    }
}