using System;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;

namespace Application.Infrastructure.NotificationsManagement
{
	public static class MailSender
	{
		private static SmtpSection MailSetting
		{
			get
			{
				return ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
			}
		}

		public static void SendMail(string to, string subject, string body, MailAddress fromMailAddress = null)
		{
			try
			{
				if (fromMailAddress == null)
				{
					fromMailAddress = new MailAddress(MailSetting.From, MailSetting.Network.UserName, Encoding.UTF8);
				}

				var message = new MailMessage(fromMailAddress, new MailAddress(to))
				{
					Subject = subject, 
					BodyEncoding = Encoding.UTF8, 
					Body = body, 
					IsBodyHtml = false, 
					SubjectEncoding = Encoding.UTF8
				};
				var client = new SmtpClient(MailSetting.Network.Host, MailSetting.Network.Port)
				{
					EnableSsl = MailSetting.Network.EnableSsl, 
					Credentials = new NetworkCredential(MailSetting.From, MailSetting.Network.Password), 
					DeliveryMethod = SmtpDeliveryMethod.Network
				};

				client.Send(message);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Ошибка", e);
			}
		}
	}
}