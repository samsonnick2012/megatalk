using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.AccountManagement;
using Application.Infrastructure.NotificationsManagement;
using Application.Infrastructure.UserManagement;
using WebMatrix.WebData;
using XChat.ADM.Models;

namespace XChat.Areas.Administration.ViewModels.AccountViewModels
{
	public class LoginViewModel
	{
		private readonly LazyDependency<IAccountManagementService> _accountAuthenticationService = new LazyDependency<IAccountManagementService>();
		private readonly LazyDependency<IUsersManagementService> _usersManagementService = new LazyDependency<IUsersManagementService>();

		[Display(Name = "Логин")]
		public string UserName { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Display(Name = "Запомнить меня?")]
		public bool RememberMe { get; set; }

		public IAccountManagementService AccountAuthenticationService
		{
			get
			{
				return _accountAuthenticationService.Value;
			}
		}

		public IUsersManagementService UsersManagementService
		{
			get
			{
				return _usersManagementService.Value;
			}
		}

		public bool Login()
		{
			return AccountAuthenticationService.Login(UserName, Password, RememberMe);
		}

		public bool PasswordReset()
		{
			if (UsersManagementService.IsExistsUser(new Query<User>(e => e.UserName == UserName)))
			{
				var user = UsersManagementService.GetUser(new Query<User>(e => e.UserName == UserName));
				if (string.IsNullOrEmpty(user.Email))
				{
					return false;
				}

				var token = WebSecurity.GeneratePasswordResetToken(UserName);
				MailSender.SendMail(user.Email, "Восстановление пароля", "Для восстановления пароля на Российском общеобразовательном портале пройдите по ссылке " + "<a href='" + ConfigurationManager.AppSettings["ResetPasswordUrl"] + token + "'>Восстановление пароля</a>");

				return true;
			}

			return false;
		}
	}
}