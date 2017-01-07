using System;
using System.Text;

namespace XChat.Helpers
{
    public class StackTraceUtility
    {
        // Methods
        public static string GetStackTraceAsHtml(Exception ex)
        {
            StringBuilder builder = new StringBuilder(0xfa0);
            for (Exception exception = ex; exception != null; exception = exception.InnerException)
            {
                string str = exception.StackTrace.Replace(" ", "&nbsp;").Replace("&nbsp;&nbsp;&nbsp;at&nbsp;", "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;at&nbsp;");
                StringBuilder builder2 = new StringBuilder(str);
                int index = str.IndexOf("<br />");
                builder2.Remove(index, "<br />".Length);
                str = builder2.ToString();
                builder.AppendFormat("<table cellpadding='0' cellspacing='0' border='0' class='Font-Simple'>\r\n            <tr><td class='{0}'>\r\n            <nobr>[{1}&nbsp;:&nbsp;{2}]</nobr>\r\n            </td></tr>\r\n            <tr><td class='{0}' nowrap>{3}</td></tr>\r\n          </table>", new object[] { "", exception.GetType(), exception.Message, str });
            }
            return builder.ToString();
        }

        public static string GetStackTraceAsPlainText(Exception ex)
        {
            StringBuilder builder = new StringBuilder(0xfa0);
            for (Exception exception = ex; exception != null; exception = exception.InnerException)
            {
                builder.AppendFormat("[{0} : {1}]\r\n{2}\r\n", exception.GetType(), exception.Message, exception.StackTrace);
            }
            return builder.ToString();
        }
    }


}