using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 将集合组合成hash键
    /// </summary>
    public class ListHashK
    {

        /// <summary>
        /// 其中的值
        /// </summary>
        public virtual object[] Keys { get; set; }

        /// <summary>
        /// 获取hash码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Keys.Sum(key => key.GetHashCode());
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //dict<listHashK,V>; dict[new ListHashK{Keys=keys}]
            var result = base.Equals(obj);

            if (!result && obj is ListHashK)
            {
                var objK = obj as ListHashK;

                if (Keys.Length == objK.Keys.Length)
                {
                    var excCount = Keys.Except(objK.Keys).Count();
                    result = excCount == Keys.Length;
                }

            }
            return result;

        }
    }
}
