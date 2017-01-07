using System.Web.Mvc;
using Application.Core.UI.Controllers;

namespace XChat.Areas.Administration.Controllers
{
    [Authorize]
    public class HomeController : BasicController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
