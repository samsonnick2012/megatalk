using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Innostar.Models;

namespace Innostar.Dal.Infrastructure.EntityTypeConfigurations
{
	public class MembershipTypeConfiguration : EntityTypeConfiguration<Membership>
	{
		public MembershipTypeConfiguration()
		{
			Map(m => m.ToTable("webpages_Membership"))
				.Property(m => m.Id)
				.HasColumnName("UserId")
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			HasMany(r => r.Roles)
			  .WithMany(u => u.Members)
			  .Map(m =>
			  {
				  m.ToTable("webpages_UsersInRoles");
				  m.MapLeftKey("UserId");
				  m.MapRightKey("RoleId");
			  });
		}
	}
}
