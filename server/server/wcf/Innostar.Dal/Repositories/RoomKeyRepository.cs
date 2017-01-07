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
    public class RoomKeyRepository: RepositoryBase<InnostarModelsContext, RoomKey>
    {
        public RoomKeyRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<RoomKey> _All
        {
            get { return DataContext.RoomKeys; }
        }

        public IQueryable<RoomKey> _AllIncluding(params Expression<Func<RoomKey, object>>[] includeProperties)
        {
            IQueryable<RoomKey> query = DataContext.RoomKeys;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(RoomKey RoomKey)
        {
            if (RoomKey.Id == default(int))
            {
                // New entity
                DataContext.RoomKeys.Add(RoomKey);
            }
            else
            {
                // Existing entity
                DataContext.Entry(RoomKey).State = EntityState.Modified;
            }
        }
    }
}