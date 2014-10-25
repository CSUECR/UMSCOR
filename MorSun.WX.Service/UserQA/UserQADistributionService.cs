using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Caching;
using MorSun.Model;
using MorSun.Bll;
using System.Linq;
using MorSun.Common.类别;

namespace MorSun.WX.ZYB.Service
{
    /// <summary>
    /// 获取答题应该分配的用户
    /// </summary>
    public class UserQADistributionService
    {
        public bmOnlineQAUser GetQADistribution(bool certification)
        {
            //目前先取两种，一种是认证答题用户，一种是未认证答题用户
            var onlineuserCache = UserQAService.GetOlineQAUserCache();
            if (onlineuserCache == null)
                return null;
            if(certification)
            {
                //收费取认证用户                
                return GetOnlineUser(onlineuserCache.CertificationUser, certification);
            }
            else
            {
                //非收费取非认证用户                
                return GetOnlineUser(onlineuserCache.NonCertificationQAUser, certification);
            }
        }

        private static bmOnlineQAUser GetOnlineUser(IQueryable<bmOnlineQAUser> onlineUsers, bool certification)
        {
            if (onlineUsers.Count() > 0)
            {//在线用户数量大于0
                var onlineWeiXinIds = onlineUsers.Select(p => p.WeiXinId);
                var djdRef = Guid.Parse(Reference.分配答题操作_待解答);
                                
                var onlineUD = new BaseBll<bmQADistribution>().All.Where(p => onlineWeiXinIds.Contains(p.WeiXinId) && p.Result == djdRef);
                if(certification)
                {
                    onlineUD = onlineUD.Where(p => p.bmQA.MaBiNum > 0);  
                }
                else
                {
                    onlineUD = onlineUD.Where(p => p.bmQA.MaBiNum <= 0);                    
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