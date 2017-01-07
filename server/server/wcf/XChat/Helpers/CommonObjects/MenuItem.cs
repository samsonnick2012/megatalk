using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace XChat.Helpers.CommonObjects
{
    [XmlRoot("MenuItem")]
    public class MenuItem
    {
        [XmlAttribute("label")]
        public string Label { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("role")]
        public int Role { get; set; }

        [XmlAttribute("defaulUrlForRole")]
        public bool DefaulUrlForRole { get; set; }

        [XmlArray("Items")]
        public List<MenuItem> Items { get; set; }
    }
}