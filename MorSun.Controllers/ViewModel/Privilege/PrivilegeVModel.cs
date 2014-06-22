using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    /// <summary>
    /// 资源
    /// </summary>
    public class PrivilegeVModel : BaseVModel<wmfPrivilege>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }
        public virtual string PrivilegeCNName { get; set; }
        public virtual Guid? ResourceID { get; set; }
        public virtual Guid? OperationID { get; set; }
        public override IQueryable<wmfPrivilege> List
        {
            get
            {
                var l = All;
                if (!string.IsNullOrEmpty(PrivilegeCNName))
                {
                    l = l.Where(r => r.PrivilegeCNName.Contains(PrivilegeCNName));
                }
                if (ResourceID != null && ResourceID != Guid.Empty)
                {
                    l = l.Where(r => r.ResourcesId == ResourceID);
                }
                if (OperationID != null && OperationID != Guid.Empty)
                {
                    l = l.Where(r => r.OperationId == OperationID);
                }

                return l.OrderBy(p => p.wmfResource.Sort).ThenBy(p => p.wmfOperation.Sort);
            }
        }
    }
}
