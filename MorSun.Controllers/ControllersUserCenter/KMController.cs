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

namespace MorSun.Controllers.SystemController
{
    /// <summary>
    /// 操作
    /// </summary>
    [HandleError]
    public class KMController : BaseController<bmSellKaMe>
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
        public ActionResult S(bmSellKaMe t, string returnUrl)
        {            
            var oper = new OperationResult(OperationResultType.Error, "添加失败");
            
            if (ModelState.IsValid)
            {
                CreateInitObject(t);
                t.ID = Guid.NewGuid();
                t.Recharge = Guid.Parse(Reference.卡密充值_未充值);
                fillOperationResult(returnUrl, oper, "添加成功");
                Bll.Insert(t);
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }            
        }

        /// <summary>
        /// 提取卡密
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult T(string returnUrl)
        {
            var model = new TKM();
            return View(model);
        }

        /// <summary>
        /// 查询要提取的卡密
        /// </summary>
        /// <param name="t"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult T(TKM t, string returnUrl)
        {
            var oper = new OperationResult(OperationResultType.Error, "提取失败");
            if (ModelState.IsValid)
            {   
                fillOperationResult(returnUrl, oper, "添加成功");
                var model = Bll.All.FirstOrDefault(p => p.OrderNum == t.OrderNum && p.Buyer == t.OrderNum);
                if(model == null)
                {
                    oper.AppendData = ModelState.GE();
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    t.KaMe = model.KaMe;
                    return View(t);
                }                
            }
            else
            {
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }         
        }

        //编辑前验证
        protected override string OnEditCK(wmfOperation t)
        {            
            return "";
        }

        //创建前验证
        protected override string OnAddCK(wmfOperation t)
        {            
            return "true";
        }
        

        //删除前验证
        protected override string OnDelCk(wmfOperation t)
        {            
            return "";
        }        

    }
}
