using MorSun.Common.配置;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MorSun.Controllers
{    
    [HandleError]
    public class QAController : Controller
    {
        public String Q(string id)
        {
            return CFG.看答案指令 + id;
        }
        
    }
}
