using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using HOHO18.Common.Helper;

namespace HOHO18.Common
{
    //[Bind(Include = "ParentId,Category,Virtual,DeptName,Domain,Description,Sort,Province,City,Town,Area,Address,Tel,WYDTLon,WYDTLat,WYDTZoom,WYDTTitle,WYDTContent,WYDTImage,WYDTImgWide,WYDTImgHigh,WYDTImgTopLeftHorizontal,WYDTImgTopLeftVertical")]

    public partial class Biao : ProjectBase
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

        private List<Field> _field_List = new List<Field>();
        /// <summary>
        /// 键列表
        /// </summary>
        [XmlElement("Field"), Browsable(false)]
        public List<Field> Field_List
        {
            get { return _field_List; }
            set { _field_List = value; }
        }

        /// <summary>
        /// 页面内容
        /// </summary>
        private List<Pages> _pages_List = new List<Pages>();
        [XmlElement("Pages"), Browsable(false)]
        public List<Pages> Pages_List
        {
            get { return _pages_List; }
            set { _pages_List = value; }
        }
    }
}
