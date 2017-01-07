using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Application.Core.UI.HtmlHelpers
{
	public static class LocalizationHelper
	{
		public static IHtmlString MetaAcceptLanguage(this HtmlHelper html)
		{
			var acceptLang = HttpUtility.HtmlAttributeEncode(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
			return new HtmlString(string.Format("<meta name=\"accept-language\" content=\"{0}\"/>", acceptLang));
		}
	}
}
