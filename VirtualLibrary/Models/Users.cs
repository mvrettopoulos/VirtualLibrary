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
    
    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            this.Librarians = new HashSet<Librarians>();
            this.Books_Ratings = new HashSet<Books_Ratings>();
            this.Reservations = new HashSet<Reservations>();
        }
    
        public int id { get; set; }
        public string username { get; set; }
        public string date_of_birth { get; set; }
        public string date_of_registration { get; set; }
        public Nullable<long> membership_id { get; set; }
        public byte[] image { get; set; }
        public string aspnet_user_id { get; set; }
        public bool active { get; set; }
        public Nullable<bool> bad_user { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Librarians> Librarians { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Books_Ratings> Books_Ratings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reservations> Reservations { get; set; }
    }
}
