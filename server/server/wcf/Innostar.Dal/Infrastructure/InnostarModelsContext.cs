using System.Data.Entity;
using Innostar.Dal.Infrastructure.EntityTypeConfigurations;
using Innostar.Models;

namespace Innostar.Dal.Infrastructure
{
	public class InnostarModelsContext : DbContext
	{
		public InnostarModelsContext()
			: base("name=DefaultConnection")
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new MembershipTypeConfiguration());
            modelBuilder.Configurations.Add(new NotificationTypeConfiguration());
            modelBuilder.Configurations.Add(new OAuthMembershipTypeConfiguration());
            modelBuilder.Configurations.Add(new RoleTypeConfiguration());
            modelBuilder.Configurations.Add(new UserTypeConfiguration());

            ConfigureServicedEntities(modelBuilder);
		}

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<DataFile> DataFiles { get; set; }

        public DbSet<AudioPiece> AudioPieces { get; set; }

        public DbSet<RoomKey> RoomKeys { get; set; }

        public DbSet<RecoverPasswordRequest> RecoverPasswordRequests { get; set; }

        public DbSet<Error> Errors { get; set; }

        public DbSet<ConfirmEmailRequest> ConfirmEmailRequests { get; set; }

        public DbSet<MessageTemplate> MessageTemplates { get; set; }

        public DbSet<ConferenceConfiguration> ConferenceConfigurations { get; set; }

        private static void ConfigureServicedEntities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>().ToTable("inf_Notifications");
            modelBuilder.Entity<UserNotification>().ToTable("inf_UserNotifications");
        }
	}
}
