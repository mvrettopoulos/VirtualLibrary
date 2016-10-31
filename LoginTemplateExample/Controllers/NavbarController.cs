using System.Web.Mvc;

namespace LoginTemplateExample.Controllers
{
    public class NavbarController : Controller
    {
        // GET: Navbar
        public ActionResult Index()
        {
            return PartialView("_Navbar");
        }
    }
}