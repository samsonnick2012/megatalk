using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Application.Core.Data;

namespace Innostar.Models
{
	public class Role : ModelBase
	{
		public Role()
		{
			Members = new List<Membership>();
		}

        public ICollection<Membership> Members
        {
            get;
            set;
        }

        [StringLength(256)]
        public string RoleDisplayName
        {
            get;
            set;
        }

        [StringLength(256)]
		public string RoleName
		{
			get;
			set;
		}
	}
}