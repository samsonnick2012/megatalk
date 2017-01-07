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
    public class ChatUserRepository : GenericRepository<ChatUser>, IChatUserRepository
    {

        public override IQueryable<ChatUser> All
        {
            get { return context.ChatUsers; }
        }

        public override IQueryable<ChatUser> AllIncluding(params Expression<Func<ChatUser, object>>[] includeProperties)
        {
            IQueryable<ChatUser> query = context.ChatUsers;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public override void InsertOrUpdate(ChatUser obj)
        {
            if (obj.Id == default(int))
            {
                // New entity
                context.ChatUsers.Add(obj);
            }
            else
            {
                // Existing entity
                context.Entry(obj).State = EntityState.Modified;
            }
        }

        public ChatUser GetUserByLogin(string login)
        {
            return Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
        }

        public ChatUser GetUserByXmppLogin(string login)
        {
            return Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
        }
    }

    public interface IChatUserRepository : IGenericRepository<ChatUser>
    {
    }
}