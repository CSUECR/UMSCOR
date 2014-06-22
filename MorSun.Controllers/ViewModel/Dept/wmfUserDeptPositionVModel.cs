using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;

namespace MorSun.Controllers.ViewModel
{
    public class wmfUserDeptPositionVModel : BaseVModel<wmfUserDeptPosition>
    {
        public override IQueryable<wmfUserDeptPosition> List
        {
            get
            {
                var l = All;
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }
                return l;
            }
        }

        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }
        //删除标记
        public virtual string FlagDeleted { get; set; }
        //回收站标记
        public virtual string FlagTrashed { get; set; }
        /// <summary>
        /// 通过Userid获取岗位id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Guid GetPositionIdByUserId(Guid? userId)
        {
            var result = Guid.Empty;
            var deptModel = All.FirstOrDefault(u => u.UserId == userId);
            if (deptModel != null)
            {
                result = deptModel.PostionId.GetValueOrDefault();
            }
            return result;
        }

        /// <summary>
        /// 通过userid 获取deptid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetDeptIdByUserId(Guid? userId)
        {
            var result = string.Empty;
            var deptModel = All.FirstOrDefault(u => u.UserId == userId);
            if (deptModel != null)
            {
                result = deptModel.DeptId.GetValueOrDefault().ToString();
            }
            return result;
        }
        /// <summary>
        /// 通过userid 获取deptid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetDeptIdByUserId(string userId)
        {
            var result = string.Empty;
            var userGuid = Guid.Empty;
            if (Guid.TryParse(userId, out userGuid))
            {
                result = GetDeptIdByUserId(userGuid);
            }
            return result;
        }
        /// <summary>
        /// get deptName by deptid 
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public string GetDeptNameByDeptId(string deptId)
        {
            var deptName = string.Empty;
            var deptGuid = Guid.Empty;
            if (Guid.TryParse(deptId, out deptGuid))
                deptName = GetDeptNameByDeptId(deptGuid);
            return deptName;
        }
        /// <summary>
        ///  get deptName by deptid 
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public string GetDeptNameByDeptId(Guid? deptId)
        {
            var deptName = string.Empty;

            var dept = this.All.FirstOrDefault(u => u.DeptId == deptId);
            if (dept != null && dept.wmfDept != null)
            {
                deptName = dept.wmfDept.DeptName;
            }
            return deptName;
        }
    }
}
