using System.ComponentModel.DataAnnotations;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.AccountManagement;
using Application.Infrastructure.UserManagement;
using XChat.ADM.Models;

namespace XChat.Areas.Administration.ViewModels.AccountViewModels
{
    public class AccountManagmentViewModel
    {
        private readonly LazyDependency<IAccountManagementService> _accountManagmentService = new LazyDependency<IAccountManagementService>();
        private readonly User _currentUser;
        private readonly LazyDependency<IUsersManagementService> _userManagmentService = new LazyDependency<IUsersManagementService>();

        public AccountManagmentViewModel()
        {
        }

        public AccountManagmentViewModel(string username)
        {
            _currentUser = UserManagmentService.GetUser(new Query<User>(e => e.UserName == username));

            Email = CurrentUser.Email;
            UnreadNotificationsCount = 1;
        }

        public IAccountManagementService AccountManagmentService
        {
            get
            {
                return _accountManagmentService.Value;
            }
        }

        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
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
        public string FirstName
        {
            get;
            set;
        }

        public string FullName
        {
            get
            {
                return string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)
                    ? CurrentUser.UserName
                    : string.Format("{0} {1}", FirstName, LastName);
            }
        }

        [StringLength(50, ErrorMessage = "Фамилия не может иметь размер больше 50 символов")]
        [DataType(DataType.Text)]
        [Display(Name = "Фамилия")]
        public string LastName
        {
            get;
            set;
        }

        [StringLength(50, ErrorMessage = "Отчество не может иметь размер больше 50 символов")]
        [DataType(DataType.Text)]
        [Display(Name = "Отчество")]
        public string MiddleName
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

        public int UnreadNotificationsCount
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

        public void UpdateProfile(string userName)
        {
            var user = UserManagmentService.GetUser(new Query<User>(e => e.UserName == userName));

            user.Email = Email;

            UserManagmentService.UpdateUser(user);
        }
    }
}