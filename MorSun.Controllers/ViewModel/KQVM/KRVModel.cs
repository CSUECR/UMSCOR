using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    public class KRVModel : BaseVModel<kqRecord>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        public string errString = "";
        public string formCid = "";
        public bool isManagerPage { get; set; }

        //public IQueryable<wmfResource> wfApplyTable
        //{
        //    get{
        //        return new BaseBll<wmfResource>().All.Where(r=>r.ParentId == new Guid(MorSun.Common.Privelege.资源.考勤申请表)).OrderBy(r=>r.Sort);
        //    }
        //}
    }
}
