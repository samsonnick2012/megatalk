using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XChat.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public const string ROLE_ADMIN = "Administrator";
        public const string ROLE_USER = "User";

        private bool _isAuthorized;

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            _isAuthorized = base.AuthorizeCore(httpContext);
            return _isAuthorized;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!_isAuthorized)
            {
                if (!filterContext.Controller.TempData.ContainsKey("RedirectReason"))
                {
                    filterContext.Controller.TempData.Add("RedirectReason", "Unauthorized");
                }
            }
        }
    }
}