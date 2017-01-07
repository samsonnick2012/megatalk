using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using XChat.Models.DB;

namespace XChat.Models
{
    public class RoomKeyRepository : GenericRepository<RoomKey>, IRoomKeyRepository
    {
        public override IQueryable<RoomKey> All
        {
            get { return context.RoomKeys; }
        }

        public override IQueryable<RoomKey> AllIncluding(params Expression<Func<RoomKey, object>>[] includeProperties)
        {
            IQueryable<RoomKey> query = context.RoomKeys;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public override void InsertOrUpdate(RoomKey roomKey)
        {
            if (roomKey.Id == default(int))
            {
                // New entity
                context.RoomKeys.Add(roomKey);
            }
            else
            {
                // Existing entity
                context.Entry(roomKey).State = EntityState.Modified;
            }
        }
    }
}