using System;
using System.ComponentModel.DataAnnotations;

namespace MorSun.Model
{
    public class HF_WorkTaskInstanceViewDTO
    {
        [Display(Name = "优先级")]
        public string Priority;
        [Display(Name = "流程编号")]
        public int? WorkFlowNo;
        [Display(Name = "到达时间")]
        public DateTime? taskStartTime;
        public System.String TaskInstanceCaption;
        [Display(Name = "业务名")]
        public System.String FlowInstanceCaption;
        public System.String OperContent;
        public System.String FlowCaption;
        [Display(Name = "任务名")]
        public System.String TaskCaption;
        public System.String UserId;
        public System.String OperatorInstanceId;
        public System.String WorkFlowId;
        public System.String WorkTaskId;
        public System.String WorkFlowInstanceId;
        public System.String WorkTaskInstanceId;
        public int? OperType;
        public System.String TaskTypeId;
        public System.Boolean? IsSubWorkFlow;
        public System.String MainWorkTaskId;
        public System.String MainWorkFlowInstanceId;
        public System.String OperatedDes;
        public System.DateTime? OperDateTime;
        public System.DateTime? taskEndTime;
        public System.DateTime? flowStartTime;
        public System.DateTime? flowEndTime;
        [Display(Name = "提交人")]
        public System.String pOperatedDes;
        public System.String Description;
        public System.String OperStatus;
        public System.String Status;
        public System.String flStatus;
        public System.String PreviousTaskInstanceId;
        public System.String OperContentText;
        public System.String taskInsType;
        public System.String SuccessMsg;
        public System.String ChangeOperator;
        public System.String TaskInsDescription;
        public string NowTaskId;
        public string UserName;
        public bool? IsJumpSelf;
        public System.Boolean? IsDebug;
    }
}

