using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.AccountManagement;
using Application.Infrastructure.UserManagement;
using XChat.ADM.Models;

namespace XChat.Areas.Administration.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        private readonly LazyDependency<IAccountManagementService> _accountManagementService = new LazyDependency<IAccountManagementService>();
        private readonly LazyDependency<IUsersManagementService> _usersManagementService = new LazyDependency<IUsersManagementService>();

		[Required(ErrorMessage = "Поле Логин обязательно для заполнения")]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "T{0} должно быть не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и подтвержденный пароль не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Поле Почтовый ящик обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Почтовый ящик не соответствует приням стандартам")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Почтовый ящик")]
        public string Email
        {
            get;
            set;
        }

        public IAccountManagementService AccountManagementService
        {
            get
            {
                return _accountManagementService.Value;
            }
        }

        public IUsersManagementService UsersManagementService
        {
            get
            {
                return _usersManagementService.Value;
            }
        }

        public void RegistrationUser(IList<string> roles)
        {
			AccountManagementService.CreateAccount(UserName, Password, roles, Email, requireConfirmationToken: false);

            var user = UsersManagementService.GetUser(new Query<User>(e => e.UserName == UserName));

            user.Email = Email;

            UsersManagementService.UpdateUser(user);
        }
    }
}