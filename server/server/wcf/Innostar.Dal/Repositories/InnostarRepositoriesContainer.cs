using Application.Core.Data;
using Innostar.Dal.Infrastructure;

namespace Innostar.Dal.Repositories
{
	public class InnostarRepositoriesContainer : IRepositoriesContainer
	{
		private readonly InnostarModelsContext _dataContext = new InnostarModelsContext();

		public IRepositoryBase<TModel> RepositoryFor<TModel>() where TModel : ModelBase, new()
		{
			IRepositoryBase<TModel> result = new RepositoryBase<InnostarModelsContext, TModel>(_dataContext);

			return result;
		}

        //public ICatalogsRepository<TCatalog> CatalogsRepository<TCatalog>() where TCatalog : CatalogBase, new()
        //{
        //    return new CatalogsRepository<TCatalog>(_dataContext);
        //}

        //public IMapObjectsRepository MapObjectsRepository
        //{
        //    get;
        //    set;
        //}

        //public IProjectPersonRelationRepository ProjectPersonRelationRepository
        //{
        //    get;
        //    set;
        //}

        //public IMapOrganizationsRepository MapOrganizationsRepository
        //{
        //    get;
        //    set;
        //}

        //public IOrganizationsRelationsRepository OrganizationsRelations
        //{
        //    get;
        //    set;
        //}

        //public IOrganizationProjectRelationsRepository OrganizationProjectRelations
        //{
        //    get;
        //    set;
        //}

        //public IOrganizationPersonRelationsRepository OrganizationPersonRelations
        //{
        //    get;
        //    set;
        //}

        //public IMapPersonsRepository MapPersonsRepository
        //{
        //    get;
        //    set;
        //}

        //public IMapProjectsRepository MapProjectsRepository
        //{
        //    get;
        //    set;
        //}

        //public IMapResourcesRepository MapResourcesRepository
        //{
        //    get;
        //    set;
        //}

		public void ApplyChanges()
		{
			_dataContext.SaveChanges();
		}

		public void Dispose()
		{
			_dataContext.Dispose();
		}

		public InnostarRepositoriesContainer()
		{
            //MapObjectsRepository = new MapObjectsRepository(_dataContext);
            //MapOrganizationsRepository = new MapOrganizationsRepository(_dataContext);
            //MapPersonsRepository = new MapPersonsRepository(_dataContext);
            //MapProjectsRepository = new MapProjectsRepository(_dataContext);
            //MapResourcesRepository = new MapResourcesRepository(_dataContext);
            //OrganizationsRelations = new OrganizationsRelationsRepository(_dataContext);
            //OrganizationProjectRelations = new OrganizationProjectRelationsRepository(_dataContext);
            //OrganizationPersonRelations = new OrganizationPersonRelationsRepository(_dataContext);
            //ProjectPersonRelationRepository = new ProjectPersonRelationRepository(_dataContext);
		}
	}
}
