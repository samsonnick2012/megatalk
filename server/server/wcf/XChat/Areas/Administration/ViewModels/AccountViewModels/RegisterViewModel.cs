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

		[Required(ErrorMessage = "���� ����� ����������� ��� ����������")]
        [Display(Name = "�����")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "���� ������ ����������� ��� ����������")]
        [StringLength(100, ErrorMessage = "T{0} ������ ���� �� ����� {2} ��������.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "������")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "������������� ������")]
        [Compare("Password", ErrorMessage = "������ � �������������� ������ �� ���������.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "���� �������� ���� ����������� ��� ����������")]
        [EmailAddress(ErrorMessage = "�������� ���� �� ������������� ������ ����������")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "�������� ����")]
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