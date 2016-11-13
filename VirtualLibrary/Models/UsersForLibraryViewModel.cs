using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualLibrary.Models
{
    public class UsersForLibraryViewModel
    {
        public IEnumerable<SelectListItem> AllLibrarians { get; set; }
        public String ThisLibrarian { get; set; }
        public Libraries Library { get; set; }


        public UsersForLibraryViewModel()
        {
            AllLibrarians = new List<SelectListItem>();
        }
    }
}