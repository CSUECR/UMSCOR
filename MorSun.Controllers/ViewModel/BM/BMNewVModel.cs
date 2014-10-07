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
                if (!String.IsNullOrEmpty(NewTitle))
                {
                    l = l.Where(p => p.NewTitle == NewTitle);
                }
                if (NewRef != null && NewRef != Guid.Empty)
                {
                    l = l.Where(p => p.NewRef == NewRef);
                }
                if(!String.IsNullOrEmpty(NewKeyWord))
                {
                    l = l.Where(p => p.NewKeyWord.Contains(NewKeyWord));
                }
                return l.OrderBy(p => p.Sort).ThenBy(p => p.RegTime);
            }
        }



        public virtual Guid? NewRef { get; set; }
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public virtual string NewTitle { get; set; }

        public virtual string FlagTrashed { get; set; }

        public virtual string NewKeyWord { get; set; }
        
    }
}
