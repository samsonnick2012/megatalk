using System.ComponentModel.DataAnnotations;
using Application.Core;
using Application.Infrastructure.AccountManagement;

namespace Innostar.UI.Areas.Administration.ViewModels.AccountViewModels
{
	public class PasswordManagmentViewModel
	{
		private readonly LazyDependency<IAccountManagementService> _passworManagmentService = new LazyDependency<IAccountManagementService>();

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Текущий пароль", Description = "Значение текущего пароля")]
		public string OldPassword
		{
			get;
			set;
		}

		[Required]
		[StringLength(100, ErrorMessage = "{0} должен быть не менее {2} символов.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Новый пароль", Description = "Новый пароль от аккаунта")]
		public string NewPassword
		{
			get;
			set;
		}

		[DataType(DataType.Password)]
		[Display(Name = "Подтвердить новый пароль", Description = "Подтверждение нового пароля от аккаунта")]
		[Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
		public string ConfirmPassword
		{
			get;
			set;
		}

		public IAccountManagementService PasswordManagmentService
		{
			get
			{
				return _passworManagmentService.Value;
			}
		}

		public bool ChangePassword(string userName)
		{
			return PasswordManagmentService.ChangePassword(userName, OldPassword, NewPassword);
		}
	}
}