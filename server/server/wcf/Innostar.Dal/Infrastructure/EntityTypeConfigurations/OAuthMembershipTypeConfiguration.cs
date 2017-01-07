using System.Data.Entity.ModelConfiguration;
using Innostar.Models;

namespace Innostar.Dal.Infrastructure.EntityTypeConfigurations
{
	public class OAuthMembershipTypeConfiguration : EntityTypeConfiguration<OAuthMembership>
	{
		public OAuthMembershipTypeConfiguration()
		{
			Map(m => m.ToTable("webpages_OAuthMembership"));
		}
	}
}
