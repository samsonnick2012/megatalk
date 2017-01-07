using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Application.Infrastructure.UserManagement
{
    public class UsersManagementService : ApplicationService, IUsersManagementService
    {
        public void DeleteUser(User user)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                repositoriesContainer.RepositoryFor<User>().Delete(user);
            }
        }

        public IEnumerable<Role> GetAllRoles()
        {
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.RepositoryFor<Role>().GetAll();
            }
        }

        public bool IsExistsUser(IQuery<User> query)
        {
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
				return repositoriesContainer.RepositoryFor<User>().GetBy(query) != null;
            }
        }

        public User GetUser(IQuery<User> query)
        {
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.RepositoryFor<User>().GetBy(query);
            }
        }

        public void UpdateUser(User user)
        {
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                repositoriesContainer.RepositoryFor<User>().Save(user);
            }
        }
    }
}
