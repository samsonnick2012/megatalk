using System.ComponentModel.DataAnnotations;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.UserManagement;
using XChat.ADM.Models;

namespace XChat.Areas.Administration.ViewModels.AccountViewModels
{
	public class UserManagmentViewModel
	{
        private readonly LazyDependency<IUsersManagementService> _userManagmentService = new LazyDependency<IUsersManagementService>();

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

		public IUsersManagementService UserManagmentService
		{
			get
			{
				return _userManagmentService.Value;
			}
		}

		public UserManagmentViewModel()
		{
		}

		public UserManagmentViewModel(string userName)
		{
            var user = UserManagmentService.GetUser(new Query<User>(e => e.UserName == userName));

			Email = user.Email;
		}

		public void SaveUserData(string userName)
		{
            var user = UserManagmentService.GetUser(new Query<User>(e => e.UserName == userName));

			user.Email = Email;

			UserManagmentService.UpdateUser(user);
		}
	}
}