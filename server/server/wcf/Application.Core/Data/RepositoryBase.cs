using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using DataException = Application.Core.Exceptions.DataException;

namespace Application.Core.Data
{
    public class RepositoryBase<TDataContext, TModel> : IRepositoryBase<TModel>
        where TModel : ModelBase, new()
        where TDataContext : DbContext
    {
        private readonly TDataContext _dataContext;
        protected DbSet<TModel> dbSet;

        public RepositoryBase(TDataContext dataContext)
        {
            _dataContext = dataContext;
            this.dbSet = DataContext.Set<TModel>();
        }

        protected TDataContext DataContext
        {
            get
            {
                return _dataContext;
            }
        }

        protected void Add(TModel model)
        {
            ProcessMethod(() => PerformAdd(model, DataContext));
        }

        protected void Add(IEnumerable<TModel> models)
        {
            ProcessMethod(() =>
            {
                foreach (var model in models)
                {
                    PerformAdd(model, DataContext);
                }
            });
        }

        public void Delete(TModel model)
        {
            ProcessMethod(() => PerformDelete(model, DataContext));
        }

        public void Delete(IEnumerable<TModel> models)
        {
            ProcessMethod(() =>
            {
                foreach (var model in models)
                {
                    PerformDelete(model, DataContext);
                }
            });
        }

        public IPageableList<TModel> GetPageableBy(IPageableQuery<TModel> query = null)
        {
            return ProcessMethod(() =>
            {
                if (query == null)
                {
                    query = new PageableQuery<TModel>(new PageInfo());
                }

                var items = PerformGetBy(query, DataContext).ToPageableList(query.PageInfo.PageNumber, query.PageInfo.PageSize);

                DataContext.Dispose();

                return items;
            });
        }

        public IList<TModel> GetAll(IQuery<TModel> query = null)
        {
            return ProcessMethod(() => PerformGetAll());
        }

        protected ICollection<TEntity> GetEntitiesForSaving<TEntity>(ICollection<TEntity> currentEntities, ICollection<TEntity> newValues, TDataContext dataContext) where TEntity : ModelBase
        {
            var forDelete = currentEntities.Except(newValues, new ModelComparerBase()).ToList();
            var forAdd = newValues.Except(currentEntities, new ModelComparerBase());

            foreach (var entity in forAdd)
            {
                currentEntities.Add(dataContext.Set<TEntity>().SingleOrDefault(e => e.Id == entity.Id));
            }

            foreach (var entity in forDelete)
            {
                currentEntities.Remove(entity as TEntity);
            }

            return currentEntities;
        }

		protected ICollection<TEntity> MarkEntitiesForDeleting<TEntity>(ICollection<TEntity> currentEntities) where TEntity : ModelBase
		{
			var forDelete = currentEntities.ToList();

			foreach (var entity in forDelete)
			{
				currentEntities.Remove(entity);
			}

			return currentEntities;
		}

        protected virtual IList<TModel> PerformGetAll()
        {
            return DataContext.Set<TModel>().ToList();
        }

        public TModel GetBy(IQuery<TModel> query)
        {
            return ProcessMethod(() =>
            {
                var item = PerformSelectSingle(query, DataContext);
                DataContext.Dispose();
                return item;
            });
        }

        public void Save(TModel model, Func<TModel, bool> performUpdate = null)
        {
            if (model != null)
            {
                if (performUpdate == null)
                {
                    performUpdate = e => !e.IsNew;
                }

                if (performUpdate(model))
                {
                    Update(model);
                }
                else
                {
                    Add(model);
                }
            }
        }

        public void Save(IEnumerable<TModel> models, Func<TModel, bool> performUpdate = null)
        {
            ProcessMethod(() =>
            {
                foreach (var model in models)
                {
                    if (model != null)
                    {
                        if (performUpdate == null)
                        {
                            performUpdate = e => !e.IsNew;
                        }

                        if (performUpdate(model))
                        {
                            PerformUpdate(model, _dataContext);
                        }
                        else
                        {
                            PerformAdd(model, _dataContext);
                        }
                    }
                }
            });
        }

        protected void Update(TModel model)
        {
            ProcessMethod(() =>
            {
                if (model != null)
                {
                    PerformUpdate(model, DataContext);
                }
            });
        }

        protected void Update(IEnumerable<TModel> models)
        {
            ProcessMethod(() =>
            {
                foreach (var model in models)
                {
                    if (model != null)
                    {
                        PerformUpdate(model, _dataContext);
                    }
                }
            });
        }

        protected virtual void PerformAdd(TModel model, TDataContext dataContext)
        {
            dataContext.Set<TModel>().Add(model);
        }

        protected virtual void PerformDelete(TModel model, TDataContext dataContext)
        {
            var deletingEntity = dataContext.Set<TModel>().Find(model.Id);

            if (deletingEntity != null)
            {
                dataContext.Set<TModel>().Remove(deletingEntity);
            }
        }

        protected virtual IOrderedQueryable<TModel> PerformGetBy(IPageableQuery<TModel> query, TDataContext dataContext)
        {
            var sequence = ApplyFilters(query, dataContext.Set<TModel>());

            sequence = ApplyIncludedProperties(query, sequence);

            sequence = ApplySortCriterias(query, sequence);

            return sequence as IOrderedQueryable<TModel>;
        }

        protected virtual TModel PerformSelectSingle(IQuery<TModel> query, TDataContext dataContext)
        {
            var sequence = ApplyFilters(query, dataContext.Set<TModel>());
            sequence = ApplyIncludedProperties(query, sequence);

            return sequence.SingleOrDefault();
        }

        protected virtual void PerformUpdate(TModel newValue, TDataContext dataContext)
        {
            dataContext.Update(newValue);
        }

        protected TResult ProcessMethod<TResult>(Func<TResult> func)
        {
            TResult result;

            try
            {
                result = func();
            }
            catch (Exception exception)
            {
                throw new DataException("Ошибка уровня доступа к данным", exception);
            }

            return result;
        }

        protected void ProcessMethod(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                throw new DataException("При сохранении данных возникла ошибка.", exception);
            }
        }

        protected IQueryable<TModel> ApplyFilters(IQuery<TModel> searchCritery, IQueryable<TModel> sequence)
        {
            if (searchCritery.Filters != null && searchCritery.Filters.Any())
            {
                sequence = searchCritery.Filters.Aggregate(sequence, (current, filterClause) => current.Where(filterClause));
            }

            return sequence;
        }

        protected IQueryable<TModel> ApplyIncludedProperties(IQuery<TModel> searchCritery, IQueryable<TModel> sequence)
        {
            return searchCritery.IncludedProperties.Aggregate(sequence, (current, includeProperty) => current.Include(includeProperty));
        }

        protected IOrderedQueryable<TModel> ApplySortCriterias(IPageableQuery<TModel> searchCritery, IQueryable<TModel> sequence)
        {
            var result = sequence.OrderBy(e => e.Id);

            return searchCritery.SortCriterias.Aggregate(result, (current, sortCriteria) => sortCriteria.SortDirection == SortDirection.Asc ? current.OrderByAsc(sortCriteria.Name) : current.OrderByDesc(sortCriteria.Name));
        }

        #region fromGenRep

        public virtual IEnumerable<TModel> _Get(
            Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TModel> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TModel _Find(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void _Insert(TModel entity)
        {
            dbSet.Add(entity);
        }

        public virtual void _Delete(object id)
        {
            TModel entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void _Delete(TModel entityToDelete)
        {
            if (DataContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void _Update(TModel entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            DataContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual IEnumerable<TModel> _GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }


        public virtual void _Save()
        {
            try
            {
                DataContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

        }

        #endregion
    }
}
