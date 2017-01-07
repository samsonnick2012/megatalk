using System.Data.Entity.ModelConfiguration;
using Innostar.Models;

namespace Innostar.Dal.Infrastructure.EntityTypeConfigurations
{
	public class UserTypeConfiguration : EntityTypeConfiguration<User>
	{
		public UserTypeConfiguration()
		{
			Map(m => m.ToTable("inf_Users"))
				.Property(m => m.Id)
				.HasColumnName("UserId");

			HasRequired(e => e.Membership)
				.WithRequiredPrincipal(e => e.User);

            HasMany(e => e.UsersNotifications)
                .WithRequired(e => e.ResipientUser)
                .HasForeignKey(e => e.ResipientUserId)
                .WillCascadeOnDelete(false);
		}
	}
}
