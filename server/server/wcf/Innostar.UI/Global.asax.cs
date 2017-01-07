using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Application.Infrastructure.AccountManagement;
using Application.Infrastructure.Logging;
using Innostar.UI.App_Start;
using Innostar.UI.Controllers;
using log4net;

namespace Innostar.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
		private readonly ILog _logger;

		public MvcApplication()
		{
			_logger = LoggerWrapper.Create(typeof(MvcApplication));
		}

		protected void Application_Start()
		{
			SimpleMembershipInitializer.Initialize();
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			AuthConfig.RegisterAuth();
			Bootstrapper.Initialize();
		}

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;
            var currentController = " ";
            var currentAction = " ";
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !string.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !string.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var exception = Server.GetLastError();

            var errorGUID = Guid.NewGuid().ToString().ToUpper();

            _logger.Error(errorGUID, exception);

            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (exception is HttpException)
            {
                var httpEx = exception as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;
                    case 401:
                        action = "AccessDenied";
                        break;
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = exception is HttpException
               ? ((HttpException)exception).GetHttpCode()
               : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            controller.ViewData["GUIDError"] = errorGUID;

            controller.ViewData.Model = new HandleErrorInfo(exception, currentController, currentAction);

            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }

		protected void Application_AcquireRequestState(
			object sender,
			EventArgs e)
		{
			var ci = new CultureInfo("ru-RU");
			Thread.CurrentThread.CurrentUICulture = ci;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
			Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ",";
		}

		private static string GenerateErrorMessage(string errorIdentificator, Exception exception)
		{
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format(" ID ошибки : {0}", errorIdentificator));
            stringBuilder.AppendLine(exception.ToString());
            stringBuilder.AppendLine(string.Empty);

            return stringBuilder.ToString();
		}
    }
}