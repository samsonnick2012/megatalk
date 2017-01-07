using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.UI.Areas.Administration.ViewModels;

namespace Innostar.UI.Areas.Administration.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new ChatUserRepository(context);
                int countAllUsers = repository._All.Count();
                int countSafeUsers = repository._All.Count(user => user.SafeModeActivated);

                return View(new StatisticsViewModel
                {
                    ActiveUserNumber = 0,
                    SafeUserNumber = countSafeUsers,
                    UserNumber = countAllUsers,
                    XmppAdminLogin =
                        ConfigurationManager.AppSettings["XmppAdminLogin"] + "@" +
                        ConfigurationManager.AppSettings["XmppServerName"],
                    XmppAdminPassword = ConfigurationManager.AppSettings["XmppAdminPassword"],
                    HttpBindAddress = ConfigurationManager.AppSettings["httpBindAdress"]
                });
            }
        }
    }
}
