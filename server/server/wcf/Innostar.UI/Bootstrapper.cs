using Application.Infrastructure.AccountManagement;
using Application.Infrastructure.UserManagement;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Innostar.UI
{
	public static class Bootstrapper
	{
		public static void Initialize()
		{
			var locator = new UnityServiceLocator(ConfigureUnityContainer());

			ServiceLocator.SetLocatorProvider(() => locator);
		}

		public static IUnityContainer RegisterTypes(IUnityContainer container)
		{
			container.RegisterType<IAccountManagementService, AccountManagementService>();
			container.RegisterType<IUsersManagementService, UsersManagementService>();
            //container.RegisterType<IMapObjectsManagementService, MapObjectsManagementService>();
            //container.RegisterType<ISearchService, SearchService>();
            //container.RegisterType<IOrganizationsManagementService, OrganizationsManagementService>();
            //container.RegisterType<ICatalogsManagementService, CatalogsManagementService>();
            //container.RegisterType<IGeoCodingService, GeoCodingService>();
            //container.RegisterType<IPersonsManagementService, PersonsManagementService>();
            //container.RegisterType<IProjectsManagementService, ProjectsManagementService>();
            //container.RegisterType<IResourcesManagementService, ResourcesManagementService>();
            //container.RegisterType<IRelationsManagementService, RelationsManagementService>();

			return container;
		}

		private static IUnityContainer ConfigureUnityContainer()
		{
			var container = new UnityContainer();

			RegisterTypes(container);

			return container;
		}
	}
}