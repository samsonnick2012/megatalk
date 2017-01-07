using System.ComponentModel.DataAnnotations;
using WebMatrix.WebData;

namespace XChat.Areas.Administration.ViewModels.AccountViewModels
{
	public class PasswordRecoveryViewModel
	{
		public string Token
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

		public void ResetPassword()
		{
			WebSecurity.ResetPassword(Token, NewPassword);
		}
	}
}