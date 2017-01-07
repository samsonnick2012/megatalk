using System.Collections.Generic;

namespace Application.Core.Data
{
	public class ModelComparerBase : IEqualityComparer<ModelBase>
	{
		public bool Equals(ModelBase x, ModelBase y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(ModelBase obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
