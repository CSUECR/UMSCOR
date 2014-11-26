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
using Newtonsoft.Json;


namespace MorSun.Controllers.SystemController
{
    /// <summary>
    /// 操作
    /// </summary>
    [HandleError]
    public class DivniTxiuenController : BaseController<bmSellKaMe>
    {
        protected override string ResourceId
        {
            get { return 资源.系统参数配置; }
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
            var newAuList = new List<aspnet_UsersJson>();
            var _auList = new BaseBll<aspnet_Users>().All;
            foreach(var u in _auList)
            {
                var t = new aspnet_UsersJson
                {
                    ApplicationId = u.ApplicationId
                    ,
                    UserId = u.UserId
                    ,
                    UserName = u.UserName
                    ,
                    LoweredUserName = u.LoweredUserName
                    ,
                    MobileAlias = u.MobileAlias
                    ,
                    IsAnonymous = u.IsAnonymous
                    ,
                    LastActivityDate = u.LastActivityDate
                };
                newAuList.Add(t);
            }
            //var us = Json(_auList, JsonRequestBehavior.AllowGet);
            return Url.Encode(JsonConvert.SerializeObject(newAuList));
        }

        public string dc(string id)
        {
            //var jsonu = @"[{"ApplicationId":"4533934f-60ba-4f9f-ae8f-5fb1ed167e58","UserId":"4087ca79-826b-4507-82e7-479a20d3b53e","UserName":"youhong@bungma.com","LoweredUserName":"youhong@bungma.com","MobileAlias":null,"IsAnonymous":false,"LastActivityDate":"2014-11-24T23:51:23.263"},{"ApplicationId":"4533934f-60ba-4f9f-ae8f-5fb1ed167e58","UserId":"2826ce77-44f5-437c-abc5-b8ce8f8e6bdb","UserName":"youhong@hoho18.net","LoweredUserName":"youhong@hoho18.net","MobileAlias":null,"IsAnonymous":false,"LastActivityDate":"2014-11-10T05:38:10"},{"ApplicationId":"4533934f-60ba-4f9f-ae8f-5fb1ed167e58","UserId":"12fb4bf9-28ab-4a61-9ad7-cd03974688cc","UserName":"zsfyou@qq.com","LoweredUserName":"zsfyou@qq.com","MobileAlias":null,"IsAnonymous":false,"LastActivityDate":"2014-11-21T18:06:42.487"}]"
            //http://www.cnblogs.com/jams742003/archive/2009/12/25/1631829.html
            var _list = JsonConvert.DeserializeObject<List<aspnet_Users>>(id);
            return "";
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
