using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public interface IResourcesManagementService : IApplicationService
	{
        IPageableList<Resource> GetResources(string text, IPageInfo pageInfo);

		Resource GetResource(int id);

        Resource SaveResource(Resource resource);

        void DeleteResource(Resource resource);

        IList<MapObject> GetSuggestions(string query);
	}
}
