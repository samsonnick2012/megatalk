using System.Web.Mvc;
using System.Web.Security;
using Application.Core;
using Application.Core.UI.Controllers;
using Application.Infrastructure.AccountManagement;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using XChat.Areas.Administration.ViewModels.AccountViewModels;

namespace XChat.Areas.Administration.Controllers
{
    [Authorize]
    public class AccountController : BasicController
    {
        private readonly LazyDependency<IAccountManagementService> _accountManagmentService = new LazyDependency<IAccountManagementService>();

        public IAccountManagementService AccountManagmentService
        {
            get
            {
                return _accountManagmentService.Value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid && model.Login())
            {
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Имя пользователя или пароль не является корректными");

            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AccountManagmentService.Logout();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult LoginError()
        {
            AccountManagmentService.Logout();

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.RegistrationUser(new[] { "user" });

                    return RedirectToAction("Login", "Account");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError(string.Empty, ErrorCodeToString(e.StatusCode));
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult PasswordRecovery(LoginViewModel model)
        {
            if (model.PasswordReset())
            {
                return View("PasswordRecoverySendMailSuccessfully");
            }

            return View("PasswordRecoveryError", model);
        }

        [AllowAnonymous]
        public ActionResult PasswordRecoveryChange(string id)
        {
            var model = new PasswordRecoveryViewModel
            {
                Token = id
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult PasswordRecoveryChange(PasswordRecoveryViewModel model)
        {
            model.ResetPassword();
            return View("PasswordRecoveryChangeSucc");
        }

        [AllowAnonymous]
        public ActionResult RegisterStepTwo()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterConfirmation(string id)
        {
            if (WebSecurity.ConfirmAccount(id))
            {
                return RedirectToAction("ConfirmationSuccess");
            }

            return RedirectToAction("ConfirmationFailure");
        }

        [AllowAnonymous]
        public ActionResult ConfirmationSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmationFailure()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(AccountManagmentViewModel model)
        {
            model.UpdateProfile(User.Identity.Name);

            return RedirectToAction("Management");
        }

        [HttpGet]
        public ActionResult Management()
        {
            ViewBag.ReturnUrl = Url.Action("Management");

            var userViewModel = new AccountManagmentViewModel(User.Identity.Name);

            return View(userViewModel);
        }

        [HttpGet]
        public ActionResult ChangePassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess
                    ? "Ваш пароль изменен"
                    : message == ManageMessageId.SetPasswordSuccess
                        ? "Пароль установлен"
                        : message == ManageMessageId.ChangeUserData
                            ? "Данные сохранены"
                            : string.Empty;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(PasswordManagmentViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Management");

            if (ModelState.IsValid)
            {
                if (model.ChangePassword(User.Identity.Name))
                {
                    return RedirectToAction("ChangePassword", new
                    {
                        Message = ManageMessageId.ChangePasswordSuccess
                    });
                }

                ModelState.AddModelError(string.Empty, "Неверный старый или новый пароль");
            }
            else
            {
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUserData(UserManagmentViewModel model)
        {
            model.SaveUserData(User.Identity.Name);

            return RedirectToAction("Management", new
            {
                Message = ManageMessageId.ChangeUserData
            });
        }

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            ChangeUserData,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }

            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion

        public ActionResult GetUserName()
        {
            return PartialView("Account/_UserNameView", new AccountManagmentViewModel(User.Identity.Name));
        }
    }
}
