using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public interface IOrganizationsManagementService : IApplicationService
	{
	    IPageableList<Organization> GetOrganizations(string text, IPageInfo pageInfo);

		Organization GetOrganization(int id);

        Organization SaveOrganization(Organization organization);

        void DeleteOrganization(Organization organization);

        IEnumerable<Organization> GetTitles(string query);

        IList<OrganizationsRelation> GetOrganizationsRelations(int parentId);

        IList<OrganizationProjectRelation> GetProjectsRelations(int parentId);

        IList<OrganizationContactRelation> GetPersonsRelations(int parentId);
	}
}