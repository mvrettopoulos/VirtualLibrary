using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualLibrary.Models
{
    public class FilterViewModel
    {
        public string Search { get; set; }
        public string Category { get; set; }
        public string Filter { get; set; }
        public string Author { get; set; }
        public string Words { get; set; }
    }
}
