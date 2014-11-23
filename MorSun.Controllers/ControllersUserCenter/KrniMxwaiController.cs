﻿using System;
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
        public string S(bmSellKaMe t, string returnUrl)
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