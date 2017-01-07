using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using XChat.Models.DB;

namespace XChat.Models
{
    public class UserTokenRepository : GenericRepository<UserToken>, IUserTokenRepository
    {
        public override IQueryable<UserToken> All
        {
            get { return context.UserTokens; }
        }

        public override IQueryable<UserToken> AllIncluding(params Expression<Func<UserToken, object>>[] includeProperties)
        {
            IQueryable<UserToken> query = context.UserTokens;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public override void InsertOrUpdate(UserToken usertoken)
        {
            if (usertoken.Id == default(int))
            {
                // New entity
                context.UserTokens.Add(usertoken);
            }
            else
            {
                // Existing entity
                context.Entry(usertoken).State = EntityState.Modified;
            }
        }
    }

    public interface IUserTokenRepository : IGenericRepository<UserToken>
    {
    }
}