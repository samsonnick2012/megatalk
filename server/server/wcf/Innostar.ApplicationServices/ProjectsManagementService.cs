using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public class ProjectsManagementService : ApplicationService, IProjectsManagementService
	{
	    public Project SaveProject(Project project)
	    {
	        return ProcessMethod(() =>
	        {
	            using (var repositoriesContainer = new InnostarRepositoriesContainer())
	            {
	                repositoriesContainer.MapProjectsRepository.Save(project);
	                repositoriesContainer.ApplyChanges();

		            return project;
	            }
	        });
	    }

        public void DeleteProject(Project project)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                repositoriesContainer.MapProjectsRepository.Delete(project);
                repositoriesContainer.ApplyChanges();
            }
        }

        public IPageableList<Project> GetProjects(string text, PageInfo pageInfo)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
                return repositoriesContainer.RepositoryFor<Project>().GetPageableBy(new PageableQuery<Project>(pageInfo).AddFilterClause(e => e.Title.Contains(text) || string.IsNullOrEmpty(text)));
			}
		}

		public Project GetProject(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
                return repositoriesContainer.RepositoryFor<Project>().GetBy(new Query<Project>(e => e.Id == id)
                    .Include(e => e.ActivitiesDirections)
                    .Include(e => e.ProjectState));
			}
		}

        public IList<OrganizationProjectRelation> GetOrganizationsRelations(int parentId)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.MapProjectsRepository.GetRelatedOrganizations(parentId);
            }
        }

        public OrganizationProjectRelation GetOrganizationRelation(int id)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.MapProjectsRepository.GetOrganizationRelation(id);
            }
        }

        public IList<ProjectContactRelation> GetPersonsRelations(int parentId)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.MapProjectsRepository.GetPersonsRelations(parentId);
            }
        }

        public ProjectContactRelation GetPersonRelation(int id)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.MapProjectsRepository.GetPersonRelation(id);
            }
        }

        public IList<MapObject> GetSuggestions(string query)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.MapProjectsRepository.GetSuggestions(query);
            }
        }

        public void Save(Project project)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                repositoriesContainer.RepositoryFor<Project>().Save(project);
                repositoriesContainer.ApplyChanges();
            }
        }
	}
}