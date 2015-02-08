using HOHO18.Common.WEB;
using MorSun.Bll;
using MorSun.Common.类别;
using MorSun.Common.配置;
using MorSun.Controllers.ViewModel;
using MorSun.Model;
using MorSun.WX.ZYB.Service;
using Quartz;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HOHO18.Common;
using MorSun.Common.常量集;
using System.Web.Caching;
using System.Web;

namespace MorSun.Controllers.Quartz
{
    public class CheckingJob4:IJob
    {
        public static OnlineQAUserCache GenerateQAUserCache()
        {
            var bll = new BaseBll<bmOnlineQAUser>();
            var qadisbll = new BaseBll<bmQADistribution>();
            var qabll = new BaseBll<bmQA>();
            var bmumbBll = new BaseBll<bmUserMaBiRecord>();
            var uwbll = new BaseBll<bmUserWeixin>();
            var numbbll = new BaseBll<bmNewUserMB>();

            #region 刚提的问题生成马币消费记录与分配记录
            //取出所有未分配记录的问题      "提问都不一定绑定用户"
            var qaRef = Guid.Parse(Reference.问答类别_问题);
            var curWeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
            var nonmbQA = qabll.All.Where(p => p.QARef == qaRef && p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.bmQADistributions.Count() == 0);    //必须是提问才分配,当前应用是微信APP。

            //取出所有未生成马币记录的提问用户
            var nonmbUid = nonmbQA.Select(p => p.WeiXinId).Distinct();
            LogHelper.Write("新提问的用户数" + nonmbUid.Count().ToString(), LogHelper.LogMessageType.Debug);
            //区分出已绑定与未绑定的用户ID            
            //绑定的用户ID
            var uwU = uwbll.All.Where(p => nonmbUid.Contains(p.WeiXinId) && p.WeiXinAPP == curWeiXinAPP);
            //绑定的用户微信ID
            var uwUid = uwU.Select(p => p.WeiXinId);

            //未绑定的用户ID
            var nonuwUID = nonmbUid.Where(p => !uwUid.Contains(p));

            //已绑定的用户取邦马币值，邦马币值大于0的取出来
            var uwUSid = uwU.Select(p => p.UserId);
            var defXFMB = Convert.ToDecimal(CFG.提问默认收费马币值);
            var UserBMB = numbbll.All.Where(p => uwUSid.Contains(p.UserId) && (p.NMB > defXFMB || p.NBB > defXFMB));
            LogHelper.Write("花邦马币提问的用户数" + UserBMB.Count().ToString(), LogHelper.LogMessageType.Debug);
            //生成马币记录
            //先生成收费的马币记录，收费问题分配给默认收费答题用户。未生成收费的马币记录，直接分配给默认免费答题用户
            var mbQAIds = new List<Guid>();
            var tempMB = Convert.ToDecimal(0);
            var tempBB = Convert.ToDecimal(0);
            var tempQACount = 0;
            foreach (var u in UserBMB)
            {
                tempMB = u.NMB.Value;
                tempBB = u.NBB.Value;
                //能取出的当前用户问题数
                tempQACount = Convert.ToInt32(((tempMB + tempBB) / defXFMB));
                var uwxid = uwU.FirstOrDefault(p => p.UserId == u.UserId).WeiXinId;
                //当前用户提问数
                var uqa = nonmbQA.Where(p => p.WeiXinId == uwxid).Take(tempQACount);
                foreach (var q in uqa)
                {
                    //有消费的问题记录做标记
                    mbQAIds.Add(q.ID);
                    var umbrModel = new bmUserMaBiRecord();
                    umbrModel.SourceRef = Guid.Parse(Reference.马币来源_消费);
                    if (tempBB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.马币类别_邦币);
                        tempBB -= defXFMB;
                    }
                    else if (tempMB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.马币类别_马币);
                        tempMB -= defXFMB;
                    }
                    umbrModel.MaBiNum = 0 - defXFMB;
                    umbrModel.QAId = q.ID;

                    umbrModel.IsSettle = false;
                    umbrModel.RegTime = DateTime.Now;
                    umbrModel.ModTime = DateTime.Now;
                    umbrModel.FlagTrashed = false;
                    umbrModel.FlagDeleted = false;

                    umbrModel.ID = Guid.NewGuid();
                    umbrModel.UserId = u.UserId;
                    umbrModel.RegUser = u.UserId;

                    bmumbBll.Insert(umbrModel, false);
                }
            }
            LogHelper.Write("花邦马币提问的问题数" + mbQAIds.Count().ToString(), LogHelper.LogMessageType.Debug);
            //生成问题分配记录，收费的到收费的默认账号，免费的到免费的默认账号
            foreach (var q in nonmbQA)
            {
                //问题分配处理                
                var qaModel = new bmQADistribution();

                qaModel.ID = Guid.NewGuid();
                qaModel.QAId = q.ID;
                qaModel.DistributionTime = DateTime.Now;

                qaModel.RegTime = DateTime.Now;
                qaModel.ModTime = DateTime.Now;
                qaModel.FlagTrashed = false;
                qaModel.FlagDeleted = false;

                qaModel.Result = Guid.Parse(Reference.分配答题操作_待解答);
                if (mbQAIds.Contains(q.ID))
                {
                    qaModel.WeiXinId = CFG.默认收费问题微信号;
                }
                else
                {
                    qaModel.WeiXinId = CFG.默认免费问题微信号;
                }
                qadisbll.Insert(qaModel, false);
            }
            //bmumbBll.UpdateChanges();
            //qadisbll.UpdateChanges();

            #endregion

            var qastate = Guid.Parse(Reference.分配答题操作_待解答);
            #region 未解决的问题重新分配
            //默认用户未解决的问题修改为待解答，以超过配置的时间为准
            var acQADmn = 0 - Convert.ToInt32(CFG.未处理问题激活时间);
            var acQADdt = DateTime.Now.AddMinutes(acQADmn);
            var nonHandleRef = Guid.Parse(Reference.分配答题操作_未处理);
            //增加微信APP的判断
            var nonACQAD = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.ModTime < acQADdt && ConstList.DefaultDISUser.Contains(p.WeiXinId) && p.Result == nonHandleRef);
            LogHelper.Write((acQADdt.ToShortTimeString() + "手动更新用户缓存时未处理的问题数量" + nonACQAD.Count().ToString()), LogHelper.LogMessageType.Debug);
            foreach (var item in nonACQAD)
            {
                item.Result = qastate;
                item.ModTime = DateTime.Now;
            }
            #endregion
            //测试看这边不更新数据库，以下代码能执行不。
            //未解决的问题修改为待解答 结束            
            var todayST = DateTime.Now.AddHours(-24);
            //取所有的待解答的问题数量
            var djdqadis = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.Result == qastate);
            LogHelper.Write("待解答的总问题数" + djdqadis.Count().ToString(), LogHelper.LogMessageType.Debug);
            ///////////////////////取数量的方式要修改，提问的方式变了，不直接生成马币//////////////////////////
            var mabiqaCount = djdqadis.Where(p => p.bmQA.bmUserMaBiRecords.Sum(m => m.MaBiNum) < 0).Count();//提问是负数
            LogHelper.Write("待解答的收费问题数" + mabiqaCount.ToString(), LogHelper.LogMessageType.Debug);
            //免费的只取24小时内的提问记录，节省服务器资源 看到未解决的问题，不必奇怪，超过24小时了
            var nonmabiqaCount = djdqadis.Where(p => p.bmQA.bmUserMaBiRecords.Count() == 0 && p.bmQA.RegTime >= todayST).Count();
            LogHelper.Write("待解答的免费问题数" + nonmabiqaCount.ToString(), LogHelper.LogMessageType.Debug);

            var state = Guid.Parse(Reference.在线状态_在线);
            //用户待答题保有量
            var qaWaitCount = Convert.ToInt32(CFG.用户待答题保有量);

            //这边要过滤掉不活跃的在线用户,超过5分钟不活跃的，则不分配题目给他
            var nondismn = 0 - Convert.ToInt32(CFG.疑似退出时间);
            var nondisdt = DateTime.Now.AddMinutes(nondismn);

            //需要强制退出的配置
            var logoutMN = 0 - Convert.ToInt32(CFG.强制退出时间);
            var logoutdt = DateTime.Now.AddMinutes(logoutMN);

            var cu = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));
            var noncu = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));

            #region 重新分配开始

            //取要强退的认证用户 '超过7分钟未活跃'
            var noActiveCU = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && p.ActiveTime < logoutdt && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));
            LogHelper.Write("需要强退的认证用户数" + noActiveCU.Count().ToString(), LogHelper.LogMessageType.Debug);
            //取要强退未认证用户
            var noActiveU = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && p.ActiveTime < logoutdt && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));
            LogHelper.Write("需要强退的未认证用户数" + noActiveU.Count().ToString(), LogHelper.LogMessageType.Debug);

            //取强退用户ID
            //存在不活跃的认证用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
            var noActiveCUWIDS = noActiveCU.Select(p => p.WeiXinId);
            //存在不活跃的认证用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
            var noActiveUWIDS = noActiveU.Select(p => p.WeiXinId);

            //取所有不活跃的答题用户，然后再区分出收费问题与免费问题
            var noActiveUCUQAD = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.Result == qastate && (noActiveCUWIDS.Contains(p.WeiXinId) || p.WeiXinId == CFG.默认收费问题微信号 || noActiveUWIDS.Contains(p.WeiXinId) || p.WeiXinId == CFG.默认免费问题微信号));
            LogHelper.Write("总的未答题数" + noActiveUCUQAD.Count().ToString(), LogHelper.LogMessageType.Debug);
            var noActiveMQAD = noActiveUCUQAD.Where(p => p.bmQA.bmUserMaBiRecords.Sum(m => m.MaBiNum) < 0).OrderBy(p => p.bmQA.RegTime);
            LogHelper.Write("收费的未答题数" + noActiveMQAD.Count().ToString(), LogHelper.LogMessageType.Debug);

            //收费问题分配
            //取活跃在线的认证答题用户,排除掉默认分配用户，省的多余操作
            var certificationUsers = cu.Where(p => p.WeiXinId != CFG.默认收费问题微信号 && p.ActiveTime >= nondisdt).OrderByDescending(p => p.ActiveNum);
            if (mabiqaCount > 0)
            {
                int selectCount = mabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                certificationUsers = certificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }
            LogHelper.Write("待分配答题的认证用户数" + certificationUsers.Count().ToString(), LogHelper.LogMessageType.Debug);

            if (certificationUsers.Count() > 0)
            {//存在在线用户时   
                if (noActiveMQAD.Count() > 0)
                {//待答题数据重新分配
                    var ouCount = certificationUsers.Count();
                    var i = 0;
                    foreach (var item in noActiveMQAD)
                    {
                        //对答题进行分配
                        i++;
                        var j = i % ouCount;
                        if (j == 0)
                            j = ouCount;
                        var disOU = certificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                        item.WeiXinId = disOU.WeiXinId;
                        item.ModTime = DateTime.Now;
                    }
                }
            }
            else
            {//无在线活跃用户，系统将答题回收给默认用户                 
                if (noActiveMQAD.Count() > 0)
                { //将答题回收给默认答题用户
                    foreach (var item in noActiveMQAD)
                    {
                        item.WeiXinId = CFG.默认收费问题微信号;
                        item.ModTime = DateTime.Now;
                    }
                }

            }
            //收费问题分配结束
            //免费问题分配  当天的分配给活跃未认证用户，非当天的分配给默认用户
            var noActiveNMQAD = noActiveUCUQAD.Where(p => p.bmQA.bmUserMaBiRecords.Count() == 0).OrderBy(p => p.bmQA.RegTime);
            LogHelper.Write("免费的未答题数" + noActiveNMQAD.Count().ToString(), LogHelper.LogMessageType.Debug);

            //未认证的用户处理
            var noncertificationUsers = noncu.Where(p => p.WeiXinId != CFG.默认免费问题微信号 && p.ActiveTime >= nondisdt).OrderByDescending(p => p.ActiveNum);
            if (nonmabiqaCount > 0)
            {
                int selectCount = nonmabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                noncertificationUsers = noncertificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }
            LogHelper.Write("待分配答题的未认证用户数" + noncertificationUsers.Count().ToString(), LogHelper.LogMessageType.Debug);

            if (noncertificationUsers.Count() > 0)
            {
                if (noActiveNMQAD.Count() > 0)
                {//待答题数据重新分配
                    var ouCount = noncertificationUsers.Count();
                    var i = 0;
                    foreach (var item in noActiveNMQAD)
                    {
                        //判断是否是当天的问题
                        if (item.bmQA.RegTime >= todayST)
                        {
                            //对答题进行分配
                            i++;
                            var j = i % ouCount;
                            if (j == 0)
                                j = ouCount;
                            var disOU = noncertificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                            item.WeiXinId = disOU.WeiXinId;
                            item.ModTime = DateTime.Now;
                        }
                        else
                        {//非当天的问题回收给默认微信号
                            if (item.WeiXinId != CFG.默认免费问题微信号)
                                item.WeiXinId = CFG.默认免费问题微信号;
                        }
                    }
                }
            }
            else
            {
                if (noActiveNMQAD.Count() > 0)
                { //将答题回收给默认答题用户
                    foreach (var item in noActiveNMQAD)
                    {
                        item.WeiXinId = CFG.默认免费问题微信号;
                        item.ModTime = DateTime.Now;
                    }
                }
            }
            //免费问题分配结束
            #endregion 重新分配结束

            #region 移除用户和他的答题缓存，数据库更新
            //强制退出不活跃用户  并将不活跃用户的答题缓存清空  
            var tqState = Guid.Parse(Reference.在线状态_退出);
            foreach (var item in noActiveCU)
            {
                //不修改修改时间 
                item.AQEndTime = DateTime.Now;
                item.State = tqState;
                CacheAccess.RemoveCache(CFG.用户待答题缓存键前缀 + item.WeiXinId);
            }
            foreach (var item in noActiveU)
            {
                item.AQEndTime = DateTime.Now;
                item.State = tqState;
                CacheAccess.RemoveCache(CFG.用户待答题缓存键前缀 + item.WeiXinId);
            }
            //统一更新进数据库
            qadisbll.UpdateChanges();
            bll.UpdateChanges();
            //不活跃用户处理结束
            #endregion

            //生成缓存对象
            var model = new OnlineQAUserCache();
            model.RefreshTime = DateTime.Now;

            //收费待答题数量
            model.MaBiQACount = mabiqaCount;
            //免费待答题数量
            model.NonMaBiQACount = nonmabiqaCount;
            model.CertificationUser = cu.OrderByDescending(p => p.ActiveNum);
            model.NonCertificationQAUser = noncu.OrderByDescending(p => p.ActiveNum);

            LogHelper.Write("手动更新用户缓存结束", LogHelper.LogMessageType.Debug);

            return model;
        }


        public void SaveToCacheByDependency(string cacheKey, object cacheObject, CacheDependency dependency)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Insert(cacheKey, cacheObject, dependency);
            LogHelper.Write("自动Insert缓存", LogHelper.LogMessageType.Debug);
        }

        public void SetOlineQAUserCache(OnlineQAUserCache qaCache)
        {
            LogHelper.Write("进入自动设置缓存", LogHelper.LogMessageType.Debug);
            string path = HttpRuntime.AppDomainAppPath + "XmlConfig\\";
            LogHelper.Write(path, LogHelper.LogMessageType.Debug);
            CacheDependency fileDependency = new CacheDependency(path);
            LogHelper.Write("自动设置缓存", LogHelper.LogMessageType.Debug);
            //保存到缓存中
            SaveToCacheByDependency(CFG.在线答题用户缓存键, qaCache, fileDependency);
        }

        public void Execute(IJobExecutionContext context)
        {           
            try
            { 
                //SetOlineQAUserCache(MorSun.Controllers.BasisController.GenerateQAUserCache());
                SetOlineQAUserCache(GenerateQAUserCache());
            }
            catch(Exception ex)
            {
                LogHelper.Write("答题缓存设置异常" + ex.Message, LogHelper.LogMessageType.Error);
            }

        }
    }
}
