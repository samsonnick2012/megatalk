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
    public class ConfirmEmailRequestRepository : RepositoryBase<InnostarModelsContext, ConfirmEmailRequest>
    {
        public ConfirmEmailRequestRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<ConfirmEmailRequest> _All
        {
            get { return DataContext.ConfirmEmailRequests; }
        }

        public IQueryable<ConfirmEmailRequest> _AllIncluding(params Expression<Func<ConfirmEmailRequest, object>>[] includeProperties)
        {
            IQueryable<ConfirmEmailRequest> query = DataContext.ConfirmEmailRequests;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(ConfirmEmailRequest confirmEmailRequest)
        {
            if (confirmEmailRequest.Id == default(int))
            {
                // New entity
                DataContext.ConfirmEmailRequests.Add(confirmEmailRequest);
            }
            else
            {
                // Existing entity
                DataContext.Entry(confirmEmailRequest).State = EntityState.Modified;
            }
        }
    }
}
