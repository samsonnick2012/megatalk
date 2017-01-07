using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using XChat.Models.DB;

namespace XChat.Models
{
    public class RecoverPasswordRequestRepository : GenericRepository<RecoverPasswordRequest>, IRecoverPasswordRequestRepository
    {
        public override IQueryable<RecoverPasswordRequest> All
        {
            get { return context.RecoverPasswordRequests; }
        }

        public override IQueryable<RecoverPasswordRequest> AllIncluding(params Expression<Func<RecoverPasswordRequest, object>>[] includeProperties)
        {
            IQueryable<RecoverPasswordRequest> query = context.RecoverPasswordRequests;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public override void InsertOrUpdate(RecoverPasswordRequest recoverPasswordRequest)
        {
            if (recoverPasswordRequest.Id == default(int))
            {
                // New entity
                context.RecoverPasswordRequests.Add(recoverPasswordRequest);
            }
            else
            {
                // Existing entity
                context.Entry(recoverPasswordRequest).State = EntityState.Modified;
            }
        }
    }
}