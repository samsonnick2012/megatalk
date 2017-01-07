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
    public class UserProfileRepository : RepositoryBase<InnostarModelsContext, UserProfile>
    {
        public UserProfileRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<UserProfile> _All
        {
            get { return DataContext.UserProfiles; }
        }

        public IQueryable<UserProfile> _AllIncluding(params Expression<Func<UserProfile, object>>[] includeProperties)
        {
            IQueryable<UserProfile> query = DataContext.UserProfiles;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(UserProfile UserProfile)
        {
            if (UserProfile.Id == default(int))
            {
                // New entity
                DataContext.UserProfiles.Add(UserProfile);
            }
            else
            {
                // Existing entity
                DataContext.Entry(UserProfile).State = EntityState.Modified;
            }
        }
    }

}