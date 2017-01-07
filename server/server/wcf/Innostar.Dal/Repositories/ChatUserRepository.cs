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
    public class ChatUserRepository : RepositoryBase<InnostarModelsContext, ChatUser>
    {
        public ChatUserRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<ChatUser> _All
        {
            get { return DataContext.ChatUsers; }
        }

        public IQueryable<ChatUser> _AllIncluding(params Expression<Func<ChatUser, object>>[] includeProperties)
        {
            IQueryable<ChatUser> query = DataContext.ChatUsers;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(ChatUser obj)
        {
            if (obj.Id == default(int))
            {
                // New entity
                DataContext.ChatUsers.Add(obj);
            }
            else
            {
                // Existing entity
                DataContext.Entry(obj).State = EntityState.Modified;
            }
        }

        public ChatUser GetUserByLogin(string login)
        {
            return _Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
        }

        public ChatUser GetUserByXmppLogin(string login)
        {
            return _Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
        }
    }
}