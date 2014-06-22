using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
namespace MorSun.Model
{
    /// <summary>
    /// 默认有删除的接口
    /// </summary>
    public interface IPModel:IModel
    {
        bool FlagTrashed
        {
            get;
            set;
        }
        bool FlagDeleted
        {
            get;
            set;
        }
    }
}
