﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VirtualLibraryEntities : DbContext
    {
        public VirtualLibraryEntities()
            : base("name=VirtualLibraryEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<C__MigrationHistory1> C__MigrationHistory1 { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Libraries> Libraries { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Librarians> Librarians { get; set; }
        public virtual DbSet<Books> Books { get; set; }
        public virtual DbSet<Books_Availability> Books_Availability { get; set; }
        public virtual DbSet<Books_Ratings> Books_Ratings { get; set; }
        public virtual DbSet<Reservations> Reservations { get; set; }

        public System.Data.Entity.DbSet<VirtualLibrary.Models.ExtendLoanView> ExtendLoanViews { get; set; }
    }
}
