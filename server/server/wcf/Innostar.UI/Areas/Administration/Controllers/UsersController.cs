using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Application.Core.Data;
using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.Models;
using Innostar.UI.Areas.Administration.ViewModels;
using Innostar.UI.Helpers;

namespace Innostar.UI.Areas.Administration.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        public ActionResult Index(int? pageNumber = 1, bool? safeMode = false)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                IPageableList<ChatUser> usersPageableList = new PageableList<ChatUser>();

                if (safeMode.Value)
                {
                    usersPageableList = repositoriesContainer.RepositoryFor<ChatUser>()
                        .GetPageableBy(new PageableQuery<ChatUser>(new PageInfo
                        {
                            PageNumber = pageNumber ?? 1,
                            PageSize = 10
                        }).AddFilterClause(e => e.SafeModeActivated));
                }
                else
                {
                    usersPageableList = repositoriesContainer.RepositoryFor<ChatUser>()
                        .GetPageableBy(new PageableQuery<ChatUser>(new PageInfo
                        {
                            PageNumber = pageNumber ?? 1,
                            PageSize = 10
                        }));
                }

                var chatUsersList = usersPageableList.Items.Select(e => new ChatUserViewModel(e, ConfigurationManager.AppSettings["XmppServerName"])).ToList();
                var chatUsersPageableList = new PageableList<ChatUserViewModel>
                {
                    HasNext = usersPageableList.HasNext,
                    HasPrevious = usersPageableList.HasPrevious,
                    Items = chatUsersList,
                    PageInfo = usersPageableList.PageInfo,
                    TotalCount = usersPageableList.TotalCount
                };

                var model = new UsersViewModel
                {
                    XmppAdminLogin = ConfigurationManager.AppSettings["XmppAdminLogin"] + "@" + ConfigurationManager.AppSettings["XmppServerName"],
                    XmppAdminPassword = ConfigurationManager.AppSettings["XmppAdminPassword"],
                    HttpBindAddress = ConfigurationManager.AppSettings["httpBindAdress"],
                    chatUserViewModels = chatUsersPageableList,
                    MessageTemplates = GetMessageTemplates()
                };

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult _UsersList(int? pageNumber = 1, bool? safeMode = false, string text = null)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                IPageableList<ChatUser> usersPageableList = new PageableList<ChatUser>();

                if (safeMode.HasValue && safeMode.Value)
                {
                    usersPageableList = repositoriesContainer.RepositoryFor<ChatUser>()
                        .GetPageableBy(new PageableQuery<ChatUser>(new PageInfo
                        {
                            PageNumber = pageNumber ?? 1,
                            PageSize = 10
                        })
                        .AddFilterClause(e => string.IsNullOrEmpty(text) || e.Login.Contains(text) || e.Name.Contains(text))
                        .AddFilterClause(e => e.SafeModeActivated));
                }
                else
                {
                    usersPageableList = repositoriesContainer.RepositoryFor<ChatUser>()
                        .GetPageableBy(new PageableQuery<ChatUser>(new PageInfo
                        {
                            PageNumber = pageNumber ?? 1,
                            PageSize = 10
                        })
                        .AddFilterClause(e => string.IsNullOrEmpty(text) || e.Login.Contains(text) || e.Name.Contains(text)));
                }

                var chatUsersList =
                    usersPageableList.Items.Select(
                        e => new ChatUserViewModel(e, ConfigurationManager.AppSettings["XmppServerName"])).ToList();
                var chatUsersPageableList = new PageableList<ChatUserViewModel>
                {
                    HasNext = usersPageableList.HasNext,
                    HasPrevious = usersPageableList.HasPrevious,
                    Items = chatUsersList,
                    PageInfo = usersPageableList.PageInfo,
                    TotalCount = usersPageableList.TotalCount
                };

                var model = new UsersViewModel
                {
                    XmppAdminLogin =
                        ConfigurationManager.AppSettings["XmppAdminLogin"] + "@" +
                        ConfigurationManager.AppSettings["XmppServerName"],
                    XmppAdminPassword = ConfigurationManager.AppSettings["XmppAdminPassword"],
                    HttpBindAddress = ConfigurationManager.AppSettings["httpBindAdress"],
                    chatUserViewModels = chatUsersPageableList,
                    MessageTemplates = GetMessageTemplates()
                };

                ViewData["text"] = text;

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult GetAllXmppLogins(bool? safeMode = false)
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new ChatUserRepository(context);
                var chatLogins = new string[2];
                if (safeMode.Value)
                {
                    chatLogins = repository._All.Where(e => e.SafeModeActivated && !e.Disabled).Select(e => e.XmppLogin).ToArray();
                }
                else
                {
                    chatLogins = repository._All.Where(e => !e.Disabled).Select(e => e.XmppLogin).ToArray();
                    for (int i = 0; i != chatLogins.Length; i++)
                    {
                        chatLogins[i] += "@" + ConfigurationManager.AppSettings["XmppServerName"];
                    }
                }

                return Json(chatLogins, JsonRequestBehavior.AllowGet);
            }
        }

        private IEnumerable<SelectListItem> GetMessageTemplates()
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new MessageTemplateRepository(context);
                var templates = repository._All.Where(e => e.SpecialTemplate == 0).ToList();
                var result = new List<SelectListItem>();
                foreach (var template in templates)
                {
                    result.Add(new SelectListItem
                    {
                        Text = template.Title,
                        Value = template.Id.ToString(CultureInfo.InvariantCulture)
                    });
                }

                return result;
            }
        }

        public string _TextTemplate(int id)
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new MessageTemplateRepository(context);
                var template = repository._Get(e => e.Id == id).ToList();

                if (template.Any())
                {
                    return template.FirstOrDefault().Message;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public JsonResult _MessageVariables(string login)
        {
            var receiverStrings = login.Split('@');
            var xmppLogin = receiverStrings[0];

            using (var context = new InnostarModelsContext())
            {
                var result = new Dictionary<string, string>();

                var repository = new ChatUserRepository(context);
                var users = repository._Get(e => e.XmppLogin == xmppLogin).ToList();

                if (users.Any())
                {
                    var user = users.FirstOrDefault();
                    var email = string.IsNullOrEmpty(user.Email) ? string.Empty : user.Email;
                    var name = string.IsNullOrEmpty(user.Name) ? user.Login : user.Name;

                    result.Add("userEmail", email);
                    result.Add("userDisplayName", name);


                    var requestRepository = new RecoverPasswordRequestRepository(context);
                    var requests =
                        requestRepository._Get(e => e.UserId == user.Id)
                            .OrderByDescending(e => e.RequestAcceptedTime)
                            .ToList();

                    var request = requests.Any() ? GetRecoverLink(requests.FirstOrDefault().RequestKey) : string.Empty;
                    result.Add("userPasswordRequest", request);

                    var confirmEmailRepository = new ConfirmEmailRequestRepository(context);
                    var confirmEmailrequests =
                        confirmEmailRepository._Get(e => e.UserId == user.Id)
                            .OrderByDescending(e => e.RequestAcceptedTime)
                            .ToList();

                    var confirmEmailrequest = confirmEmailrequests.Any() ? GetConfirmLink(confirmEmailrequests.FirstOrDefault().RequestKey) : string.Empty;
                    result.Add("userConfirmRequest", confirmEmailrequest);
                }
                else
                {
                    result.Add("userEmail", string.Empty);
                    result.Add("userDisplayName", string.Empty);
                    result.Add("userRequest", string.Empty);
                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public string _GenerateRecoverLink(string login)
        {
            var receiverStrings = login.Split('@');
            var xmppLogin = receiverStrings[0];

            using (var context = new InnostarModelsContext())
            {
                var repository = new ChatUserRepository(context);
                var users = repository._Get(e => e.XmppLogin == xmppLogin).ToList();

                if (users.Any())
                {
                    var user = users.FirstOrDefault();

                    var rnd = new Random();
                    var requestKey = rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                     + Guid.NewGuid().ToString().Replace("-", string.Empty)
                                     + rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                     + Guid.NewGuid().ToString().Replace("-", string.Empty);

                    var newPassword = rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                     + Guid.NewGuid().ToString().Replace("-", string.Empty)
                                     + rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                     + Guid.NewGuid().ToString().Replace("-", string.Empty);

                    var pu = new PasswordUtility(newPassword.ToCharArray());
                    user.PasswordHash = pu.Hash;
                    user.PasswordSalt = pu.Salt;

                    //repository._Update(user);
                    //repository._Save();

                    var recoverRepository = new RecoverPasswordRequestRepository(context);

                    var request = new RecoverPasswordRequest
                    {
                        IsActive = true,
                        RequestAcceptedTime = DateTime.Now,
                        RequestKey = requestKey,
                        User = user
                    };

                    recoverRepository._Insert(request);
                    recoverRepository._Save();
                }
            }

            return string.Empty;
        }

        public string _GenerateConfirmLink(string login)
        {
            var receiverStrings = login.Split('@');
            var xmppLogin = receiverStrings[0];

            using (var context = new InnostarModelsContext())
            {
                var repository = new ChatUserRepository(context);
                var users = repository._Get(e => e.XmppLogin == xmppLogin).ToList();

                if (users.Any())
                {
                    var user = users.FirstOrDefault();

                    var confirmRepository = new ConfirmEmailRequestRepository(context);
                    var rnd = new Random();
                    var requestKey = rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                     + Guid.NewGuid().ToString().Replace("-", string.Empty)
                                     + rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                     + Guid.NewGuid().ToString().Replace("-", string.Empty);

                    //user.Disabled = true;
                    //repository._Update(user);
                    //repository._Save();

                    var request = new ConfirmEmailRequest
                    {
                        IsActive = true,
                        RequestAcceptedTime = DateTime.Now,
                        RequestKey = requestKey,
                        User = user
                    };

                    confirmRepository._Insert(request);
                    confirmRepository._Save();
                }
            }

            return string.Empty;
        }

        private string GetRecoverLink(string requestKey)
        {
            return string.Format("{0}/Recover/Password?key={1}",
                ConfigurationManager.AppSettings["MailSenderHost"],
                requestKey);
        }

        private string GetConfirmLink(string requestKey)
        {
            return string.Format("{0}/Home/ConfirmEmail?key={1}",
                ConfigurationManager.AppSettings["MailSenderHost"],
                requestKey);
        }

        [HttpPost]
        public string EnableSafeMode(int id, DateTime endTime)
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new ChatUserRepository(context);
                var users = repository._Get(e => e.Id == id).ToList();

                if (users.Any())
                {
                    var user = users.FirstOrDefault();
                    user.SafeModeActivated = true;
                    user.SafeModeStartDate = DateTime.Now;
                    user.SafeModeEndDate = endTime;
                    if (user.SafeModeStartDate < user.SafeModeEndDate)
                    {
                        repository._Update(user);
                        repository._Save();
                    }
                }
            }
            return string.Empty;
        }

        [HttpPost]
        public string DisableSafeMode(int id)
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new ChatUserRepository(context);
                var users = repository._Get(e => e.Id == id).ToList();

                if (users.Any())
                {
                    var user = users.FirstOrDefault();
                    user.SafeModeActivated = false;
                    user.SafeModeEndDate = DateTime.Now;

                    repository._Update(user);
                    repository._Save();
                }
            }

            return string.Empty;
        }
    }
}
