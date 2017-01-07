using System.Data.Entity.ModelConfiguration;
using Innostar.Models;

namespace Innostar.Dal.Infrastructure.EntityTypeConfigurations
{
	public class RoleTypeConfiguration : EntityTypeConfiguration<Role>
	{
		public RoleTypeConfiguration()
		{
			Map(m => m.ToTable("webpages_Roles"))
				.Property(m => m.Id)
				.HasColumnName("RoleId");
		}
	}
}
