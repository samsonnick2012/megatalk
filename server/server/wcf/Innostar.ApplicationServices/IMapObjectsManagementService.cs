using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public interface IMapObjectsManagementService : IApplicationService
	{
        IList<MapObject> GetRelatedMapObjects(int id, MapObjectType parentMapObjectType, MapObjectType filterType);

		ICollection<string> GetAllTags();

		Organization GetOrganization(int id);

		Person GetPerson(int id);

		Project GetProject(int id);

		Resource GetResource(int id);

        IList<MapObject> GetAllMapObjects(MapObjectType mapObjectType);

		byte[] GetMapObjectImage(MapObjectType type, int id);
	}
}