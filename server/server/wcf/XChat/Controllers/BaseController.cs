using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XChat.Filters;
using XChat.Helpers;

namespace XChat.Controllers
{
    [InitializeSimpleMembership]
    public class BaseController : Controller
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (Session != null && Session["RoleId"] != null)
            {
                int roleId;
                Int32.TryParse(Session["RoleId"].ToString(), out roleId);
                ViewData["Menu"] = MenuHelper.GetMemuItembyRole(roleId);
            }
            else
            {
                ViewData["Menu"] = MenuHelper.GetMemuItembyRole(1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public string GetMenu()
        {
            return "menu";
        }

       
    }
}