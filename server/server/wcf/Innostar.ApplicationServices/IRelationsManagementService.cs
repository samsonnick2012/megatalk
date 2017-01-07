using Application.Core.ApplicationServices;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public interface IRelationsManagementService : IApplicationService
	{
		void SaveOrganizationsRelation(OrganizationsRelation organizationsRelation);

		void SaveOrganizationProjectRelation(OrganizationProjectRelation organizationProjectRelation);

		void SaveOrganizationPersonRelation(OrganizationContactRelation organizationPersonRelation);

		OrganizationProjectRelation GetOrganizationProjectRelation(int id);

		OrganizationContactRelation GetOrganizationPersonRelation(int id);

		OrganizationsRelation GetOrganizationsRelation(int id);

		ProjectContactRelation GetProjectPersonRelation(int id);

		void SaveProjectPersonRelation(ProjectContactRelation domainModel);

		void DeleteOrganizationProjectRelation(int id);

		void DeletePersonProjectRelation(int id);

		void DeleteOrganizationPersonRelation(int id);

		void DeleteOrganizationsRelation(int id);
	}
}