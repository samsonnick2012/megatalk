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
    public class ConferenceConfigurationRepository : RepositoryBase<InnostarModelsContext, ConferenceConfiguration>
    {
        public ConferenceConfigurationRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<ConferenceConfiguration> _All
        {
            get { return DataContext.ConferenceConfigurations; }
        }

        public IQueryable<ConferenceConfiguration> _AllIncluding(params Expression<Func<ConferenceConfiguration, object>>[] includeProperties)
        {
            IQueryable<ConferenceConfiguration> query = DataContext.ConferenceConfigurations;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(ConferenceConfiguration conferenceConfiguration)
        {
            if (conferenceConfiguration.Id == default(int))
            {
                // New entity
                DataContext.ConferenceConfigurations.Add(conferenceConfiguration);
            }
            else
            {
                // Existing entity
                DataContext.Entry(conferenceConfiguration).State = EntityState.Modified;
            }
        }
    }
}
