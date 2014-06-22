using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;

namespace MorSun.Controllers.ViewModel
{
    public class ReferListVModel : BaseVModel<wmfReference>
    {
        public override IQueryable<wmfReference> List
        {
            get
            {
                var l = All;
                if (!Guid.Equals(RefGroupId, null))
                {
                    l = l.Where(p => p.RefGroupId == RefGroupId);
                }
                //没传值的话默认取未被删除的
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
                SumNum = l.Count().ToString();
                return from q in l orderby q.RefGroupId ascending, q.Sort ascending select q;
            }
        }

        public virtual Guid? RefGroupId { get; set; }
        public virtual string SumNum { get; set; }
        public virtual string FlagTrashed { get; set; }
    }
}
