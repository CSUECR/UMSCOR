using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using MorSun.Model;

namespace MorSun.Controllers
{    
    [HandleError]
    public class CSController : Controller
    {
        public ActionResult C()
        {
            var indexModel = new IndexModel();
            return View(indexModel);
        }
        
    }
}
