using System.Web.Mvc;

namespace XChat.Areas.Administration
{
    public class AdministrationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Administration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("Administration_default", "Administration/{controller}/{action}/{id}", new  {controller = "Account", action = "Login", id = UrlParameter.Optional }, new[] { "Innostar.UI.Areas.Administration.Controllers" });
        }
    }
}
