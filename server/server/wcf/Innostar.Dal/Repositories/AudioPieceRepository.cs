using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Application.Core.Data;

using Innostar.Dal.Infrastructure;
using Innostar.Models;

namespace Innostar.Dal.Repositories
{
    public class AudioPieceRepository : RepositoryBase<InnostarModelsContext, AudioPiece>
    {
        public AudioPieceRepository(InnostarModelsContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<AudioPiece> _All
        {
            get { return DataContext.AudioPieces; }
        }

        public IQueryable<AudioPiece> _AllIncluding(params Expression<Func<AudioPiece, object>>[] includeProperties)
        {
            IQueryable<AudioPiece> query = DataContext.AudioPieces;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public void _InsertOrUpdate(AudioPiece audioPiece)
        {
            if (audioPiece.Id == default(int))
            {
                // New entity
                DataContext.AudioPieces.Add(audioPiece);
            }
            else
            {
                // Existing entity
                DataContext.Entry(audioPiece).State = EntityState.Modified;
            }
        }

        public IEnumerable<string> GetPiecePaths(string key)
        {
            var result = new List<string>();

            DataContext.AudioPieces.Where(e => e.Key == key && !e.ReadyForRemoving).OrderBy(e => e.Order)
                .ToList().ForEach(e => result.Add(e.PhysicalFileName));

            return result;
        }
    }
}
