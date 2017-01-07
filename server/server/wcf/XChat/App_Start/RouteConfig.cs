using System.Web.Mvc;
using System.Web.Routing;

namespace XChat.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Images", "Images/{id}", new { controller = "Home", action = "GetImage" });
            routes.MapRoute("UnitData", "UnitData/{id}", new { controller = "Home", action = "GetUnitData" });
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Service/Service.svc/");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}