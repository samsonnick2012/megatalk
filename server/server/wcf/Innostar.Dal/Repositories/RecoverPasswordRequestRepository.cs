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
    public class RecoverPasswordRequestRepository : RepositoryBase<InnostarModelsContext, RecoverPasswordRequest>
    {
        public RecoverPasswordRequestRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<RecoverPasswordRequest> _All
        {
            get { return DataContext.RecoverPasswordRequests; }
        }

        public IQueryable<RecoverPasswordRequest> _AllIncluding(params Expression<Func<RecoverPasswordRequest, object>>[] includeProperties)
        {
            IQueryable<RecoverPasswordRequest> query = DataContext.RecoverPasswordRequests;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(RecoverPasswordRequest RecoverPasswordRequest)
        {
            if (RecoverPasswordRequest.Id == default(int))
            {
                // New entity
                DataContext.RecoverPasswordRequests.Add(RecoverPasswordRequest);
            }
            else
            {
                // Existing entity
                DataContext.Entry(RecoverPasswordRequest).State = EntityState.Modified;
            }
        }
    }
}