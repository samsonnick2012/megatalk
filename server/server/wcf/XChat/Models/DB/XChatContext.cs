using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace XChat.Models.DB
{
    public class XChatContext : DbContext
    {
        public XChatContext(): base("DefaultConnection")
        {
        }

        public static XChatContext Current
        {
            get { return HttpContext.Current.Items["_EntityContext"] as XChatContext; }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<DataFile> DataFiles { get; set; }

        public DbSet<RoomKey> RoomKeys { get; set; }

        public DbSet<RecoverPasswordRequest> RecoverPasswordRequests { get; set; }
    }
}