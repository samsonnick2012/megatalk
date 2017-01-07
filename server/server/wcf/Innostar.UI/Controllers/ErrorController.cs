using System.Web.Mvc;
using Application.Core.UI.Controllers;

namespace Innostar.UI.Controllers
{
	public class ErrorController : BasicController
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult NotFound()
		{
			return View();
		}
	}
}