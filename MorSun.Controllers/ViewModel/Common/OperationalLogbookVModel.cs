using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    public class OperationalLogbookVModel:BaseVModel<wmfOperationalLogbook>
    {
        public virtual string CheckedId { get; set; }
        public virtual string RegTimeFrom { get; set; }
        public virtual string RegTimeTo { get; set; }
        public virtual string OperateContent { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Dep { get; set; }

        public override IQueryable<wmfOperationalLogbook> List
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

                if (!string.IsNullOrEmpty(RegTimeFrom))
                {
                    var from = RegTimeFrom.ToDateFrom(false);
                    l = l.Where(p => p.RegTime >= from);
                }
                if (!string.IsNullOrEmpty(RegTimeTo))
                {
                    var to = RegTimeTo.ToDateTo(false);
                    l = l.Where(p => p.RegTime <= to);
                }

                if (!string.IsNullOrEmpty(OperateContent))
                {
                    l = l.Where(p => p.OperateContent == OperateContent);
                }
                if (!string.IsNullOrEmpty(Dep) && !string.IsNullOrEmpty(UserId))
                {
                    var ID = Guid.Parse(UserId);
                    l = l.Where(p => p.UserId == ID);
                }

                if (!string.IsNullOrEmpty(myUserId))
                {
                    var ID = Guid.Parse(myUserId);
                    l = l.Where(p => p.UserId == ID);
                }

                return from q in l orderby q.RegTime descending,q.ModTime descending select q;
            }
        }

        public string myUserId { get; set; }

       private BaseBll<aspnet_Users> userBll;

        public virtual BaseBll<aspnet_Users> UserBll
        {
            get
            {
                userBll = userBll.Load();
                return userBll;
            }
            set { userBll = value; }
        }

        /// <summary>
        /// 全部员工
        /// </summary>
        public virtual IQueryable<aspnet_Users> Users
        {
            get
            {
                return UserBll.All;
            }
        }
        
    }
}
