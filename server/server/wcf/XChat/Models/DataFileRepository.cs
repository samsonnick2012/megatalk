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
    public class DataFileRepository : GenericRepository<DataFile>, IDataFileRepository
    {
        public override IQueryable<DataFile> All
        {
            get { return context.DataFiles; }
        }

        public override IQueryable<DataFile> AllIncluding(params Expression<Func<DataFile, object>>[] includeProperties)
        {
            IQueryable<DataFile> query = context.DataFiles;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public override void InsertOrUpdate(DataFile datafile)
        {
            if (datafile.Id == default(int))
            {
                // New entity
                context.DataFiles.Add(datafile);
            } else {
                // Existing entity
                context.Entry(datafile).State = EntityState.Modified;
            }
        }

    }

    public interface IDataFileRepository : IGenericRepository<DataFile>
    {
    }
}