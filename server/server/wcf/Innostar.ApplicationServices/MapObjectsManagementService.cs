using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public class MapObjectsManagementService : ApplicationService, IMapObjectsManagementService
	{
		public IList<MapObject> GetAllMapObjects(MapObjectType mapObjectType)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapObjectsRepository.GetAllMapObjects(mapObjectType);
			}
		}

		public byte[] GetMapObjectImage(MapObjectType mapObjectType, int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapObjectsRepository.GetMapObjectImage(mapObjectType, id);
			}
		}

        public IList<MapObject> GetRelatedMapObjects(int id, MapObjectType parentMapObjectType, MapObjectType filterType)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				var parentMapObject = new MapObject
				{
					Id = id,
					MapObjectType = parentMapObjectType
				};

				return repositoriesContainer.MapObjectsRepository.GetRelatedMapObjects(parentMapObject, filterType);
			}
		}

		public ICollection<string> GetAllTags()
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapObjectsRepository.GetAllTags();
			}
		}

		public Organization GetOrganization(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapObjectsRepository.GetOrganization(id);
			}
		}

		public Person GetPerson(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapObjectsRepository.GetPerson(id);
			}
		}

		public Project GetProject(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapObjectsRepository.GetProject(id);
			}
		}

		public Resource GetResource(int id)
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.MapObjectsRepository.GetResource(id);
			}
		}
	}
}
