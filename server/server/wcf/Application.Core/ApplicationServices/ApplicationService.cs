using System;
using System.Data.Entity.Validation;
using Application.Core.Exceptions;

namespace Application.Core.ApplicationServices
{
	public class ApplicationService : IApplicationService
	{
		protected TResult ProcessMethod<TResult>(Func<TResult> func)
		{
			TResult result;

			try
			{
				result = func();
			}
			catch (DataException exception)
			{
				throw new ApplicationServiceException("Ошибка уровня сервиса приложения", exception);
			}
			catch (Exception exception)
			{
				throw new ApplicationServiceException("Ошибка уровня сервиса приложения", exception);
			}

			return result;
		}

		protected void ProcessMethod(Action action)
		{
			try
			{
				action();
			}
			catch (DbEntityValidationException e)
			{
				foreach (var eve in e.EntityValidationErrors)
				{
					var t = eve.Entry.Entity.GetType().Name;
					var ix = eve.Entry.State;

					foreach (var ve in eve.ValidationErrors)
					{
						var t1 = 0;
					}
				}
			}
			catch (DataException exception)
			{
				throw new ApplicationServiceException("Ошибка уровня сервиса приложения", exception);
			}
			catch (Exception exception)
			{
				throw new ApplicationServiceException("Ошибка уровня сервиса приложения", exception);
			}
		}
	}
}
