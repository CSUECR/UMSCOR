using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using System.Linq;
using HOHO18.Common.Helper;
using HOHO18.Common;

namespace HOHO18.Common
{

    public class Pages : ProjectBase
    {
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
    }
}
