using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualLibrary.Models
{
    public class SearchBooksFilter
    {
        public int itemNum { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public byte[] image { get; set; }
        public string authors { get; set; }
    }
}