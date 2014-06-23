using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using System.Linq;
using HOHO18.Common.Helper;

namespace HOHO18.Common
{

    public class Field : ProjectBase
    {
        private string _KeyName;
        private string _KeyValue;
        //private string _type;
        /// <summary>
        /// 键名
        /// </summary>
        [XmlAttribute("Key"), DisplayName("键名"), Description("键名")]
        public string KeyName
        {
            get { return _KeyName; }
            set { _KeyName = value; }
        }
        /// <summary>
        /// 列
        /// </summary>
        [XmlAttribute("Value"), DisplayName("键值"), Description("键值")]
        public string KeyValue
        {
            get { return _KeyValue; }
            set { _KeyValue = value; }
        }
    }
}
