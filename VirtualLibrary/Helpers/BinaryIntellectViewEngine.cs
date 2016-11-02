using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VirtualLibrary.Helper
{
    public class BinaryIntellectViewEngine : RazorViewEngine
    {
        public BinaryIntellectViewEngine()
        {
            string[] locations = new string[] {
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{1}/Users/{0}.cshtml",
            "~/Views/{1}/Roles/{0}.cshtml",
            "~/Views/BuildingModels/{1}/{0}.cshtml",
            "~/Views/Shared/{0}.cshtml",
            "~/Views/Shared/PartialViews/{0}.cshtml",
            "~/Views/Shared/Layouts/{0}.cshtml"
        };

            this.ViewLocationFormats = locations;
            this.PartialViewLocationFormats = locations;
            this.MasterLocationFormats = locations;
        }
    }
}
