using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;
using MorSun.Common.类别;

namespace MorSun.Controllers.ViewModel
{
    public class BMSellKaMeVModel : BaseVModel<bmSellKaMe>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        //public virtual Guid? CheckedId { get; set; }

        public override IQueryable<bmSellKaMe> List
        {
            get
            {
                var l = All;
                if (String.IsNullOrEmpty(FlagTrashed))
                    FlagTrashed = "0";
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }                
                return l.OrderBy(p => p.RegTime);
            }
        }
    }
}
