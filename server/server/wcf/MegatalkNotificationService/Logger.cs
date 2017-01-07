using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace MegatalkNotificationService
{
    public class Logger
    {
        public static void LogException(Exception exception)
        {
            try
            {        
                using (var writer = File.AppendText(ConfigurationManager.AppSettings["LogPath"]))
                {
                    writer.WriteLine(" {0} - {1} - {2} - {3}", DateTime.Now, exception.Message, exception.Source, new StackTrace(exception, true).GetFrame(0).GetFileLineNumber());
                }
            }
            catch 
            {
                // to do something
            }
        }

        public static void LogMessage(string text)
        {
            try
            {
                using (var writer = File.AppendText(ConfigurationManager.AppSettings["LogPath"]))
                {
                    writer.WriteLine(" {0} - {1}", DateTime.Now, text);
                }
            }
            catch
            {
                // to do something
            }
        }

    }
}
