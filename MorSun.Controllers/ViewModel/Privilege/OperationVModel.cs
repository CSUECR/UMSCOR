using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using MorSun.Model;
using System.Web.Mvc;

namespace MorSun.Controllers.ViewModel
{
    public class OperationVModel : BaseVModel<wmfOperation>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public override IQueryable<wmfOperation> List
        {
            get
            {

                var l = All;
                if (!string.IsNullOrEmpty(OperationCNName))
                {
                    l = l.Where(r => r.OperationCNName.Contains(OperationCNName));
                }
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
                return l.OrderBy(p => p.Sort).ThenBy(p => p.OperationCNName);
            }
        }
        public virtual string OperationCNName { get; set; }
    }


}
