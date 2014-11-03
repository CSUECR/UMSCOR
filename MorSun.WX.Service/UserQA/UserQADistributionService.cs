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
        /// <summary>
        /// 取题目应该分配的答题用户
        /// </summary>
        /// <param name="certification"></param>
        /// <returns></returns>
        public bmOnlineQAUser GetQADistribution(string certification)
        {
            //目前先取两种，一种是认证答题用户，一种是未认证答题用户
            var onlineuserCache = UserQAService.GetOlineQAUserCache();
            if (onlineuserCache == null)
                return null;
            switch(certification)
            {
                case Reference.认证类别_认证邦主:
                    return GetOnlineUser(onlineuserCache.CertificationUser, certification);
                case Reference.认证类别_未认证:
                    return GetOnlineUser(onlineuserCache.NonCertificationQAUser, certification);
                default:
                    return GetOnlineUser(onlineuserCache.NonCertificationQAUser, certification);
            }            
        }

        /// <summary>
        /// 对在线答题用户按题目数量顺序与活跃次数逆序排序
        /// </summary>
        /// <param name="onlineUsers"></param>
        /// <param name="certification"></param>
        /// <returns></returns>
        private static bmOnlineQAUser GetOnlineUser(IQueryable<bmOnlineQAUser> onlineUsers, string certification)
        {
            if (onlineUsers.Count() > 0)
            {//在线用户数量大于0
                var onlineWeiXinIds = onlineUsers.Select(p => p.WeiXinId);
                var djdRef = Guid.Parse(Reference.分配答题操作_待解答);
                                
                var onlineUD = new BaseBll<bmQADistribution>().All.Where(p => onlineWeiXinIds.Contains(p.WeiXinId) && p.Result == djdRef);            
                if(certification.Eql(Reference.认证类别_认证邦主))
                {
                    onlineUD = onlineUD.Where(p => p.bmQA.MaBiNum > 0);
                }
                else if(certification.Eql(Reference.认证类别_未认证))
                {
                    onlineUD = onlineUD.Where(p => p.bmQA.MaBiNum <= 0);  
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