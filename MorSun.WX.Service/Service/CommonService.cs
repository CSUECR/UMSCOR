using System;
using System.Linq;
using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;

using Senparc.Weixin.MP.Helpers;
using MorSun.Bll;
using MorSun.Model;

namespace MorSun.WX.ZYB.Service
{
    public class CommonService
    { 
        /// <summary>
        /// 通过id获取其itemValue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetReferenceValue(Guid? guid)
        {
            var resultValue = string.Empty;
            if (guid != null && guid != Guid.Empty)
            {
                var referenceModel = new BaseBll<wmfReference>().GetModel(guid);
                if (referenceModel != null)
                {
                    resultValue = referenceModel.ItemValue;
                }
            }
            return resultValue;
        }
    }
}