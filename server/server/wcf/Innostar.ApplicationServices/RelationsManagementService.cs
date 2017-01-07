using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public class RelationsManagementService : ApplicationService, IRelationsManagementService
	{
		public void SaveOrganizationsRelation(OrganizationsRelation organizationsRelation)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.OrganizationsRelations.Save(organizationsRelation);
				repositoriesContainer.ApplyChanges();
			}
		}

		public void SaveOrganizationProjectRelation(OrganizationProjectRelation organizationProjectRelation)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.OrganizationProjectRelations.Save(organizationProjectRelation);
				repositoriesContainer.ApplyChanges();
			}
		}

		public void SaveOrganizationPersonRelation(OrganizationContactRelation organizationPersonRelation)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.OrganizationPersonRelations.Save(organizationPersonRelation);
				repositoriesContainer.ApplyChanges();
			}
		}

		public OrganizationProjectRelation GetOrganizationProjectRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<OrganizationProjectRelation>().GetBy(new Query<OrganizationProjectRelation>(e => e.Id == id)
					.Include(e => e.Organization)
					.Include(e => e.Project)
					.Include(e => e.ProjectOrganizationRelationType));
			}
		}

		public OrganizationContactRelation GetOrganizationPersonRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<OrganizationContactRelation>().GetBy(new Query<OrganizationContactRelation>(e => e.Id == id)
					.Include(e => e.Organization)
					.Include(e => e.Person)
					.Include(e => e.OrganizationContactRelationTypes));
			}
		}

		public OrganizationsRelation GetOrganizationsRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<OrganizationsRelation>().GetBy(new Query<OrganizationsRelation>(e => e.Id == id)
					.Include(e => e.PrimaryOrganization)
					.Include(e => e.DependentOrganization)
					.Include(e => e.OrganizationsRelationType));
			}
		}

		public ProjectContactRelation GetProjectPersonRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<ProjectContactRelation>().GetBy(new Query<ProjectContactRelation>(e => e.Id == id)
					.Include(e => e.Person)
					.Include(e => e.Project)
					.Include(e => e.ProjectContactTypes));
			}
		}

		public void SaveProjectPersonRelation(ProjectContactRelation domainModel)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.ProjectPersonRelationRepository.Save(domainModel);
				repositoriesContainer.ApplyChanges();
			}
		}

		public void DeleteOrganizationProjectRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.OrganizationProjectRelations.Delete(new OrganizationProjectRelation
				{
					Id = id
				});
				repositoriesContainer.ApplyChanges();
			}
		}

		public void DeletePersonProjectRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.ProjectPersonRelationRepository.Delete(new ProjectContactRelation
				{
					Id = id
				});
				repositoriesContainer.ApplyChanges();
			}
		}

		public void DeleteOrganizationPersonRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.OrganizationPersonRelations.Delete(new OrganizationContactRelation
				{
					Id = id
				});
				repositoriesContainer.ApplyChanges();
			}
		}

		public void DeleteOrganizationsRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.OrganizationsRelations.Delete(new OrganizationsRelation
				{
					Id = id
				});
				repositoriesContainer.ApplyChanges();
			}
		}
	}
}
