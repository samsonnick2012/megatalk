using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Application.Core.Data
{
    public interface IRepositoryBase<TModel> where TModel : IHasIdentifyKey
    {
        #region IRepositoryBase Members

        void Delete(TModel model);

        IPageableList<TModel> GetPageableBy(IPageableQuery<TModel> query = null);

        IList<TModel> GetAll(IQuery<TModel> query = null);

        TModel GetBy(IQuery<TModel> query);

        void Save(TModel model, Func<TModel, bool> performUpdate = null);

        void Delete(IEnumerable<TModel> models);

        void Save(IEnumerable<TModel> models, Func<TModel, bool> performUpdate = null);

        #endregion IRepositoryBase Members

        IEnumerable<TModel> _Get(
            Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            string includeProperties = "");

        TModel _Find(object id);
        void _Insert(TModel entity);
        void _Delete(object id);
        void _Delete(TModel entityToDelete);
        void _Update(TModel entityToUpdate);
        IEnumerable<TModel> _GetWithRawSql(string query, params object[] parameters);
        void _Save();
    }
}