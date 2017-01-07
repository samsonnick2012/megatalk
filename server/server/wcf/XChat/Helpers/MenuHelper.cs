using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using XChat.Helpers.CommonObjects;

namespace XChat.Helpers
{
    public class MenuHelper
    {
        public static MenuItem GetMemuItembyRole(int roleid)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu.xml");

            MenuItem menuitem;
            var serializer = new XmlSerializer(typeof(MenuItem));

            using (var reader = new StreamReader(path))
            {
                menuitem = (MenuItem)serializer.Deserialize(reader);
            }

            MenuItem _menuitem = new MenuItem { Items = new List<MenuItem>() };
            if (roleid == 1)
                return menuitem;

            _menuitem.Items.Add(menuitem.Items.FirstOrDefault(x => x.Role == roleid));

            return _menuitem;
        }
    }
}