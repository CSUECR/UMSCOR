using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;

namespace MorSun.Controllers.ViewModel
{
    public class BMNewVModel : BaseVModel<bmNew>
    {
        public override IQueryable<bmNew> List
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
                if (!String.IsNullOrEmpty(sNewTitle))
                {
                    l = l.Where(p => p.NewTitle.Contains(sNewTitle));
                }
                if (sNewRef != null && sNewRef != Guid.Empty)
                {
                    l = l.Where(p => p.NewRef == sNewRef);
                }
                if(!String.IsNullOrEmpty(sNewKeyWord))
                {
                    l = l.Where(p => p.NewKeyWord.Contains(sNewKeyWord));
                }
                return l.OrderBy(p => p.Sort).ThenBy(p => p.RegTime);
            }
        }



        public virtual Guid? sNewRef { get; set; }
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public virtual string sNewTitle { get; set; }

        public virtual string FlagTrashed { get; set; }

        public virtual string sNewKeyWord { get; set; }
        
    }
}
