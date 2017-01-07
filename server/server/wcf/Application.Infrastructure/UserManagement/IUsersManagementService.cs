using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Models;

namespace Application.Infrastructure.UserManagement
{
	public interface IUsersManagementService : IApplicationService
	{
        User GetUser(IQuery<User> query);

	    void UpdateUser(User user);

		void DeleteUser(User user);

		IEnumerable<Role> GetAllRoles();

        bool IsExistsUser(IQuery<User> query);
	}
}