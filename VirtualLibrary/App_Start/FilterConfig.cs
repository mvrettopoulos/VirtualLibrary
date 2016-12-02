using System.Web;
using System.Web.Mvc;

namespace VirtualLibrary.App_Start
{
    public class FilterConfig
    {
        protected FilterConfig()
        {

        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
