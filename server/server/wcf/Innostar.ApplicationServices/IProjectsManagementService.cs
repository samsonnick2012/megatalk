using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public interface IProjectsManagementService : IApplicationService
	{
        IPageableList<Project> GetProjects(string text, PageInfo pageInfo);

		Project GetProject(int id);

        Project SaveProject(Project project);

        void DeleteProject(Project project);

        IList<MapObject> GetSuggestions(string query);

        IList<OrganizationProjectRelation> GetOrganizationsRelations(int parentId);

        OrganizationProjectRelation GetOrganizationRelation(int id);

        IList<ProjectContactRelation> GetPersonsRelations(int parentId);

        ProjectContactRelation GetPersonRelation(int id);
	}
}
