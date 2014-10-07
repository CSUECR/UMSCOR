using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Bll;
using MorSun.Model;
using System.Web.Routing;
using HOHO18.Common;
using System.Web.Security;
using MorSun.Common.类别;
using HOHO18.Common.WEB;
using MorSun.Controllers.ViewModel;


namespace MorSun.Controllers
{  
    [HandleError]
    public class NController : BasisController
    {        
        public ActionResult A()
        {
            return View(new BMNewVModel());
        }

        public ActionResult S(Guid? id,string returnUrl)
        {
            var bmnew = new bmNew();
            if(id == null)
            {
                bmnew = new BaseBll<bmNew>().All.OrderBy(p => p.Sort).FirstOrDefault();
            }
            else {
                bmnew = new BaseBll<bmNew>().GetModel(id);
            }
                
            ViewBag.ReturnUrl = returnUrl;
            return View(bmnew);
        }
    }
}
