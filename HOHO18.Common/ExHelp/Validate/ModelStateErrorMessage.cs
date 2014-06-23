using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOHO18.Common
{
    public class ModelStateErrorMessage
    {
        /// <summary>
        ///键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; set; }
    }
}
