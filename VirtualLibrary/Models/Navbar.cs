using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualLibrary.Models
{
    public class Navbar
    {
        public int Id { get; set; }
        public string NameOption { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string ImageClass { get; set; }
        public string Activeli { get; set; }
        public bool Status { get; set; }
        public int ParentId { get; set; }
        public bool IsParent { get; set; }
    }
}