using MorSun.Common.配置;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;

namespace MorSun.Controllers
{    
    [HandleError]
    public class QAController : Controller
    {
        public ActionResult Q(Guid? id)
        {
            var model = new BMQAViewVModel();
            if(id != null)
            { 
                model.sParentId = id;
                return View(model);
            }    
            else
            {
                return RedirectToAction("I", "H");
            }
            
        }
        
    }
}
