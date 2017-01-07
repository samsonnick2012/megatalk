using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Core.Data
{
	public abstract class ModelBase : IHasIdentifyKey
	{
        [Key]
		public int Id
		{
			get;
			set;
		}

		public virtual bool IsNew
		{
			get
			{
				return Id == 0;
			}
		}
	}
}
