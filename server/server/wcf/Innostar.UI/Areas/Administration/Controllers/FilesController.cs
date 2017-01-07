using System.Web.Mvc;
using Application.Core.UI.Controllers;

namespace Innostar.UI.Areas.Administration.Controllers
{
    public class FilesController : BasicController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
