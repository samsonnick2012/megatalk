using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
    public class ResourcesManagementService : ApplicationService, IResourcesManagementService
    {
        public Resource SaveResource(Resource resource)
        {
            return ProcessMethod(() =>
            {
                using (var repositoriesContainer = new InnostarRepositoriesContainer())
                {
                    repositoriesContainer.MapResourcesRepository.Save(resource);
                    repositoriesContainer.ApplyChanges();

	                return resource;
                }
            });
        }

        public void DeleteResource(Resource resource)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                repositoriesContainer.MapResourcesRepository.Delete(resource);
                repositoriesContainer.ApplyChanges();
            }
        }

        public IPageableList<Resource> GetResources(string text, IPageInfo pageInfo)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.RepositoryFor<Resource>().GetPageableBy(new PageableQuery<Resource>(pageInfo).AddFilterClause(e => e.Title.Contains(text) || string.IsNullOrEmpty(text)));
            }
        }

        public Resource GetResource(int id)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.RepositoryFor<Resource>().GetBy(new Query<Resource>(e => e.Id == id)
                    .Include(e => e.Organization)
                    .Include(e => e.Project)
                    .Include(e => e.ResourceState)
                    .Include(e => e.ResourceType));
            }
        }

        public IList<MapObject> GetSuggestions(string query)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                return repositoriesContainer.MapResourcesRepository.GetSuggestions(query);
            }
        }
    }
}