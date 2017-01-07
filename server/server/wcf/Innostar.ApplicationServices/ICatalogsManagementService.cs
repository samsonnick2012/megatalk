using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public interface ICatalogsManagementService : IApplicationService
	{
		IList<CatalogBase> GetAllCatalogs();

		IEnumerable<TCatalog> GetCatalogValues<TCatalog>() where TCatalog : CatalogBase, new();

		TCatalog GetCatalogValue<TCatalog>(int catalogValueId) where TCatalog : CatalogBase, new();

		TCatalog SaveCatalogValue<TCatalog>(TCatalog catalog) where TCatalog : CatalogBase, new();

		bool TryReorderCatalogValues<TCatalog>(int catalogValueId, int orderDirection) where TCatalog : CatalogBase, new();

		void DeleteCatalog<TCatalog>(int catalogValueId) where TCatalog : CatalogBase, new();
	}
}