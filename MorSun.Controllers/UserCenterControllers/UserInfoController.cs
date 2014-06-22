using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using MorSun.Model;

namespace MorSun.Controllers
{
    public class UserInfoController:BaseController<wmfUserInfo>
    {
        /// <summary>
        /// 获取用于自动补全autoComplete的JSON数据，有固定的格式要求
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAutocompleteList(string sName)
        {
            UserVModel vmodel = new UserVModel() { IsApproved="1"};
            var res = vmodel.List.Where(u=>u.wmfUserInfo.TrueName.Contains(sName));
            return Json((from u in res
                         select new
                         {
                             value = u.UserId
                             ,
                             label = u.wmfUserInfo.TrueName,
                         }).Take(10), JsonRequestBehavior.AllowGet);
        }
    }
}
