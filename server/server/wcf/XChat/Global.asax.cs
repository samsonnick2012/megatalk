using System;
using System.Data.Entity;
using System.Globalization;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Util;
using XChat.App_Start;
using XChat.Filters;
using XChat.Helpers;
using XChat.Models.DB;
using WebMatrix.WebData;

namespace XChat
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /*Database.SetInitializer<XChatContext>(new ProjectInitializer());
            using (XChatContext context = new XChatContext())
            {
                context.Database.Initialize(false);
            }
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "Id", "UserName", autoCreateTables: true);*/

            InitializeSimpleMembershipAttribute.EnsureInitialized();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");

        }

        protected virtual void Application_BeginRequest()
        {
            System.Web.HttpContext.Current.Items["_EntityContext"] = new XChatContext();
        }

        protected virtual void Application_EndRequest()
        {
            var entityContext = System.Web.HttpContext.Current.Items["_EntityContext"] as XChatContext;
            if (entityContext != null)
            {
                entityContext.Dispose();
            }
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            string errorUid = Guid.NewGuid().ToString().ToUpper();
            Exception ex = Server.GetLastError();
            string errorText =
                string.Format(
                    @"ErrorUid: {0}
                                             {1}
                                             ",
                    errorUid, StackTraceUtility.GetStackTraceAsPlainText(ex));
            Log.Error(errorText);
        }
    }
}