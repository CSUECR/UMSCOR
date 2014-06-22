//T4模板生成的，只需将上一行删除，并且将后缀名改成cs文件然后放在MorSun.Model类库中修改一些即可
using System;
using System.ComponentModel.DataAnnotations;
namespace MorSun.Model
{
    [MetadataType(typeof(HF_WorkTaskInstanceViewMetadata))]
    public partial class HF_WorkTaskInstanceView
    {
       /// <summary>
       /// 未使用，只是为了页面的统一显示
       /// </summary>
        [Display(Name="申请人")]
        public string ApplyUser { get; set; }
        [Display(Name="操作")]
        /// <summary>
        /// 未使用，只是为了页面的统一显示
        /// </summary>
        public string Operate { get; set; }
    }

    public class HF_WorkTaskInstanceViewMetadata
    {
        [Display(Name="优先级")]
        public System.String Priority;
        [Display(Name = "流程编号")]
        public System.Int32 WorkFlowNo;
        [Display(Name = "到达时间")]
        public System.DateTime taskStartTime;
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
        public System.Int32 OperType;
        public System.String TaskTypeId;
        public System.Boolean IsSubWorkFlow;
        public System.String MainWorkTaskId;
        public System.String MainWorkFlowInstanceId;
        public System.String OperatedDes;
        public System.DateTime OperDateTime;
        public System.DateTime taskEndTime;
        public System.DateTime flowStartTime;
        public System.DateTime flowEndTime;
        [Display(Name="提交人")]
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
        public System.String NowTaskId;
        public System.String UserName;
        public System.Boolean IsJumpSelf;
        public System.Boolean IsDebug;
    }
}

