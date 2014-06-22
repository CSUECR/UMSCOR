using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
namespace MorSun.Model
{
    /// <summary>
    /// 默认有省市的接口
    /// </summary>
    public interface IPCModel:IModel
    {
        string formProvince
        {
            get;
            set;
        }

        string formCity
        {
            get;
            set;
        }

        string formTown
        {
            get;
            set;
        }
        wmfProvince wmfProvince
        {
            get;
            set;
        }
        wmfCity wmfCity
        {
            get;
            set;
        }
        wmfCounty wmfCounty
        {
            get;
            set;
        }
    }
}
