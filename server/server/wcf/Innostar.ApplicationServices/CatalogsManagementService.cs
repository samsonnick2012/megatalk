using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public class CatalogsManagementService : ApplicationService, ICatalogsManagementService
	{
		public IList<CatalogBase> GetAllCatalogs()
		{
			return new List<CatalogBase>
				{
					new ActivityDirection
					{
						Title = "Направления деятельности"
					},
					new OrganizationActivityType
					{
						Title = "Тип деятельности"
					},
					new OrganizationContactRelationType
					{
						Title = "Тип связи организация - персона"
					},
                    new OrganizationForm
					{
						Title = "Организационно - правовая форма организации"
					},
                    new OrganizationProjectRelationType
					{
						Title = "Тип связи организация - проект"
					},
                    new OrganizationsRelationType
					{
						Title = "Тип связи организация - организация"
					},
                    new PersonStatus
					{
						Title = "Статус персоны"
					},
                    new ProjectContactRelationType
					{
						Title = "Тип связи проект - персона"
					},
                    new ProjectState
					{
						Title = "Статус проекта"
					},
                    new ResourceState
                    {
                        Title = "Статус ресурса"
                    },
                    new ResourceType
                    {
                        Title = "Тип ресурса"
                    }
				};
		}

		public IEnumerable<TCatalog> GetCatalogValues<TCatalog>() where TCatalog : CatalogBase, new()
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.CatalogsRepository<TCatalog>().GetAll();
			}
		}

		public TCatalog GetCatalogValue<TCatalog>(int catalogValueId) where TCatalog : CatalogBase, new()
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				return repositoriesContainer.CatalogsRepository<TCatalog>().GetBy(new Query<TCatalog>(e => e.Id == catalogValueId));
			}
		}

		public TCatalog SaveCatalogValue<TCatalog>(TCatalog catalog) where TCatalog : CatalogBase, new()
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				repositoriesContainer.CatalogsRepository<TCatalog>().Save(catalog);
				repositoriesContainer.ApplyChanges();
			}

			return catalog;
		}

		public bool TryReorderCatalogValues<TCatalog>(int catalogValueId, int orderDirection) where TCatalog : CatalogBase, new()
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				var catalog = repositoriesContainer.CatalogsRepository<TCatalog>().GetCatalogValue(catalogValueId);
				var catalogValues = repositoriesContainer.CatalogsRepository<TCatalog>().GetCatalogValues();

				OrderableModel neighbor;
				var isSuccessFully = false;

				if (catalog.TryReOrder(catalogValues, out neighbor, orderDirection))
				{
					repositoriesContainer.CatalogsRepository<TCatalog>().Save((TCatalog)neighbor);
					repositoriesContainer.CatalogsRepository<TCatalog>().Save((TCatalog)catalog);
					repositoriesContainer.ApplyChanges();

					isSuccessFully = true;
				}

				return isSuccessFully;
			}
		}

		public void DeleteCatalog<TCatalog>(int catalogValueId) where TCatalog : CatalogBase, new()
		{
			ProcessMethod(() =>
			{
				using (var repositoriesContainer = new InnostarRepositoriesContainer())
				{
					repositoriesContainer.CatalogsRepository<TCatalog>().Delete(new TCatalog
					{
						Id = catalogValueId
					});

					repositoriesContainer.ApplyChanges();
				}
			});
		}
	}
}
