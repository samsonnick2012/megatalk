using System.Collections.Generic;
using Application.Core.ApplicationServices;

namespace Application.Infrastructure.AccountManagement
{
	public interface IAccountManagementService : IApplicationService
	{
		bool Login(string login, string password, bool remembeMe);

		void Logout();

		string CreateAccount(string login, string password, IList<string> roles, string email, object properties = null, bool requireConfirmationToken = false);

		bool ChangePassword(string userName, string oldPassword, string newPassword);
	}
}