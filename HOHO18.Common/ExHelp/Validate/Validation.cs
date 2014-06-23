using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using HOHO18.Common.Helper;


namespace HOHO18.Common
{
    /// <summary>
    /// XML规则配置类--该类是工程树的根节点,所有的规则对象都隶属于它.
    /// </summary>
    [XmlRoot("Validation")]
    public partial class Validation : ProjectBase
    {
        #region Model
        /// <summary>
        /// 表
        /// </summary>
        private List<Biao> _biao_List = new List<Biao>();

        [XmlElement("Biao"), Browsable(false)]
        public List<Biao> Biao_List
        {
            get { return _biao_List; }
            set { _biao_List = value; }
        }
        #endregion Model
    }
}
