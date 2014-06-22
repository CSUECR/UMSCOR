using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    /// <summary>
    /// 为SelectListItem扩充可以有父节点的下拉菜单
    /// </summary>
    public class SelectListItemParent : SelectListItem
    {
        public SelectListItemParent() { }
        public string ParentValue { get; set; }
        public string TreeText { get; set; }
        public int Depth { get; set; }
    }
}
