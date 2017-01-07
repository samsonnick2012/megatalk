using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public interface IPersonsManagementService : IApplicationService
	{
		IPageableList<Person> GetPersons(string text, IPageInfo pageInfo);

		Person GetPerson(int id);

		Person SavePerson(Person person);

        void DeletePerson(Person person);

        IList<MapObject> GetSuggestions(string query);

        IList<OrganizationContactRelation> GetOrganizationsRelations(int parentId);

        OrganizationContactRelation GetOrganizationRelation(int id);

        IList<ProjectContactRelation> GetProjectsRelations(int parentId);

        ProjectContactRelation GetProjectRelation(int id);

		void SaveOrganizationRelation(OrganizationContactRelation domainModel);

		void SaveProjectRelation(ProjectContactRelation domainModel);
	}
}
