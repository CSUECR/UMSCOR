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
        public string UJS(string Tok,bool? Auto, List<Guid> UIds)
        {
            var rz = false;
            rz = IsRZ(Tok, rz);
            if (!rz)
                return "";
            var newAuList = new List<aspnet_UsersJson>();   
            var _auList = new BaseBll<aspnet_Users>().All.Skip(0);
            if(Auto.Value)
            {
                var hours = 0 - Convert.ToDouble(CFG.邦马网_用户数据同步时间范围);
                var dt = DateTime.Now.AddHours(hours);
                _auList = _auList.Where(p => p.wmfUserInfo.RegTime > dt);
            }
            if(UIds.Count() > 0)
            {
                _auList = _auList.Where(p => UIds.Contains(p.UserId));
            }
            
            if (_auList.Count() == 0)
                return "";           

            //MembershipJson List
            var newMbList = new List<aspnet_MembershipJson>();
            //UserInfoJson List
            var newUIList = new List<wmfUserInfoJson>();


            var s = "";
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
            s += JsonConvert.SerializeObject(newAuList);



            var eys = EncodeJson(s);
            return eys;
        }        

        public string DC()
        {                        
            var id = UJS("",true,null);
            var s = DecodeJson(id);          
            var _list = JsonConvert.DeserializeObject<List<aspnet_Users>>(s);
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
