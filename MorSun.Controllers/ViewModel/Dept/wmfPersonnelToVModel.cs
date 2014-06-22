using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using MorSun.Model;
using System.Web.Mvc;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{

    public class wmfPersonnelToVModel : BaseVModel<wmfPersonnelTo>
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        public override IQueryable<wmfPersonnelTo> List
        {
            get
            {
                var l = All;
                return l;
            }
        }

        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }

        //删除标记
        public virtual int FlagDeleted { get; set; }

        //回收站标记
        public virtual int FlagTrashed { get; set; }

        //去向开始时间
        public virtual DateTime? PersonnelToBeginTime { get; set; }

        //去向结束时间
        public virtual DateTime? PersonnelToEndTime { get; set; }

        //是否审核通过的
        public virtual bool IsAudit { get; set; }

        /// <summary>
        /// 多表分析，形成去向牌
        /// </summary>
        //public virtual List<PersonnelTo> AnalysisList
        //{
        //    get
        //    {
        //        var l = new List<PersonnelTo>();

        //        var AuditL = new List<PersonnelTo>();

        //        //今天的开始时间
        //        var nowbeginTimeDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));

        //        //今天的截至时间
        //        var nowendTimeDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

        //        if (PersonnelToBeginTime != null && PersonnelToEndTime != null)
        //        {
        //            //今天的开始时间
        //            nowbeginTimeDate = (DateTime)PersonnelToBeginTime;

        //            //今天的截至时间
        //            nowendTimeDate = (DateTime)PersonnelToEndTime;
        //        }

        //        //请假
        //        var leaveList = new BaseBll<kqwfLeave>().All.Where(p => (p.StartTime >= nowbeginTimeDate && p.EndTime <= nowendTimeDate) || (p.StartTime <= nowbeginTimeDate && p.EndTime >= nowendTimeDate) || (p.StartTime < nowbeginTimeDate && p.EndTime > nowbeginTimeDate && p.EndTime < nowendTimeDate) || (p.StartTime > nowbeginTimeDate && p.EndTime > nowendTimeDate));

        //        //外出服务
        //        var serviceList = new BaseBll<kqwfService>().All.Where(p => (p.StartTime >= nowbeginTimeDate && p.EndTime <= nowendTimeDate) || (p.StartTime <= nowbeginTimeDate && p.EndTime >= nowendTimeDate) || (p.StartTime < nowbeginTimeDate && p.EndTime > nowbeginTimeDate && p.EndTime < nowendTimeDate) || (p.StartTime > nowbeginTimeDate && p.EndTime > nowendTimeDate));

        //        //调休
        //        var daysOffList = new BaseBll<kqwfDaysOff>().All.Where(p => (p.StartTime >= nowbeginTimeDate && p.EndTime <= nowendTimeDate) || (p.StartTime <= nowbeginTimeDate && p.EndTime >= nowendTimeDate) || (p.StartTime < nowbeginTimeDate && p.EndTime > nowbeginTimeDate && p.EndTime < nowendTimeDate) || (p.StartTime > nowbeginTimeDate && p.EndTime > nowendTimeDate));

        //        //调换班 前
        //        var transferBeforeList = new BaseBll<kqwfTransfer>().All.Where(p => p.TransferTimeBefore == nowbeginTimeDate);

        //        //调换班 后
        //        var transferAfterList = new BaseBll<kqwfTransfer>().All.Where(p => p.TransferTimeAfter == nowbeginTimeDate);

        //        //人员去向登记
        //        var wfGoneList = new BaseBll<kqwfGone>().All.Where(p => (p.StartTime >= nowbeginTimeDate && p.EndTime <= nowendTimeDate) || (p.StartTime <= nowbeginTimeDate && p.EndTime >= nowendTimeDate) || (p.StartTime < nowbeginTimeDate && p.EndTime > nowbeginTimeDate && p.EndTime < nowendTimeDate) || (p.StartTime > nowbeginTimeDate && p.EndTime > nowendTimeDate));

        //        //出差
        //        var travelList = new BaseBll<kqwfEvection>().All.Where(p => ((p.StartTime >= nowbeginTimeDate && p.EndTime <= nowendTimeDate) || (p.StartTime <= nowbeginTimeDate && p.EndTime >= nowendTimeDate) || (p.StartTime < nowbeginTimeDate && p.EndTime > nowbeginTimeDate && p.EndTime < nowendTimeDate) || (p.StartTime > nowbeginTimeDate && p.EndTime > nowendTimeDate)) && (p.RESULT != "3-0") && (p.RESULT != "1-0") && (p.RESULT != "2-0"));

        //        //审核类别 请假
        //        var sqj = Guid.Parse(MorSun.Common.类别.Reference.审核类别_请假);

        //        //审核类别 服务-出差
        //        var sccha = Guid.Parse(MorSun.Common.类别.Reference.审核类别_出差);

        //        //审核类别 服务-外出
        //        var swc = Guid.Parse(MorSun.Common.类别.Reference.审核类别_外出);

        //        //审核类别 调休
        //        var stx = Guid.Parse(MorSun.Common.类别.Reference.审核类别_调休);

        //        //审核类别 调班
        //        var stb = Guid.Parse(MorSun.Common.类别.Reference.审核类别_调班);

        //        //审核类别 换班
        //        var shb = Guid.Parse(MorSun.Common.类别.Reference.审核类别_换班);

        //        //服务出差
        //        var cchai = Guid.Parse(MorSun.Common.类别.Reference.服务_出差);

        //        //服务外出
        //        var wchu = Guid.Parse(MorSun.Common.类别.Reference.服务_外出);

        //        //调班
        //        var tb = Guid.Parse(MorSun.Common.类别.Reference.调换班_调班);

        //        //换班
        //        var hb = Guid.Parse(MorSun.Common.类别.Reference.调换班_换班);

        //        //人员去向登记
        //        var qxdj = Guid.Parse(MorSun.Common.类别.Reference.审核类别_人员去向登记);

        //        var userList = new UserVModel().List;

        //        //流程实例状态
        //        var wf_wfentryList = new BaseBll<WF_WFENTRY>().All;

        //        #region 请假信息

        //        foreach (var item in leaveList)
        //        {
        //            var personnelTo = new PersonnelTo();

        //            //请假
        //            personnelTo.PersonnelToType = 1;

        //            //用户部门
        //            personnelTo.DeptName = item.Dept;

        //            //部门排序
        //            var userdept = item.aspnet_Users.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault();
        //            if (userdept != null)
        //            {
        //                personnelTo.DeptId = userdept.wmfDept.ID;
        //            }

        //            //用户ID
        //            personnelTo.UserId = item.UserId;

        //            //人员名称
        //            personnelTo.TrueName = item.aspnet_Users.wmfUserInfo.TrueName;

        //            //去向ID(请假类型)
        //            personnelTo.ToID = item.LeaveRef;

        //            //去向开始时间
        //            personnelTo.StartTime = item.StartTime;

        //            //去向截至时间
        //            personnelTo.EndTime = item.EndTime;

        //            //去向名称
        //            personnelTo.ToName = "请假";

        //            //联系电话
        //            personnelTo.Tel = item.aspnet_Users.wmfUserInfo.MobilePhone;

        //            //备注
        //            personnelTo.Remark = item.LeaveReason;

        //            //查看流程状态
        //            var wfentry = wf_wfentryList.Where(p => p.ID == item.WF_ID).First();
        //            if (wfentry != null && wfentry.STATE == 4)
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = (item.LeaveDays >= 21 ? (item.RESULT == "4-1" ? true : false) : (item.RESULT == "3-1" ? true : false));
        //            }
        //            else
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = false;
        //            }

        //            //审核请假
        //            personnelTo.RefID = sqj;

        //            //审核通过的记录
        //            if (personnelTo.IsAudit == true)
        //            {
        //                AuditL.Add(personnelTo);
        //            }
        //            else
        //            {
        //                l.Add(personnelTo);
        //            }
        //        }
        //        #endregion

        //        #region 外出服务
        //        foreach (var item in serviceList)
        //        {
        //            var personnelTo = new PersonnelTo();

        //            //外出服务
        //            personnelTo.PersonnelToType = 2;

        //            //用户部门
        //            personnelTo.DeptName = item.Dept;

        //            //部门排序
        //            var userdept = item.aspnet_Users.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault();
        //            if (userdept != null)
        //            {
        //                personnelTo.DeptId = userdept.wmfDept.ID;
        //            }

        //            //用户ID
        //            personnelTo.UserId = item.UserId;

        //            //人员名称
        //            personnelTo.TrueName = item.aspnet_Users.wmfUserInfo.TrueName;

        //            //去向ID
        //            personnelTo.ToID = item.ServiceRef;

        //            //去向开始时间
        //            personnelTo.StartTime = item.StartTime;

        //            //去向截至时间
        //            personnelTo.EndTime = item.EndTime;

        //            //去向名称:出差 or 外出
        //            personnelTo.ToName = item.wmfReference.ItemValue;

        //            //联系电话
        //            personnelTo.Tel = item.aspnet_Users.wmfUserInfo.MobilePhone;

        //            //备注
        //            personnelTo.Remark = item.Remark;

        //            //查看流程状态
        //            var wfentry = wf_wfentryList.Where(p => p.ID == item.WF_ID).First();
        //            if (wfentry != null && wfentry.STATE == 4)
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = item.RESULT;
        //            }
        //            else
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = false;
        //            }

        //            //服务单位
        //            personnelTo.Address = item.ServiceUnit;

        //            if (item.ServiceRef == wchu)
        //            {
        //                //外出
        //                personnelTo.RefID = swc;
        //            }
        //            else
        //            {
        //                //出差
        //                personnelTo.RefID = sccha;
        //            }

        //            //审核通过的记录
        //            if (personnelTo.IsAudit == true)
        //            {
        //                AuditL.Add(personnelTo);
        //            }
        //            else
        //            {
        //                l.Add(personnelTo);
        //            }
        //        }
        //        #endregion

        //        #region 调休
        //        foreach (var item in daysOffList)
        //        {
        //            var personnelTo = new PersonnelTo();

        //            //调休
        //            personnelTo.PersonnelToType = 3;

        //            //用户部门
        //            personnelTo.DeptName = item.Dept;

        //            //部门排序
        //            var userdept = item.aspnet_Users.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault();
        //            if (userdept != null)
        //            {
        //                personnelTo.DeptId = userdept.wmfDept.ID;
        //            }

        //            //用户ID
        //            personnelTo.UserId = item.UserId;

        //            //人员名称
        //            personnelTo.TrueName = item.aspnet_Users.wmfUserInfo.TrueName;

        //            //去向ID
        //            //personnelTo.ToID = item.LeaveRef;

        //            //去向开始时间
        //            personnelTo.StartTime = item.StartTime;

        //            //去向截至时间
        //            personnelTo.EndTime = item.EndTime;

        //            //去向名称
        //            personnelTo.ToName = "调休";

        //            //联系电话
        //            personnelTo.Tel = item.aspnet_Users.wmfUserInfo.MobilePhone;

        //            //备注
        //            personnelTo.Remark = item.Reason;

        //            //查看流程状态
        //            var wfentry = wf_wfentryList.Where(p => p.ID == item.WF_ID).First();
        //            if (wfentry != null && wfentry.STATE == 4)
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = (item.LeaveDays >= 21 ? (item.RESULT == "4-1" ? true : false) : (item.RESULT == "3-1" ? true : false));
        //            }
        //            else
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = false;
        //            }

        //            //调休
        //            personnelTo.RefID = stx;

        //            //审核通过的记录
        //            if (personnelTo.IsAudit == true)
        //            {
        //                AuditL.Add(personnelTo);
        //            }
        //            else
        //            {
        //                l.Add(personnelTo);
        //            }
        //        }
        //        #endregion

        //        #region 调换班 前
        //        foreach (var item in transferBeforeList)
        //        {
        //            var personnelTo = new PersonnelTo();


        //            //用户部门
        //            personnelTo.DeptName = item.Dept;

        //            //部门排序
        //            var userdept = item.aspnet_Users.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault();
        //            if (userdept != null)
        //            {
        //                personnelTo.DeptId = userdept.wmfDept.ID;
        //            }

        //            //用户ID
        //            personnelTo.UserId = item.UserId;

        //            //人员名称
        //            personnelTo.TrueName = item.aspnet_Users.wmfUserInfo.TrueName;

        //            //userList.Where(p => p.wmfUserInfo.AutoGeneticId == item.TransferUserId);

        //            //去向ID
        //            personnelTo.ToID = item.TransferRef;

        //            //调整前 时间
        //            personnelTo.StartTime = item.TransferTimeBefore;

        //            //调整后 时间
        //            personnelTo.EndTime = item.TransferTimeAfter;

        //            //换班
        //            if (item.TransferRef == hb)
        //            {
        //                //去向名称 换班
        //                personnelTo.ToName = "与" + item.ByTransferUserName + item.wmfReference.ItemValue;
        //            }
        //            else
        //            {
        //                //去向名称 调班
        //                personnelTo.ToName = item.wmfReference.ItemValue;
        //            }

        //            //联系电话
        //            personnelTo.Tel = item.aspnet_Users.wmfUserInfo.MobilePhone;

        //            //备注
        //            personnelTo.Remark = item.LeaveReason;

        //            //查看流程状态
        //            var wfentry = wf_wfentryList.Where(p => p.ID == item.WF_ID).First();
        //            if (wfentry != null && wfentry.STATE == 4)
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = (item.RESULT == "3-1" ? true : false);
        //            }
        //            else
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = false;
        //            }

        //            if (item.TransferRef == tb)
        //            {
        //                //调班
        //                personnelTo.RefID = stb;
        //            }
        //            else
        //            {
        //                //换班
        //                personnelTo.RefID = shb;
        //            }
        //            //调换班
        //            personnelTo.PersonnelToType = 4;

        //            //审核通过的记录
        //            if (personnelTo.IsAudit == true)
        //            {
        //                AuditL.Add(personnelTo);
        //            }
        //            else
        //            {
        //                l.Add(personnelTo);
        //            }
        //        }
        //        #endregion

        //        #region 调换班 后
        //        foreach (var item in transferAfterList)
        //        {
        //            var personnelTo = new PersonnelTo();

        //            //换班
        //            if (item.TransferRef == hb)
        //            {
        //                //换班
        //                personnelTo.RefID = shb;

        //                //用户部门
        //                personnelTo.DeptName = item.Dept;

        //                //部门排序
        //                var byuser = userList.Where(p => p.wmfUserInfo.AutoGeneticId == item.ByTransferAfterUserId).FirstOrDefault();

        //                var userdept = byuser.wmfUserDeptPositions.Where(p => p.aspnet_Users.wmfUserInfo.AutoGeneticId == item.ByTransferAfterUserId).FirstOrDefault();
        //                if (userdept != null)
        //                {
        //                    personnelTo.DeptId = userdept.wmfDept.ID;
        //                }


        //                //用户ID
        //                personnelTo.UserId = byuser.UserId;

        //                //人员名称
        //                personnelTo.TrueName = item.ByTransferUserName;

        //                //userList.Where(p => p.wmfUserInfo.AutoGeneticId == item.TransferUserId);

        //                //去向ID
        //                personnelTo.ToID = item.TransferRef;

        //                //调整前 时间
        //                personnelTo.StartTime = item.ByTransferTimeBefore;

        //                //调整后 时间
        //                personnelTo.EndTime = item.ByTransferTimeAfter;

        //                //换班
        //                if (item.TransferRef == hb)
        //                {
        //                    //去向名称 换班
        //                    personnelTo.ToName = "被" + item.TransferUserName + item.wmfReference.ItemValue;
        //                }
        //                else
        //                {
        //                    //去向名称 调班
        //                    personnelTo.ToName = item.wmfReference.ItemValue;
        //                }

        //                //联系电话
        //                personnelTo.Tel = byuser.wmfUserInfo.MobilePhone;

        //            }
        //            //调班
        //            else
        //            {
        //                //用户部门
        //                personnelTo.DeptName = item.Dept;

        //                //部门排序
        //                var userdept = item.aspnet_Users.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault();
        //                if (userdept != null)
        //                {
        //                    personnelTo.DeptId = userdept.wmfDept.ID;
        //                }

        //                //人员名称
        //                personnelTo.TrueName = item.aspnet_Users.wmfUserInfo.TrueName;

        //                //userList.Where(p => p.wmfUserInfo.AutoGeneticId == item.TransferUserId);

        //                //去向ID
        //                personnelTo.ToID = item.TransferRef;

        //                //调整前 时间
        //                personnelTo.StartTime = item.TransferTimeBefore;

        //                //调整后 时间
        //                personnelTo.EndTime = item.TransferTimeAfter;

        //                //换班
        //                if (item.TransferRef == hb)
        //                {
        //                    //去向名称 换班
        //                    personnelTo.ToName = "与" + item.ByTransferUserName + item.wmfReference.ItemValue;
        //                }
        //                else
        //                {
        //                    //去向名称 调班
        //                    personnelTo.ToName = item.wmfReference.ItemValue;
        //                }

        //                //联系电话
        //                personnelTo.Tel = item.aspnet_Users.wmfUserInfo.MobilePhone;

        //                //调班
        //                personnelTo.RefID = stb;
        //            }

        //            //调换班
        //            personnelTo.PersonnelToType = 4;

        //            //备注
        //            personnelTo.Remark = item.LeaveReason;


        //            //查看流程状态
        //            var wfentry = wf_wfentryList.Where(p => p.ID == item.WF_ID).First();
        //            if (wfentry != null && wfentry.STATE == 4)
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = (item.RESULT == "3-1" ? true : false);
        //            }
        //            else
        //            {
        //                personnelTo.IsAudit = false;
        //            }


        //            //审核通过的记录
        //            if (personnelTo.IsAudit == true)
        //            {
        //                AuditL.Add(personnelTo);
        //            }
        //            else
        //            {
        //                l.Add(personnelTo);
        //            }
        //        }
        //        #endregion

        //        #region 人员去向登记

        //        foreach (var item in wfGoneList)
        //        {
        //            var personnelTo = new PersonnelTo();

        //            //用户部门
        //            //personnelTo.DeptName = item.Dept;

        //            //部门排序
        //            var userdept = item.aspnet_Users.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault();
        //            if (userdept != null)
        //            {
        //                personnelTo.DeptId = userdept.wmfDept.ID;
        //            }

        //            //人员名称
        //            personnelTo.TrueName = item.aspnet_Users.wmfUserInfo.TrueName;

        //            //用户ID
        //            personnelTo.UserId = item.UserId;

        //            //去向ID(请假类型)
        //            //personnelTo.ToID = item.LeaveRef;

        //            //去向开始时间
        //            personnelTo.StartTime = item.StartTime;

        //            //去向截至时间
        //            personnelTo.EndTime = item.EndTime;

        //            //去向名称
        //            personnelTo.ToName = "行政登记";

        //            //联系电话
        //            personnelTo.Tel = item.aspnet_Users.wmfUserInfo.MobilePhone;

        //            //备注
        //            personnelTo.Remark = item.LeaveReason;

        //            //是否审核
        //            personnelTo.IsAudit = true;

        //            //人员去向登记
        //            personnelTo.RefID = qxdj;

        //            l.Add(personnelTo);

        //            AuditL.Add(personnelTo);
        //        }
        //        #endregion

        //        #region 出差登记
        //        foreach (var item in travelList)
        //        {
        //            var personnelTo = new PersonnelTo();
        //            //部门排序
        //            var userdept = item.aspnet_Users.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault();
        //            if (userdept != null)
        //            {
        //                personnelTo.DeptId = userdept.wmfDept.ID;
        //            }

        //            //用户ID
        //            personnelTo.UserId = item.UserId;

        //            //人员名称
        //            personnelTo.TrueName = item.aspnet_Users.wmfUserInfo.TrueName;

        //            //去向开始时间
        //            personnelTo.StartTime = item.StartTime;

        //            //去向截至时间
        //            personnelTo.EndTime = item.EndTime;

        //            //去向名称
        //            personnelTo.ToName = "出差登记";

        //            //联系电话
        //            personnelTo.Tel = item.aspnet_Users.wmfUserInfo.MobilePhone;

        //            personnelTo.Remark = item.Destination;


        //            //查看流程状态
        //            var wfentry = wf_wfentryList.Where(p => p.ID == item.WF_ID).First();
        //            if (wfentry != null && wfentry.STATE == 4)
        //            {
        //                //是否审核
        //                personnelTo.IsAudit = (item.RESULT == "3-1" || (item.RESULT == "7-1" && item.Food + item.TrafficSubsidy + item.Journey + item.StopAt < 500) || (item.RESULT == "8-1" && item.Food + item.TrafficSubsidy + item.Journey + item.StopAt >= 500) ? true : false);
        //            }
        //            else
        //            {
        //                personnelTo.IsAudit = false;
        //            }

        //            //出差登记
        //            personnelTo.PersonnelToType = 5;

        //            //审核通过的记录
        //            if (personnelTo.IsAudit == true)
        //            {
        //                AuditL.Add(personnelTo);
        //            }
        //            else
        //            {
        //                l.Add(personnelTo);
        //            }
        //        }
        //        #endregion
        //        if (IsAudit)
        //        {
        //            return AuditL;
        //        }
        //        else
        //        {
        //            return l;
        //        }
        //    }
        //}
    }
}
