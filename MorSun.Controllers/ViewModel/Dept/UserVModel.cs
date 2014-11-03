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
                if (!string.IsNullOrEmpty(UserTrueName))
                {
                    l = l.Where(r => r.wmfUserInfo.TrueName.Contains(UserTrueName));
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

                if (CLevel.HasValue)
                    l = l.Where(p => p.wmfUserInfo.CertificationLevel == CLevel);

                return l;
            }
        }


        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }        

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserTrueName { get; set; }
       

        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        public virtual string IsApproved { get; set; }
        //离职
        public virtual bool IsFlagTrashed { get; set; }
        //退休
        public virtual bool IsFlagDeleted { get; set; }

        /// <summary>
        /// 认证级别
        /// </summary>
        public Guid? CLevel { get; set; }
    }
}
