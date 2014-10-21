using System;
using System.Text;
using System.Collections.Generic;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Common;
using MorSun.Bll;
using System.Linq;
using MorSun.Common.类别;

namespace MorSun.WX.ZYB.Service
{
    /// <summary>
    /// 用户马币操作类
    /// </summary>
    public class UserMaBiService
    {

        /// <summary>
        /// 实时计算的用户马币结果。先取缓存数据，然后再减去未结算的值
        /// </summary>
        /// <param name="userWeiXinId"></param>
        /// <returns></returns>
        public UserMaBiCache GetUserCurrentMaBi(string userWeiXinId)
        {
            var userMaBiCache = GetUserMaBiFromCache(userWeiXinId);
            if (userMaBiCache == null || userMaBiCache.UserId == null)
                return null;
            //缓存的马币值，减去已扣掉的马币值
            var rbll = new BaseBll<bmUserMaBiRecord>();
            var nonSettleMBR = rbll.All.Where(p => p.IsSettle == false && p.UserId == userMaBiCache.UserId);
            //币种
            var mabi = Guid.Parse(Reference.马币类别_马币);
            var bbi = Guid.Parse(Reference.马币类别_邦币);
            var banbi = Guid.Parse(Reference.马币类别_绑币);
            //加马币
            var mabisum = nonSettleMBR.Where(p => p.MaBiRef == mabi).Sum(p => p.MaBiNum);
            //加邦币
            var bbisum = nonSettleMBR.Where(p => p.MaBiRef == bbi).Sum(p => p.MaBiNum);
            //加绑币
            var banbisum = nonSettleMBR.Where(p => p.MaBiRef == banbi).Sum(p => p.MaBiNum);
            if (mabisum != null && mabisum > 0)
                userMaBiCache.UMB.mabi += mabisum == null ? 0 : mabisum.Value;
            if (bbisum != null && bbisum > 0)
                userMaBiCache.UMB.bbi += bbisum == null ? 0 : bbisum.Value;
            if (banbisum != null && banbisum > 0)
                userMaBiCache.UMB.banbi += banbisum == null ? 0 : banbisum.Value;
            return userMaBiCache;
        }

        /// <summary>
        /// 从缓存中获取用户的马币值
        /// </summary>
        /// <param name="userWeiXinId"></param>
        /// <returns></returns>
        public UserMaBiCache GetUserMaBiFromCache(string userWeiXinId)
        {
            //该微信用户是否绑定邦马网账号，未绑定则直接返回空
            var uwx = GetUserByWeiXinId(userWeiXinId);
            if (uwx == null)
                return null;
            //取缓存马币数据
            var cacheWeiXinId = "mb" + userWeiXinId;
            var userMaBiCache = MaBiCache.GetUserMaBiCache(cacheWeiXinId);
            if(userMaBiCache.UMB == null)
            {
                userMaBiCache.UserId = uwx.UserId.Value;
                //未设置缓存的情况
                userMaBiCache.UMB = GetUserMaBiByUId(uwx.UserId.Value);
                //手动设置缓存
                MaBiCache.SetUserMaBiCache(cacheWeiXinId, userMaBiCache);
            }
            return userMaBiCache;
        }        

        /// <summary>
        /// 根据用户ID取各种马币值
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private UserMaBi GetUserMaBiByUId(Guid userId)
        {
            var umb = new BaseBll<bmUserMaBi>().All.Where(p => p.UserId == userId);
            //币种
            var mabi = Guid.Parse(Reference.马币类别_马币);
            var bbi = Guid.Parse(Reference.马币类别_邦币);
            var banbi = Guid.Parse(Reference.马币类别_绑币);

            var mabiO = umb.FirstOrDefault(p => p.MaBiRef == mabi);
            var bbiO = umb.FirstOrDefault(p => p.MaBiRef == bbi);
            var banbiO = umb.FirstOrDefault(p => p.MaBiRef == banbi);
            var userMaBi = new UserMaBi();
            userMaBi.mabi = mabiO == null ? 0 : mabiO.MaBiNum.Value;
            userMaBi.bbi = bbiO == null ? 0 : bbiO.MaBiNum.Value;
            userMaBi.banbi = banbiO == null ? 0 : banbiO.MaBiNum.Value;

            return userMaBi;
        }

        /// <summary>
        /// 获取绑定用户
        /// </summary>
        /// <param name="userWeiXinId"></param>
        /// <returns></returns>
        private bmUserWeixin GetUserByWeiXinId(string userWeiXinId)
        {
            if (!String.IsNullOrEmpty(userWeiXinId))
                return new BaseBll<bmUserWeixin>().All.Where(p => p.WeiXinId == userWeiXinId).FirstOrDefault();
            else
                return null;
        }
    }
}