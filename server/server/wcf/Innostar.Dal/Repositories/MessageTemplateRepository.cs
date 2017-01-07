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
    public class MessageTemplateRepository : RepositoryBase<InnostarModelsContext, MessageTemplate>
    {
        public MessageTemplateRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<MessageTemplate> _All
        {
            get { return DataContext.MessageTemplates; }
        }

        public IQueryable<MessageTemplate> _AllIncluding(params Expression<Func<MessageTemplate, object>>[] includeProperties)
        {
            IQueryable<MessageTemplate> query = DataContext.MessageTemplates;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(MessageTemplate messageTemplate)
        {
            if (messageTemplate.Id == default(int))
            {
                // New entity
                DataContext.MessageTemplates.Add(messageTemplate);
            }
            else
            {
                // Existing entity
                DataContext.Entry(messageTemplate).State = EntityState.Modified;
            }
        }
    }
}
