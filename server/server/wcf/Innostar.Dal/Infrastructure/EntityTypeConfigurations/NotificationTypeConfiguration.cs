using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innostar.Models;

namespace Innostar.Dal.Infrastructure.EntityTypeConfigurations
{
	public class NotificationTypeConfiguration : EntityTypeConfiguration<Notification>
	{
		public NotificationTypeConfiguration()
		{
			HasMany(e => e.UsersNotifications)
				.WithRequired(e => e.Notification)
				.HasForeignKey(e => e.NotificationId)
				.WillCascadeOnDelete(false);
		}
	}
}
