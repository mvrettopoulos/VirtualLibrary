//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VirtualLibrary.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Librarians
    {
        public int id { get; set; }
        public Nullable<int> username_id { get; set; }
        public Nullable<int> library_id { get; set; }
    
        public virtual Libraries Libraries { get; set; }
        public virtual Users Users { get; set; }
    }
}
