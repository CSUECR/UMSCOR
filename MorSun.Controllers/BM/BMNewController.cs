using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Bll;
using System.Collections;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using System.Xml;
using MorSun.Common.Privelege;

namespace MorSun.Controllers.SystemController
{
    public class BMNewController : BaseController<bmNew>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.新闻; }
        }

        protected override string OnAddCK(bmNew t)
        {
              
            return "";
        }

        protected override string OnEditCK(bmNew t)
        {                       
            return "";
        }        
    }
}
