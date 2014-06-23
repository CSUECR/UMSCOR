using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOHO18.Common
{
    public class ReturnMsg
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 追加返回的数据
        /// </summary>
        public object AppendData { get; set; }
        public string ReturnUrl { get; set; }
    }
}
