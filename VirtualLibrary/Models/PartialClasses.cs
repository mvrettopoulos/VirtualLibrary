using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VirtualLibrary.Models
{
    [MetadataType(typeof(MetaData.LibrariesMetaData))]
    public partial class Libraries
    {
    }

    [MetadataType(typeof(MetaData.BooksMetaData))]
    public partial class Books
    {
    }


    [MetadataType(typeof(MetaData.Books_AvailabilityMetaData))]
    public partial class Books_Availability
    {
    }


}