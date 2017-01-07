using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innostar.Models
{
	public class OAuthMembership
	{
		[Key, Column(Order = 0), StringLength(30)]
		public string Provider
		{
			get;
			set;
		}

		[Key, Column(Order = 1), StringLength(100)]
		public string ProviderUserId
		{
			get;
			set;
		}

        [Column("UserId"), InverseProperty("OAuthMemberships")]
        public Membership User
        {
            get;
            set;
        }

        public int UserId
		{
			get;
			set;
		}
	}
}