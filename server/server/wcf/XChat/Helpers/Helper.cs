using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XChat.Filters;

namespace XChat.Helpers
{
    public static class Helper
    {
        public static string LocalizeRole(string roleValue)
        {
            switch (roleValue)
            {
                case CustomAuthorizeAttribute.ROLE_ADMIN:
                    return "Администратор";
                case CustomAuthorizeAttribute.ROLE_USER:
                    return "Пользователь";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static int? GetNullableInt(string text)
        {
            if (text == null)
            {
                return null;
            }
            int value;
            if (int.TryParse(text.Trim(), out value))
            {
                return value;
            }
            return null;
        }


        public static IList<EnumItem> TranslateEnum(Type enumType, bool includeZeroValue = false)
        {
            Array values = Enum.GetValues(enumType);
            IList<EnumItem> items = new List<EnumItem>();
            for (int i = 0; i < values.Length; i++)
            {
                int value = Convert.ToInt32(values.GetValue(i));

                if ((includeZeroValue && value == 0) || value > 0)
                {
                    EnumItem item = new EnumItem();
                    item.EnumValue = value;
                    item.Name = Enum.GetName(enumType, values.GetValue(i));
                    string resourceName = enumType.Name + "_" + item.Name;
                    item.TranslatedName = Resources.Enums.ResourceManager.GetString(resourceName);
                    items.Add(item);
                }
            }

            return items;
        }

        public static EnumItem TranslateEnumItem(Type enumType, Enum enumValue, string postfix = "")
        {
            EnumItem item = new EnumItem();
            item.EnumValue = Convert.ToInt32(enumValue);
            item.Name = Enum.GetName(enumType, enumValue);
            string resourceName = enumType.Name + "_" + item.Name + postfix;
            item.TranslatedName = Resources.Enums.ResourceManager.GetString(resourceName);
            return item;
        }



        public static string ClearStringTokens(string str)
        {
            return str.Trim().Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "");
        }

        public static string BaseSiteUrl
        {
            get
            {
                HttpContext context = HttpContext.Current;
                string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
                return baseUrl;
            }
        }

        public static string BaseVirtualAppPath
        {
            get
            {
                HttpContext context = HttpContext.Current;
                string url = context.Request.ApplicationPath;
                if (url.EndsWith("/"))
                    return url;
                else
                    return url + "/";
            }
        }




    }
}