using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public class PersonsManagementService : ApplicationService, IPersonsManagementService
	{
		public Person SavePerson(Person person)
		{
			return ProcessMethod(() =>
			{
				using (var repositoriesContainer = new InnostarRepositoriesContainer())
				{
					repositoriesContainer.MapPersonsRepository.Save(person);
					repositoriesContainer.ApplyChanges();

					return person;
				}
			});
		}

		public void DeletePerson(Person person)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.MapPersonsRepository.Delete(person);
				repositoriesContainer.ApplyChanges();
			}
		}

		public IPageableList<Person> GetPersons(string text, IPageInfo pageInfo)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<Person>().GetPageableBy(new PageableQuery<Person>(pageInfo).AddFilterClause(e => (e.LastName + " " + e.Name + " " + e.MiddleName).Contains(text) || string.IsNullOrEmpty(text)));
			}
		}

		public Person GetPerson(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.RepositoryFor<Person>().GetBy(new Query<Person>(e => e.Id == id)
					.Include(e => e.PersonStatuses));
			}
		}

		public IList<OrganizationContactRelation> GetOrganizationsRelations(int parentId)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapPersonsRepository.GetRelatedOrganizations(parentId);
			}
		}

		public OrganizationContactRelation GetOrganizationRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapPersonsRepository.GetOrganizationRelation(id);
			}
		}

		public IList<ProjectContactRelation> GetProjectsRelations(int parentId)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapPersonsRepository.GetProjectsRelations(parentId);
			}
		}

		public ProjectContactRelation GetProjectRelation(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapPersonsRepository.GetProjectRelation(id);
			}
		}

		public void SaveOrganizationRelation(OrganizationContactRelation domainModel)
		{
			throw new System.NotImplementedException();
		}

		public void SaveProjectRelation(ProjectContactRelation domainModel)
		{
			throw new System.NotImplementedException();
		}

		public IList<MapObject> GetSuggestions(string query)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapPersonsRepository.GetSuggestions(query);
			}
		}

		public void Save(Person person)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.RepositoryFor<Person>().Save(person);
				repositoriesContainer.ApplyChanges();
			}
		}
	}
}