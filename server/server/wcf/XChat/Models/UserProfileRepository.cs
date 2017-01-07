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
    public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
    {

        public override IQueryable<UserProfile> All
        {
            get { return context.UserProfiles; }
        }

        public override IQueryable<UserProfile> AllIncluding(params Expression<Func<UserProfile, object>>[] includeProperties)
        {
            IQueryable<UserProfile> query = context.UserProfiles;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public override void InsertOrUpdate(UserProfile userprofile)
        {
            if (userprofile.Id == default(int)) {
                // New entity
                context.UserProfiles.Add(userprofile);
            } else {
                // Existing entity
                context.Entry(userprofile).State = EntityState.Modified;
            }
        }
    }

    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {
    }
}