using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MorSun.Model
{
    public class ChildNode
    {
        public Guid MenuID { get; set; }
        public string Icon { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
    }

    public class Node
    {
        [XmlAttribute]
        public Guid ID { get; set; }
        [XmlAttribute]
        public string Basic { get; set; }
        public Guid MenuID { get; set; }
        public string Icon { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        [XmlArray("Menus")]
        public List<ChildNode> Menus { get; set; }
    }
    /// <summary>
    /// 序列化菜单的模型
    /// </summary>
    public class Menu
    {
        [XmlAttribute]
        public string Basic { get; set; }
        [XmlAttribute]
        public string BasicName { get; set; }
        [XmlAttribute]
        public string BasicIcon { get; set; }
        [XmlElement("Node")]
        public List<Node> NodeList { get; set; }
    }
}
