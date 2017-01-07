using System;
using System.Web.Mvc;
using Application.Core.UI.Controllers;
using Innostar.Models;

namespace Innostar.UI.Areas.Administration.Controllers
{
    [Authorize]
    public class HomeController : BasicController
    {
        public ActionResult Index()
        {
            return View();
        }

        public string Error()
        {
            throw new NullReferenceException();
            return string.Empty;
        }
    }
}
