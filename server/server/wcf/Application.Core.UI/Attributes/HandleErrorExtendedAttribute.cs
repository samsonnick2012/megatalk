using System;
using System.Text;
using System.Web.Mvc;
using Application.Core.Exceptions;
using Application.Infrastructure.Logging;
using log4net;

namespace Application.Core.UI.Attributes
{
	public class HandleErrorExtendedAttribute : HandleErrorAttribute
	{
		private readonly ILog _logger;

		public HandleErrorExtendedAttribute()
		{
			_logger = LoggerWrapper.Create(typeof(HandleErrorExtendedAttribute));
		}

		public override void OnException(ExceptionContext filterContext)
		{
			if (!filterContext.ExceptionHandled)
			{
				if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
				{
					var errorMessage = "В процессе обработки запроса возникла ошибка.\n Обновите страницу и свяжитесь с администратором.";

					if (filterContext.Exception is ApplicationUIException)
					{
						errorMessage = filterContext.Exception.Message;
					}

					filterContext.Result = new JsonResult
					{
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						Data = new
						{
							error = true,
							message = errorMessage
						}
					};

					var errorGUID = Guid.NewGuid().ToString().ToUpper();

					_logger.Error(GenerateErrorMessage(errorGUID, filterContext.Exception.InnerException ?? filterContext.Exception));

					filterContext.ExceptionHandled = true;
					filterContext.HttpContext.Response.Clear();
					filterContext.HttpContext.Response.StatusCode = 500;

					filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
				}
			}
		}

		private static string GenerateErrorMessage(string errorIdentificator, Exception exception)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format(" ID ошибки : {0}", errorIdentificator));
			stringBuilder.AppendLine(exception.ToString());
			stringBuilder.AppendLine(string.Empty);

			return stringBuilder.ToString();
		}
	}
}
