using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Caching;
using MorSun.Model;
using MorSun.Bll;
using System.Linq;
using MorSun.Common.类别;
using MorSun.Common.常量集;
using MorSun.Common.配置;

namespace MorSun.WX.ZYB.Service
{
    /// <summary>
    /// 获取答题应该分配的用户
    /// </summary>
    public class UserQADistributionService
    {
        /// <summary>
        /// 取题目应该分配的答题用户
        /// </summary>
        /// <param name="certification"></param>
        /// <returns></returns>
        public bmOnlineQAUser GetQADistribution(Guid certification)
        {
            //目前先取两种，一种是认证答题用户，一种是未认证答题用户
            var onlineuserCache = UserQAService.GetOlineQAUserCache();
            if (onlineuserCache == null)
                return null;
            //这边要过滤掉不活跃的在线用户,超过5分钟不活跃的，则不分配题目给他
            var mn = 0 - Convert.ToInt32(CFG.疑似退出时间);
            var dt = DateTime.Now.AddMinutes(mn);

            //用户待答题保有量
            var qaWaitCount = Convert.ToInt32(CFG.用户待答题保有量);
            if(ConstList.DTCertificationLevel.Contains(certification))
            {
                var onlineUsers = onlineuserCache.CertificationUser.Where(p => p.ActiveTime >= dt);
                //在这边按用户的活跃次数多少取在线用户，活跃次数越多越有优先权
                //应该取的用户量
                if (onlineuserCache.MaBiQACount != null && onlineuserCache.MaBiQACount != 0)
                {                    
                    int selectCount = onlineuserCache.MaBiQACount / qaWaitCount;
                    if (selectCount == 0)
                        selectCount = 1;
                    onlineUsers = onlineUsers.OrderByDescending(p => p.ActiveNum).Take(selectCount);
                }
                return GetOnlineUser(onlineUsers, certification);
            }
            else
            {
                var onlineUsers = onlineuserCache.NonCertificationQAUser.Where(p => p.ActiveTime >= dt);
                //在这边按用户的活跃次数多少取在线用户，活跃次数越多越有优先权
                //应该取的用户量
                if (onlineuserCache.NonMaBiQACount != null && onlineuserCache.NonMaBiQACount != 0)
                {
                    int selectCount = onlineuserCache.NonMaBiQACount / qaWaitCount;
                    if (selectCount == 0)
                        selectCount = 1;
                    onlineUsers = onlineUsers.OrderByDescending(p => p.ActiveNum).Take(selectCount);
                }
                return GetOnlineUser(onlineUsers, certification);
            }
        }

        /// <summary>
        /// 对在线答题用户按题目数量顺序与活跃次数逆序排序
        /// </summary>
        /// <param name="onlineUsers"></param>
        /// <param name="certification"></param>
        /// <returns></returns>
        private static bmOnlineQAUser GetOnlineUser(IQueryable<bmOnlineQAUser> onlineUsers, Guid certification)
        {
            if (onlineUsers.Count() > 0)
            {//在线用户数量大于0
                var onlineWeiXinIds = onlineUsers.Select(p => p.WeiXinId);
                var djdRef = Guid.Parse(Reference.分配答题操作_待解答);
                
                //在线用户的答题分配记录
                var onlineUD = new BaseBll<bmQADistribution>().All.Where(p => onlineWeiXinIds.Contains(p.WeiXinId) && p.Result == djdRef);
                if (ConstList.DTCertificationLevel.Contains(certification))
                {
                    onlineUD = onlineUD.Where(p => p.bmQA.bmUserMaBiRecords.Sum(q => q.MaBiNum) > 0);
                }                
                else
                {
                    onlineUD = onlineUD.Where(p => p.bmQA.bmUserMaBiRecords.Sum(q => q.MaBiNum) <= 0);  
                }

                var qaDis = onlineUD.GroupBy(p => p.WeiXinId)
                    .Select(p => new bmQADistribution()
                    {
                        WeiXinId = p.Key,
                        DJDCount = p.Key.Count()
                    });

                foreach (var olu in onlineUsers)
                {
                    var oud = qaDis.Where(p => p.WeiXinId == olu.WeiXinId).FirstOrDefault();
                    olu.DJDCount = oud == null ? 0 : oud.DJDCount;
                }
                return onlineUsers.OrderBy(p => p.DJDCount).ThenByDescending(p => p.ActiveNum).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}