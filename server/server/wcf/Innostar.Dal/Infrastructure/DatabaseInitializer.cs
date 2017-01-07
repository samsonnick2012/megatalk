using System.Data.Entity.Infrastructure;
using Innostar.Models;

namespace Innostar.Dal.Infrastructure
{
	public class DatabaseInitializer
	{
		private static void FillInitData(InnostarModelsContext context)
		{
			FillRoles(context);
            FillTemplates(context);

			context.SaveChanges();
		}

		private static void FillRoles(InnostarModelsContext context)
		{
			context.Set<Role>().Add(new Role
			{
				RoleName = "admin",
				RoleDisplayName = "Администратор"
			});
			context.Set<Role>().Add(new Role
			{
				RoleName = "editor",
				RoleDisplayName = "Редактор"
			});
			context.Set<Role>().Add(new Role
			{
				RoleName = "chiefeditor",
				RoleDisplayName = "Главный редактор"
			});
			context.Set<Role>().Add(new Role
			{
				RoleName = "user",
				RoleDisplayName = "Зарегистрированный пользователь"
			});
			context.Set<Role>().Add(new Role
			{
				RoleName = "collectioncreator",
				RoleDisplayName = "Создатель коллекций"
			});
		}

	    private static void FillTemplates(InnostarModelsContext context)
	    {
	        context.Set<MessageTemplate>().Add(new MessageTemplate
	        {
	            Message = "Восстановление пароля",
                Title = "Восстановление пароля",
                SpecialTemplate = 1,
                Blocked = true
	        });
            context.Set<MessageTemplate>().Add(new MessageTemplate
            {
                Message = "Подтверждение email",
                Title = "Подтверждение email",
                SpecialTemplate = 2,
                Blocked = true
            });
            context.Set<MessageTemplate>().Add(new MessageTemplate
            {
                Message = "Добавление контакта",
                Title = "Добавление контакта",
                SpecialTemplate = 3,
                Blocked = true
            });
            context.Set<MessageTemplate>().Add(new MessageTemplate
            {
                Message = "Уведомление об окончании подписки",
                Title = "Уведомление об окончании подписки",
                SpecialTemplate = 4,
                Blocked = true
            });
	    }

	    public static bool InitializeDatabase(InnostarModelsContext dataContext)
		{
			if (!dataContext.Database.Exists())
			{
				((IObjectContextAdapter)dataContext).ObjectContext.CreateDatabase();
				FillInitData(dataContext);

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}