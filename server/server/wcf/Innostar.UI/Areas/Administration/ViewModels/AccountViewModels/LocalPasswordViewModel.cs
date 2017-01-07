using System.ComponentModel.DataAnnotations;

namespace Innostar.UI.Areas.Administration.ViewModels.AccountViewModels
{
	public class LocalPasswordViewModel
	{
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

		[DataType(DataType.EmailAddress)]
		[Display(Name = "Почтовый адрес")]
		[StringLength(50, ErrorMessage = "Email не может иметь размер больше 50 символов")]
		public string Email
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "Имя не может иметь размер больше 50 символов")]
		[DataType(DataType.Text)]
		[Display(Name = "Имя")]
		public string Name
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "Фамилия не может иметь размер больше 50 символов")]
		[DataType(DataType.Text)]
		[Display(Name = "Фамилия")]
		public string Surname
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "Отчество не может иметь размер больше 50 символов")]
		[DataType(DataType.Text)]
		[Display(Name = "Отчество")]
		public string Patronymic
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "Телефон не может иметь размер больше 50 символов")]
		[DataType(DataType.PhoneNumber)]
		[Display(Name = "Телефон")]
		public string PhoneNumber
		{
			get;
			set;
		}
	}
}