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
    public class DataFileRepository : RepositoryBase<InnostarModelsContext, DataFile>
    {
        public DataFileRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<DataFile> _All
        {
            get { return DataContext.DataFiles; }
        }

        public IQueryable<DataFile> _AllIncluding(params Expression<Func<DataFile, object>>[] includeProperties)
        {
            IQueryable<DataFile> query = DataContext.DataFiles;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(DataFile datafile)
        {
            if (datafile.Id == default(int))
            {
                // New entity
                DataContext.DataFiles.Add(datafile);
            }
            else
            {
                // Existing entity
                DataContext.Entry(datafile).State = EntityState.Modified;
            }
        }
    }

}