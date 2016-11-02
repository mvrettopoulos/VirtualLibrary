using System.Web.Mvc;

namespace VirtualLibrary.Controllers
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