using System;
using log4net;
using log4net.Config;

namespace XChat.Helpers
{
    public class Log
    {
        // Fields
        private static ILog log = LogManager.GetLogger(typeof(Log));
        private const string PARAM_TEMPLATE = "param{0}";

        // Methods
        static Log()
        {
            XmlConfigurator.Configure();
        }

        private Log()
        {
        }

        public static void Debug(string s, params string[] par)
        {
            SetThreadContextProperties(par);
            log.Debug(s);
        }

        public static void Error(string s, params string[] par)
        {
            SetThreadContextProperties(par);
            log.Error(s);
        }

        public static void Error(Exception ex, string s, params string[] par)
        {
            SetThreadContextProperties(par);
            log.Error(s, ex);
        }

        public static void Error(Exception ex)
        {
            string errorUid = Guid.NewGuid().ToString().ToUpper();
            string errorText =
                string.Format(
                    @"ErrorUid: {0}
                                             {1}
                                             ",
                    errorUid, StackTraceUtility.GetStackTraceAsPlainText(ex));
            Log.Error(errorText);
        }

        public static void Info(string s, params string[] par)
        {
            SetThreadContextProperties(par);
            log.Info(s);
        }

        private static void SetThreadContextProperties(string[] par)
        {
            int num = 0;
            foreach (string str in par)
            {
                int num3 = ++num;
                ThreadContext.Properties[string.Format("param{0}", num3.ToString())] = str;
            }
        }

        public static void Warn(string s, params string[] par)
        {
            SetThreadContextProperties(par);
            log.Warn(s);
        }
    }

}