using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
    public class OrganizationsManagementService : ApplicationService, IOrganizationsManagementService
    {
		public void DeleteOrganization(Organization organization)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.MapOrganizationsRepository.Delete(organization);
				repositoriesContainer.ApplyChanges();
			}
		}

		public Organization GetOrganization(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<Organization>().GetBy(new Query<Organization>(e => e.Id == id).Include(e => e.ActivitiesDirections).Include(e => e.ActivitiesTypes).Include(e => e.OrganizationForm));
			}
		}

		public IPageableList<Organization> GetOrganizations(string text, IPageInfo pageInfo)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<Organization>().GetPageableBy(new PageableQuery<Organization>(pageInfo).AddFilterClause(e => e.Title.Contains(text) || string.IsNullOrEmpty(text)));
			}
		}

		public IList<OrganizationsRelation> GetOrganizationsRelations(int parentId)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapOrganizationsRepository.GetRelatedOrganizations(parentId);
			}
		}

		public IList<OrganizationContactRelation> GetPersonsRelations(int parentId)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapOrganizationsRepository.GetPersonsRelations(parentId);
			}
		}

		public IList<OrganizationProjectRelation> GetProjectsRelations(int parentId)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapOrganizationsRepository.GetProjectsRelations(parentId);
			}
		}

		public IEnumerable<Organization> GetTitles(string query)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapOrganizationsRepository.GetTitles(query, 5);
			}
		}

		public Organization SaveOrganization(Organization organization)
        {
	        return ProcessMethod(() =>
	        {
		        using (var repositoriesContainer = new InnostarRepositoriesContainer())
		        {
			        repositoriesContainer.MapOrganizationsRepository.Save(organization);
			        repositoriesContainer.ApplyChanges();

			        return organization;
		        }
	        });
        }
    }
}
