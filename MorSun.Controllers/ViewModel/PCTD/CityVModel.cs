using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Web.Mvc;

namespace MorSun.Controllers.ViewModel
{
    public class CityVModel : BaseVModel<wmfCity>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public override IQueryable<wmfCity> List
        {
            get
            {
                var l = All;
                if (!string.IsNullOrEmpty(CityName))
                {
                    l = l.Where(p => p.CityName.Contains(CityName));
                }
                if (!string.IsNullOrEmpty(ProvinceId))
                { 
                    var ID=ProvinceId.ToAs<Guid>();
                    l = l.Where(p => p.ProvinceId == ID);
                }
                return l.OrderBy(p=>p.wmfProvince.Sort).ThenBy(p=>p.Sort);
            }
        }

        public virtual string CityName { get; set; }

        public virtual string ProvinceId { get; set; }

        /// <summary>
        /// 获取对应的城市下的城镇下拉框,默认获取福州市
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetCitySelectList(Guid? provinceId)
        {
            if (provinceId == null)
            {
                provinceId = new Guid("fe6945a9-b211-440a-8604-b89504fefcb5");
            }
            return new SelectList(this.List.Where(u => u.ProvinceId == provinceId), "ID", "CityName");
        }
    }
}
