using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;

namespace MorSun.Controllers.ViewModel
{
    public class ProvinceVModel : BaseVModel<wmfProvince>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public override IQueryable<wmfProvince> List
        {
            get
            {
                var l = All;
                if (!string.IsNullOrEmpty(ProvinceName))
                {
                    l = l.Where(p => p.ProvinceName.Contains(ProvinceName));
                }

                return l.OrderBy(p=>p.Sort);
            }
        }

        public virtual string ProvinceName { get; set; }
    }
}
