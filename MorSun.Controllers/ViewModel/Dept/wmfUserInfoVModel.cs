using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.Objects;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    //[Bind]
    public class wmfUserInfoVModel : BaseVModel<wmfUserInfo>
    {

        /// <summary>
        /// 获取用户列表
        /// </summary>
        public override IQueryable<wmfUserInfo> List
        {
            get
            {
                //return All.Where(v => string.IsNullOrEmpty(UserName) || v..Contains(UserName));
                return All;
            }
        }


        /// <summary>
        /// 用户名
        /// </summary>
        public Guid? ID { get; set; }

        /// <summary>
        /// 获取同部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<wmfUserInfo> GetSameDeptUserListByUserId(Guid? userId)
        {
            var deptPosition = this.Dao.GetModel(userId).aspnet_Users.wmfUserDeptPositions.FirstOrDefault();
            var detpTypeId = deptPosition == null ? Guid.Empty : deptPosition.DeptId;
            var userList = this.List.Where(u => u.FlagTrashed == false && u.aspnet_Users.wmfUserDeptPositions.Count(j => j.DeptId == detpTypeId) > 0);
            return userList;
        }

        /// <summary>
        /// 获取用户真实姓名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserTrueName(Guid? userId)
        {
            var user = this.Dao.GetModel(userId);
            return user == null ? "" : user.TrueName;
        }

        public string GetUserNamesByIds(string userIds)
        {
            var userIdCollections = userIds.Split(',');
            var res = "";
            foreach (var item in userIdCollections)
            {
                var model = base.Dao.GetModel(Guid.Parse(item));
                var name = model == null ? "" : model.TrueName;
                res += string.IsNullOrEmpty(res) ? name : ("," + name);
            }
            return res;
        }


        /// <summary>
        /// 通过用户Id获取用户部门领导
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public aspnet_Users GetDeptLeader(Guid? userId)
        {
            var res = new aspnet_Users();
            var deptVModel = new DeptVModel();
            var userVModel = new UserVModel();
            var user = this.Dao.GetModel(userId);
            if (user != null)
            {
                var dept = user.aspnet_Users.wmfUserDeptPositions.First().wmfDept;
                var positionIds=dept.wmfPositions.Where(u=>u.Leader==1).Select(u=>u.ID);
                res =dept.wmfUserDeptPositions.Where(u=>positionIds.Contains(u.PostionId.Value)).Select(u=>u.aspnet_Users).FirstOrDefault();
            }
            return res;
        }
    }
}
