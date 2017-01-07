using System;
using System.Collections.Generic;

namespace Application.Core.Data
{
	public interface IRepositoryBaseExtended<TModel> where TModel : class
	{
		#region IRepositoryBaseExtended Members

        void Add(IEnumerable<TModel> models);

        void Delete(IEnumerable<TModel> models);

        void Save(IEnumerable<TModel> models, Func<TModel, bool> performUpdate = null);

        void Update(IEnumerable<TModel> models);

		#endregion IRepositoryBaseExtended Members
	}
}
