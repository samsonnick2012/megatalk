using System.Web.Mvc;
using System.Web.Routing;

namespace Innostar.UI.App_Start
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("mapobjects", "mapobjectsimages/{type}", new { controller = "Home", action = "GetMapObjectImagesInfo", type = UrlParameter.Optional }, new[] { "Innostar.UI.Controllers" });

			routes.MapRoute("searchtag", "mapobjects/", new { area = string.Empty, controller = "Home", action = "Find", tag = UrlParameter.Optional }, new[] { "Innostar.UI.Controllers" });

			routes.MapRoute("search", "mapobjects/", new { controller = "Home", action = "Find", text = UrlParameter.Optional }, new[] { "Innostar.UI.Controllers" });

			routes.MapRoute("mapoject", "mapobject/{mapObjectType}/{id}", new { area = string.Empty, controller = "Home", action = "GetDetailedMapObjectCard", id = UrlParameter.Optional, objectType = UrlParameter.Optional }, new[] { "Innostar.UI.Controllers" });

			routes.MapRoute("Default", "{controller}/{action}", new { controller = "Home", action = "Index" }, new[] { "Innostar.UI.Controllers" });

			routes.LowercaseUrls = true;
		}
	}
}