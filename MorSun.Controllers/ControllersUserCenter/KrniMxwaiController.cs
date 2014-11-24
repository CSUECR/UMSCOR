using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MorSun.Bll;
using MorSun.Model;
using MorSun.Controllers.Filter;
using System.Data.Objects;
using MorSun.Common;
using System.Text;
using HOHO18.Common;
using MorSun.Controllers.ViewModel;
using System.Collections;
using MorSun.Common.Privelege;
using MorSun.Common.类别;
using HOHO18.Common.WEB;
using HOHO18.Common.SSO;
using MorSun.Common.配置;

namespace MorSun.Controllers.SystemController
{
    /// <summary>
    /// 操作
    /// </summary>
    [HandleError]
    public class KrniMxwaiController : BaseController<bmSellKaMe>
    {
        protected override string ResourceId
        {
            get { return 资源.卡密; }
        }

        /// <summary>
        /// 已售卡密保存
        /// </summary>
        /// <param name="t"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]        
        public string S(bmSellKaMe t)
        {            
            var oper = "添加失败";
            LogHelper.Write(t.OrderNum + "|" + t.KaMe + "|" + t.Buyer + "|" + t.GoodsName + "|" + t.GoodsNum, LogHelper.LogMessageType.Info);
            if (ModelState.IsValid)
            {
                LogHelper.Write("卡密验证成功", LogHelper.LogMessageType.Info);
                t.ID = Guid.NewGuid();                
                t.RegTime = DateTime.Now;
                t.ModTime = DateTime.Now;
                t.FlagTrashed = false;
                t.FlagDeleted = false;
                t.Recharge = Guid.Parse(Reference.卡密充值_未充值);                
                Bll.Insert(t);
                return "添加成功";
            }
            else
            {                
                return oper;
            }            
        }

        /// <summary>
        /// 卡密退款
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tok"></param>
        /// <returns></returns>
        [HttpGet]    
        public string TK(string id, string tok)
        {
            var oper = CFG.卡密退款_退款操作失败;
            var ts = "";
            var ind = 0;

            try
            {
                //判断是否是正常渠道访问
                ts = SecurityHelper.Decrypt(tok);
                //取时间戳
                ind = ts.IndexOf(';');
                DateTime dt = DateTime.Parse(ts.Substring(0, ind));
                if(dt.AddMinutes(2) < DateTime.Now || !ts.Contains(CFG.邦马网_对接统一码))
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }

            if (String.IsNullOrEmpty(id))
                oper = CFG.卡密退款_请录入卡密;
            LogHelper.Write("退款卡密：" + id.Substring(0,30), LogHelper.LogMessageType.Info);

            var rcKaMe = new BaseBll<bmRecharge>().All.Where(p => p.KaMe == id).FirstOrDefault();
            if (rcKaMe != null)
            {
                return CFG.卡密检测结果_已充值;
            }

            var rc = Guid.Parse(Reference.卡密充值_已退款);
            var kame = Bll.All.FirstOrDefault(p => p.KaMe == id && p.Recharge == rc);
            if (kame != null)
                return CFG.卡密退款_该卡密已退款;
                            
            LogHelper.Write("退款时间：" + ts.Substring(0, ind), LogHelper.LogMessageType.Info);

            var model = new bmSellKaMe();
            model.ID = Guid.NewGuid();
            model.KaMe = id;
            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
            model.Recharge = rc;
            Bll.Insert(model);
            LogHelper.Write("退款成功", LogHelper.LogMessageType.Info);
            return CFG.卡密退款_卡密退款操作成功;            
             
        }

        /// <summary>
        /// 不让查询
        /// </summary>
        /// <returns></returns>
        public override ActionResult I()
        {
            return RedirectToAction("I", "H");
        }

        public override ActionResult Sort(string returnUrl)
        {
            return RedirectToAction("I", "H");
        }
        
        //编辑前验证
        protected override string OnEditCK(bmSellKaMe t)
        {            
            return "";
        }

        //创建前验证
        protected override string OnAddCK(bmSellKaMe t)
        {            
            return "true";
        }
        

        //删除前验证
        protected override string OnDelCk(bmSellKaMe t)
        {            
            return "";
        }        

    }
}
