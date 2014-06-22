using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Web.Mvc;

namespace MorSun.Controllers.ViewModel
{
    public class CountyVModel : BaseVModel<wmfCounty>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public override IQueryable<wmfCounty> List
        {
            get
            {
                var l = All;
                if (!string.IsNullOrEmpty(CountyName))
                {
                    l = l.Where(p => p.CountyName.Contains(CountyName));
                }
                if (!string.IsNullOrEmpty(CityId))
                {
                    var ID = CityId.ToAs<Guid>();
                    l = l.Where(p => p.CityId == ID);
                }
                if (ProvinceId != null)
                {

                    l = l.Where(c => c.wmfCity.ProvinceId == ProvinceId);
                }
                return l.OrderBy(p=>p.wmfCity.wmfProvince.Sort).ThenBy(p=>p.wmfCity.Sort).ThenBy(p=>p.Sort);
            }
        }

        public virtual string CountyName { get; set; }

        public virtual string CityId { get; set; }

        public virtual Guid? ProvinceId { get; set; }

        /// <summary>
        /// 获取对应的城市下的城镇下拉框,默认获取福州市
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetCountySelectList(Guid? cityId)
        {
            if(cityId==null)
            {
                cityId = new Guid("da9f1f50-6073-4705-bc75-952893089b75");
            }
            return new SelectList(this.List.Where(u=>u.CityId==cityId),"ID","CountyName");
        }
    }
}
