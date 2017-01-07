using System;
using log4net;

namespace Application.Infrastructure.Logging
{
	public class LoggerWrapper
	{
		public static ILog Create(Type type)
		{
			log4net.Config.XmlConfigurator.Configure();
			return LogManager.GetLogger(type);
		}
	}
}
