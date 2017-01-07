using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Application.Core.Data;
using Innostar.Dal.Infrastructure;
using Innostar.Models;

namespace Innostar.Dal.Repositories
{
    class ErrorRepository : RepositoryBase<InnostarModelsContext, Error>
    {
        public ErrorRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<Error> _All
        {
            get { return DataContext.Errors; }
        }

        public IQueryable<Error> _AllIncluding(params Expression<Func<Error, object>>[] includeProperties)
        {
            IQueryable<Error> query = DataContext.Errors;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
    }
}
