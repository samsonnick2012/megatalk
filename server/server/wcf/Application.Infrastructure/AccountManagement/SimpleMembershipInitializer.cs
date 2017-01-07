using System;
using System.Data.Entity;
using System.Threading;
using System.Web.Security;
using Innostar.Dal.Infrastructure;
using WebMatrix.WebData;

namespace Application.Infrastructure.AccountManagement
{
    public class SimpleMembershipInitializer
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public static void Initialize()
        {
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        public SimpleMembershipInitializer()
        {
            Database.SetInitializer<InnostarModelsContext>(null);

            try
            {
                bool firstLoad;

				using (var context = new InnostarModelsContext())
                {
                    firstLoad = DatabaseInitializer.InitializeDatabase(context);
                }

                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "inf_Users", "UserId", "UserName", true);

                if (firstLoad)
                {
                    WebSecurity.CreateUserAndAccount("admin", "123456");
                    Roles.AddUserToRole("admin", "admin");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка", ex);
            }
        }
    }
}