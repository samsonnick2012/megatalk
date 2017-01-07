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
    public class UserTokenRepository : RepositoryBase<InnostarModelsContext, UserToken>
    {
        public UserTokenRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<UserToken> _All
        {
            get { return DataContext.UserTokens; }
        }

        public IQueryable<UserToken> _AllIncluding(params Expression<Func<UserToken, object>>[] includeProperties)
        {
            IQueryable<UserToken> query = DataContext.UserTokens;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(UserToken UserToken)
        {
            if (UserToken.Id == default(int))
            {
                // New entity
                DataContext.UserTokens.Add(UserToken);
            }
            else
            {
                // Existing entity
                DataContext.Entry(UserToken).State = EntityState.Modified;
            }
        }
    }

}