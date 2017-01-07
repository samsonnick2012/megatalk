using System;
using System.IO;
using System.Web.Mvc;
using Application.Core.Exceptions;

namespace Application.Core.UI.Controllers
{
	public abstract class BasicController : Controller
	{
		public ActionResult UnderConstruction()
		{
			return View();
		}

		public string PartialViewToString(string viewName, object model)
		{
			if (string.IsNullOrEmpty(viewName))
			{
				viewName = ControllerContext.RouteData.GetRequiredString("action");
			}

			ViewData.Model = model;

			using (var stringWriter = new StringWriter())
			{
				var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, stringWriter);
				viewResult.View.Render(viewContext, stringWriter);
				return stringWriter.GetStringBuilder().ToString();
			}
		}

		protected TResult ProcessMethod<TResult>(Func<TResult> func)
		{
			TResult result;

			try
			{
				result = func();
			}
			catch (ApplicationServiceException exception)
			{
				throw new ApplicationUIException(string.Format("В процессе обработки запроса возникла ошибка.\n Обновите страницу и свяжитесь с администратором."), exception);
			}
			catch (DataException exception)
			{
				throw new ApplicationUIException(string.Format("В процессе обработки запроса возникла ошибка.\n Обновите страницу и свяжитесь с администратором."), exception);
			}
			catch (Exception exception)
			{
				throw new ApplicationUIException(string.Format("В процессе обработки запроса возникла ошибка.\n Обновите страницу и свяжитесь с администратором."), exception);
			}

			return result;
		}

		protected void ProcessMethod(Action action)
		{
			try
			{
				action();
			}
			catch (ApplicationServiceException exception)
			{
				throw new ApplicationUIException(string.Format("В процессе обработки запроса возникла ошибка.\n Обновите страницу и свяжитесь с администратором."), exception);
			}
			catch (DataException exception)
			{
				throw new ApplicationUIException(string.Format("В процессе обработки запроса возникла ошибка.\n Обновите страницу и свяжитесь с администратором."), exception);
			}
			catch (Exception exception)
			{
				throw new ApplicationUIException(string.Format("В процессе обработки запроса возникла ошибка.\n Обновите страницу и свяжитесь с администратором."), exception);
			}
		}
	}
}
