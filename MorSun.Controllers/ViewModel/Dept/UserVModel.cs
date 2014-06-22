using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.Objects;
using MorSun.Model;
using System.Web.Mvc;

namespace MorSun.Controllers.ViewModel
{
    //[Bind]
    public class UserVModel : BaseVModel<aspnet_Users>
    {

        /// <summary>
        /// 获取用户列表
        /// </summary>
        public override IQueryable<aspnet_Users> List
        {
            get
            {
                var l = All;
                if (!string.IsNullOrEmpty(UserName))
                {
                    l = l.Where(r => r.UserName.Contains(UserName));
                }
                if (Dep != null)
                {
                    l = l.Where(r => r.wmfUserDeptPositions.Where(t => t.DeptId == Dep).FirstOrDefault().UserId == r.UserId);
                }
                if (!string.IsNullOrEmpty(UserTrueName))
                {
                    l = l.Where(r => r.wmfUserInfo.TrueName.Contains(UserTrueName));
                }
                ////已经被，回收站
                //if (FlagTrashed == "1")
                //{
                //    l = l.Where(r => r.wmfUserInfo.FlagTrashed == true);
                //}
                //if (FlagTrashed == "0")
                //{
                //    l = l.Where(r => r.wmfUserInfo.FlagTrashed == false);
                //}
                //离职
                if (IsApproved == "0")
                {
                    l = l.Where(r => r.aspnet_Membership.IsApproved == false);
                }
                //在职
                if (IsApproved == "1")
                {
                    l = l.Where(r => r.aspnet_Membership.IsApproved == true);
                }
                //离职
                if (IsFlagTrashed)
                    l = l.Where(r => r.wmfUserInfo.FlagTrashed == true);
                else
                    l = l.Where(r => r.wmfUserInfo.FlagTrashed == false);
                //退休
                if(IsFlagDeleted)
                    l = l.Where(r => r.wmfUserInfo.FlagDeleted == true);
                else
                    l = l.Where(r => r.wmfUserInfo.FlagDeleted == false);
                if (IsNoCheck == "1")
                {
                    l = l.Where(r => r.wmfUserInfo.IsNoCheck== true);
                }
                return l;
            }
        }


        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        public Guid? Dep { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserTrueName { get; set; }

        public string formPassword { get; set; }
        /// <summary>
        /// 免签标识
        /// </summary>
        public string IsNoCheck { get; set; }

        ///// <summary>
        ///// 绑定部门
        ///// </summary>
        //public IQueryable<wmfDept> DepList
        //{
        //    get
        //    {
        //        var deplist = All;
        //        var newList = new List<wmfDept>();
        //        var top = deplist.Where(v => v.ParentId == null);
        //        foreach (var dep in top)
        //        {
        //            newList.Add(dep);
        //            var childDeps = deplist.Where(v => v.ParentId == dep.Id);
        //            newList.AddRange(childDeps);
        //        }
        //        return newList.AsQueryable();
        //    }
        //}

        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public virtual string IsApproved { get; set; }
        //离职
        public virtual bool IsFlagTrashed { get; set; }
        //退休
        public virtual bool IsFlagDeleted { get; set; }

        public IEnumerable<SelectListItem> GetTeacherSelectList()
        {
            return new SelectList(this.List, "UserId", "UserName");
        }

        /// <summary>
        /// 通过userid获取用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserNameByUserId(Guid? userId)
        {
            var userName = string.Empty;
            var user = this.All.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                userName = user.UserName;
            }
            return userName;
        }
    }
}
