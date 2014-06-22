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
                //if (ResourcesID != null && ResourcesID != Guid.Empty)
                //{
                //    l = l.Where(r => r.ParentId == ResourcesID);
                //}
                //if (!string.IsNullOrEmpty(ResourcesName))
                //{
                //    l = l.Where(r => r.ResourcesName.Contains(ResourcesName));
                //}
                return from q in l orderby q.Sort ascending select q;
            }
        }
        public virtual string OperationCNName { get; set; }
    }


}
