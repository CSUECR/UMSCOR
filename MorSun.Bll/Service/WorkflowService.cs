using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HOHO18.Common.Web;
using MorSun.Model;

namespace MorSun.Bll.Service
{
    public enum WorkflowStatusType
    {
        /// <summary>
        /// 默认，初始化时候调用
        /// </summary>
        [Description("")]
        Default = 0,

        /// <summary>
        /// 未提交
        /// </summary>
        [Description("未提交")]
        NoSubmit = 1,
        /// <summary>
        /// 审批通过
        /// </summary>
        [Description("审批通过")]
        Success = 2,
        /// <summary>
        /// 被退回
        /// </summary>
        [Description("被退回")]
        Back = 3,
        /// <summary>
        /// 审批中
        /// </summary>
        [Description("审批中")]
        Aduiting = 4
    }


    public class WorkflowService
    {

        #region Field
        private BaseBll<Aduit> _auditBll = new BaseBll<Aduit>();
        private BaseBll<HF_WorkTaskInstanceView> _wtInsViewBll = new BaseBll<HF_WorkTaskInstanceView>();
        private BaseBll<HF_OperContentView> _operContentViewBll = new BaseBll<HF_OperContentView>();
        private BaseBll<HF_AccreditInstanceView> _accreditInsViewBll = new BaseBll<HF_AccreditInstanceView>();
        private BaseBll<HF_WorkTask> _wtBll = new BaseBll<HF_WorkTask>();
        private string _hfDomain = webConfigHelp.GetWebConfigValue("hfDomain");
        #endregion

        #region  Get Workflow Status Function
        /// <summary>
        /// 判断流程实例是否完成
        /// </summary>
        /// <param name="worktaskInstanceViewBll">bll</param>
        /// <param name="workflowInsId">流程实例id</param>
        /// <returns></returns>
        public bool IsCompleteWorkflowIns(string workflowInsId)
        {
            var isComplete = false;
            isComplete = _wtInsViewBll.All.Where(u => u.WorkFlowInstanceId == workflowInsId).All(u => u.flStatus == "3");
            return isComplete;
        }

        /// <summary>
        /// 是被退回
        /// </summary>
        /// <param name="worktaskInsViewBll"></param>
        /// <param name="workflowInsId"></param>
        /// <returns></returns>
        public bool IsBackWorkflowIns(string workflowInsId)
        {
            var isBack = false;
            isBack = (_wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == workflowInsId && u.Status == "4") != null);
            return isBack;
        }

        /// <summary>
        /// 未认领(表单在提交人提交时，又点击放弃，这时候任务是未认领状态(注意：增加是否只有一条工作流节点记录的判断，否则都会有未提交的状态出来)，但工作流状态是审批中)
        /// </summary>
        /// <param name="worktaskInsViewBll"></param>
        /// <param name="workflowInsId"></param>
        /// <returns></returns>
        public bool IsNoReceiveWorkflowIns(string workflowInsId)
        {
            var isBack = false;
            isBack = (_wtInsViewBll.All.Where(u => u.WorkFlowInstanceId == workflowInsId).Count() ==1) && (_wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == workflowInsId && u.Status == "1") != null);
            return isBack;
        }

        /// <summary>
        /// 获取流程实例的状态信息
        /// </summary>
        /// <param name="worktaskInstanceViewBll"></param>
        /// <param name="aduit"></param>
        /// <returns></returns>
        public string GetWorkflowInsStatusMessage(Guid? contentId)
        {
            var status = GetWorkflowInsStatus(contentId);
            //状态信息
            return GetMessageByWorkflowStatus(status);
        }


        /// <summary>
        /// 获取指定工作流状态的aduit集合
        /// </summary>
        /// <param name="wfStatus">工作流审批状态</param>
        /// <param name="resourceId">资源id</param>
        /// <returns></returns>
        public IQueryable<Aduit> GetWorkflowStatusAuditList(WorkflowStatusType wfStatus, string resourceId)
        {
            if (string.IsNullOrEmpty(resourceId))
                throw new ArgumentNullException("resourceId");
            var wfStatusValue = (int)wfStatus;
            var resourceGuid = Guid.Empty;
            if (!Guid.TryParse(resourceId, out resourceGuid))
                throw new Exception("resourceId转换失败");
            //UpdateAuditWorkflowStatus(wfStatus, resourceGuid);//移到工程查询列表查询
            IQueryable<Aduit> result = null;
            if (wfStatus == WorkflowStatusType.Default)
            {
                result = _auditBll.All.Take(0);
            }
            if (wfStatus == WorkflowStatusType.NoSubmit)
            {
                result = _auditBll.All.Where(u => u.ResourceID == resourceGuid);
            }
            if (wfStatus == WorkflowStatusType.Success)
            {
                result = _auditBll.All.Where(u => u.ResourceID == resourceGuid && u.WorkflowInsStatus == wfStatusValue);
            }
            if (wfStatus == WorkflowStatusType.Back)
            {
                result = _auditBll.All.Where(u => u.ResourceID == resourceGuid && u.WorkflowInsStatus == wfStatusValue);
            }
            if (wfStatus == WorkflowStatusType.Aduiting)
            {
                result = _auditBll.All.Where(u => u.ResourceID == resourceGuid && u.WorkflowInsStatus == wfStatusValue);
            }
            return result;
        }
        /// <summary>
        /// 获取指定工作流状态的aduit表中的ContentId集合
        /// </summary>
        /// <param name="wfStatus"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public IQueryable<Guid?> GetWorkflowStatusAuditContentIds(WorkflowStatusType wfStatus, string resourceId)
        {
            var result = GetWorkflowStatusAuditList(wfStatus, resourceId);
            return result.Select(u => u.ContentID);
        }
        

        /// <summary>
        ///更新aduit表中的对应审批状态信息
        /// </summary>
        /// <param name="wfStatus">工作流状态</param>
        /// <param name="resourceId">资源Id</param>
        private void UpdateAuditWorkflowStatus(WorkflowStatusType wfStatus, Guid resourceId)
        {

            //审批中的时候要审批成功的集合和审批被退回的
            if (wfStatus == WorkflowStatusType.Success || wfStatus == WorkflowStatusType.Aduiting)
            {
                var successStatusValue = (int)WorkflowStatusType.Success;
                var auditIds = _auditBll.All.Where(u => u.ResourceID == resourceId && u.WorkflowInsStatus != successStatusValue
                    && _wtInsViewBll.All.Where(r => r.WorkFlowInstanceId == u.workflowInsId).All(r => r.flStatus == "3")).Select(u => u.ID);
                foreach (var id in auditIds)
                {
                    var model = _auditBll.GetModel(id);
                    model.WorkflowInsStatus = (int)WorkflowStatusType.Success;//改为审批通过2
                }
                _auditBll.UpdateChanges();
            }
            if (wfStatus == WorkflowStatusType.Back || wfStatus == WorkflowStatusType.Aduiting)
            {
                var backStatusValue = (int)WorkflowStatusType.Back;
                var auditIds = _auditBll.All.Where(u => u.ResourceID == resourceId && u.WorkflowInsStatus != backStatusValue
                    && _wtInsViewBll.All.Where(r => r.WorkFlowInstanceId == u.workflowInsId && r.Status == "4").Any()).Select(u => u.ID);
                foreach (var id in auditIds)
                {
                    var model = _auditBll.GetModel(id);
                    model.WorkflowInsStatus = (int)WorkflowStatusType.Back;//改为退回3
                }
                _auditBll.UpdateChanges();
            }
        }

        /// <summary>
        ///更新aduit表中的对应审批状态信息,统一更新,在工程查询界面做以下工作
        /// </summary>
        /// <param name="wfStatus">工作流状态</param>
        /// <param name="resourceId">资源Id</param>
        public void AllUpdateAuditWorkflowStatus()
        {            
                var successStatusValue = (int)WorkflowStatusType.Success;
                var auditIdsSuccess = _auditBll.All.Where(u => u.WorkflowInsStatus != successStatusValue
                    && _wtInsViewBll.All.Where(r => r.WorkFlowInstanceId == u.workflowInsId).All(r => r.flStatus == "3")).Select(u => u.ID);
                foreach (var id in auditIdsSuccess)
                {
                    var model = _auditBll.GetModel(id);
                    model.WorkflowInsStatus = (int)WorkflowStatusType.Success;
                }
            
                var backStatusValue = (int)WorkflowStatusType.Back;
                var auditIdsBack = _auditBll.All.Where(u => u.WorkflowInsStatus != backStatusValue
                    && _wtInsViewBll.All.Where(r => r.WorkFlowInstanceId == u.workflowInsId && r.Status == "4").Any()).Select(u => u.ID);
                foreach (var id in auditIdsBack)
                {
                    var model = _auditBll.GetModel(id);
                    model.WorkflowInsStatus = (int)WorkflowStatusType.Back;
                }
                _auditBll.UpdateChanges();            
        }
        #endregion

        #region Format Url Function

        /// <summary>
        /// 获取流程实例的状态（审批中，审批通过，被退回，未提交）
        /// </summary>
        /// <param name="worktaskInstanceViewBll"></param>
        /// <param name="aduit"></param>
        /// <returns></returns>
        /// 
        public WorkflowStatusType GetWorkflowInsStatus(Guid? contentId)
        {
            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == contentId);
            WorkflowStatusType status;
            if (audit == null)
                status = WorkflowStatusType.NoSubmit;
            else
            {
                if (IsCompleteWorkflowIns(audit.workflowInsId))
                {
                    status = WorkflowStatusType.Success;
                }
                else if (IsBackWorkflowIns(audit.workflowInsId))
                {
                    status = WorkflowStatusType.Back;
                }
                else if (IsNoReceiveWorkflowIns(audit.workflowInsId))
                {
                    status = WorkflowStatusType.NoSubmit;
                }
                else
                {
                    status = WorkflowStatusType.Aduiting;
                }
            }
            return status;
        }
        /// <summary>
        /// 是未提交或者被退回
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public bool IsNoSubmitOrBack(Guid? contentId)
        {
            var result = false;
            var wfStatus = GetWorkflowInsStatus(contentId);
            result = (wfStatus == WorkflowStatusType.NoSubmit || wfStatus == WorkflowStatusType.Back);
            return result;
        }
        /// <summary>
        /// 获取未提交审批的格式化链接，或者被退回的控制链接
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public string GetStartUrlOrBeBackedUrl(Guid? contentId, string resourceId, HF_GetAllowsStartWorkFlowsPro_Result startWfResult, string controllerName, string actionName)
        {
            var url = string.Empty;
            var wfStatus = GetWorkflowInsStatus(contentId);
            if (wfStatus == WorkflowStatusType.NoSubmit)
            {
                url = GetStartUrlFormat(startWfResult, controllerName, actionName, contentId, resourceId);
            }
            else if (wfStatus == WorkflowStatusType.Back)
            {
                url = GetCtrlUrlWhileBeBacked(contentId);
            }
            return url;
        }
        /// <summary>
        /// 获取未提交审批的格式化链接，或者被退回的控制链接
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="resourceId">资源id</param>
        /// <param name="wfCaption">流程名称</param>
        /// <param name="userId">当前用户id</param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public string GetStartUrlOrBeBackedUrl(Guid? contentId, string resourceId, string wfCaption, string userId, string controllerName, string actionName)
        {
            var swfP = getStartWorkflowProResult(wfCaption, userId);
            return GetStartUrlOrBeBackedUrl(contentId, resourceId, swfP, controllerName, actionName);
        }

        private HF_GetAllowsStartWorkFlowsPro_Result getStartWorkflowProResult(string flowCaption, string userId)
        {
            var startWF = _auditBll.Db.HF_GetAllowsStartWorkFlowsPro(userId).FirstOrDefault(u => u.FlowCaption.Contains(flowCaption));
            return startWF;
        }

        /// <summary>
        /// 当被退回的时候，获取控制节点的url;
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public string GetCtrlUrlWhileBeBacked(Guid? contentId)
        {
            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == contentId);
            var wfStatus = GetWorkflowInsStatus(contentId);
            if (wfStatus != WorkflowStatusType.Back)
                throw new ArgumentNullException("contentId");
            var wtIV = _wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == audit.workflowInsId && u.Status == "4");
            return GetCtrlUrlFormat(wtIV.WorkTaskInstanceId, wtIV.OperatorInstanceId);
        }

        /// <summary>
        /// 获取审批页面的链接
        /// </summary>
        public string GetCtrlUrlFormat(string worktaskInsId, string operatorInsId)
        {
            if (string.IsNullOrEmpty(_hfDomain) || string.IsNullOrEmpty(worktaskInsId) || string.IsNullOrEmpty(operatorInsId))
                throw new ArgumentNullException();
            var ctrlUrlFormat = string.Format("http://{0}/basepages/WorkTaskCtrlPage.aspx?WorkTaskInstanceId={1}&OperatorInstanceId={2}",
                      _hfDomain, worktaskInsId, operatorInsId);
            return ctrlUrlFormat;
        }

        /// <summary>
        /// 获取流程图的链接
        /// </summary>
        /// <param name="hfDomain"></param>
        /// <param name="workflowId"></param>
        /// <param name="workflowInsId"></param>
        /// <returns></returns>
        public string GetMapUrlFormat(string workflowId, string workflowInsId)
        {
            if (string.IsNullOrEmpty(_hfDomain) || string.IsNullOrEmpty(workflowId) || string.IsNullOrEmpty(workflowInsId))
                throw new ArgumentNullException();
            var mapUrlFormat = string.Format("http://{0}/basepages/WorkflowMap.aspx?workflowId={1}&workflowInstanceId={2}"
                                , _hfDomain, workflowId, workflowInsId);
            return mapUrlFormat;
        }

        /// <summary>
        /// 获取启动流程的链接
        /// </summary>
        /// <param name="startWfResult">启动流程对象</param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public string GetStartUrlFormat(HF_GetAllowsStartWorkFlowsPro_Result startWfResult, string controllerName, string actionName, Guid? contentId, string resourceId)
        {
            if (string.IsNullOrEmpty(_hfDomain) || startWfResult == null || string.IsNullOrEmpty(controllerName)
                || string.IsNullOrEmpty(actionName) || contentId == null || contentId == Guid.Empty || string.IsNullOrEmpty(resourceId))
                throw new ArgumentNullException();

            var startUrl = string.Format("http://{0}/BasePages/StartWorkFlow.aspx?workflowid={1}&worktaskid={2}&ct={3}&at={4}&id={5}&rcId={6}",
                _hfDomain, startWfResult.WorkFlowId, startWfResult.WorkTaskid, controllerName, actionName, contentId, resourceId);
            return startUrl;
        }

        /// <summary>
        /// 获取启动流程的链接
        /// </summary>
        /// <param name="hfDomain"></param>
        /// <param name="workflowId"></param>
        /// <param name="worktaskId"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public string GetStartUrlFormat(string workflowId, string worktaskId, string controllerName, string actionName, string contentId, string resourceId)
        {
            if (string.IsNullOrEmpty(_hfDomain) || string.IsNullOrEmpty(workflowId) || string.IsNullOrEmpty(worktaskId) || string.IsNullOrEmpty(controllerName)
                || string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(contentId) || string.IsNullOrEmpty(resourceId))
                throw new ArgumentNullException();

            var startUrl = string.Format("http://{0}/BasePages/StartWorkFlow.aspx?workflowid={1}&worktaskid={2}&ct={3}&at={4}&id={5}&rcId={6}",
                _hfDomain, workflowId, worktaskId, controllerName, actionName, contentId, resourceId);

            return startUrl;
        }
        #endregion

        #region Other Function

        /// <summary>
        /// 通过优先级值获取优先级的表示方式
        /// </summary>
        /// <param name="priorityNum"></param>
        /// <returns></returns>
        public string GetPriorityByNum(string priorityNum)
        {
            var result = string.Empty;
            switch (priorityNum)
            {
                case "1":
                    result = "特急";
                    break;
                case "2":
                    result = "紧急";
                    break;
                case "3":
                    result = "普通";
                    break;
                default:
                    result = "有误";
                    break;
            }
            return result;
        }
        /// <summary>
        /// 获取当前节点实例的描述信息
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public string GetCurrentTaskInstanceCaption(Guid? contentId)
        {
            var msg = string.Empty;

            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == contentId);
            if (audit != null)
            {
                var wtInsView = _wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == audit.workflowInsId && (u.Status == "1" || u.Status == "2" || u.Status == "4" || u.Status == "5"));
                if (wtInsView != null)
                {
                    msg = wtInsView.TaskInstanceCaption;
                }
            }
            return msg;
        }


        public string GetCurrentTask(Guid? contentId)
        {
            var currentMsg = string.Empty;
            WorkflowStatusType wfST = GetWorkflowInsStatus(contentId);
            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == contentId);
            if (wfST == WorkflowStatusType.Success)
            {
                currentMsg = "流程正常结束";
            }
            else if (wfST == WorkflowStatusType.Back)
            {
                var backWtInsView = _wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == audit.workflowInsId && u.Status == "4");
                if (backWtInsView != null)
                {
                    currentMsg = backWtInsView.OperatedDes;
                }
            }
            else if (wfST == WorkflowStatusType.Aduiting)
            {
                var auditingWtInsView = _wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == audit.workflowInsId && (u.Status == "2" || u.Status == "1" || u.Status == "5"));
                if (auditingWtInsView != null)
                {
                    if (auditingWtInsView.OperType == 10)
                    {
                        currentMsg = auditingWtInsView.OperatedDes;
                    }
                    else
                    {
                        currentMsg = auditingWtInsView.TaskCaption;
                    }
                }
            }
            else if (wfST == WorkflowStatusType.NoSubmit)
            {
                currentMsg = "未启动";
            }
            return currentMsg;
        }
        /// <summary>
        /// 获取流程启动者
        /// </summary>
        /// <param name="workTaskInstanceViewBll">bll</param>
        /// <param name="workflowInsId">流程实例</param>
        /// <returns></returns>
        public string GetWorkFlowStarter(Guid? contentId)
        {
            var starter = string.Empty;
            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == contentId);
            if (audit != null)
            {
                var workTaskIns = _wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == audit.workflowInsId && u.TaskTypeId == "1");
                if (workTaskIns != null)
                {
                    starter = workTaskIns.UserName;
                }
                else
                {
                    starter = "未发现流程启动者，请联系管理员！";
                }
            }
            else
            {
                starter = "流程未启动";
            }
            return starter;
        }

        public string GetWorkFlowStarter(string workflowInsId)
        {
            var starter = string.Empty;
            var workTaskIns = _wtInsViewBll.All.FirstOrDefault(u => u.WorkFlowInstanceId == workflowInsId && u.TaskTypeId == "1");
            if (workTaskIns != null)
            {
                starter = workTaskIns.UserName;
            }
            else
            {
                starter = "未发现流程启动者，请联系管理员！";
            }
            return starter;
        }

        /// <summary>
        /// 通过taskid获取当前的任务模板标题
        /// </summary>
        /// <param name="worktaskId"></param>
        /// <returns></returns>
        public string GetTaskCaptionByTaskId(string worktaskId)
        {
            string taskCaption = string.Empty;
            var wt = _wtBll.All.SingleOrDefault(u => u.WorkTaskId == worktaskId);
            if (wt != null)
                taskCaption = wt.TaskCaption;
            return taskCaption;
        }
        /// <summary>
        ///通过内容ID以及人物节点标题从而获得该节点的审批人员
        /// </summary>
        /// <param name="ContentID">内容ID</param>
        /// <param name="taskCaption">任务节点标题</param>
        /// <returns></returns>
        public string GetUserIdByContentIDAndTaskCaption(Guid? ContentID, string taskCaption)
        {
            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == ContentID);
            var wtInstances = _wtInsViewBll.All.Where(u => u.WorkFlowInstanceId == audit.workflowInsId &&
                (string.Compare(u.TaskCaption, taskCaption, true) == 0 || u.TaskCaption.Contains(taskCaption)));
            if (wtInstances.Count() == 1)
            {
                return wtInstances.First().UserId;
            }
            else
            {
                throw new ArgumentException("具有多个匹配项，你检查输出参数!");
            }
        }


        public bool DeleteWorkflowInstanceByContentId(Guid? contentId)
        {
            var operationInsBll = new BaseBll<HF_OperatorInstance>();
            var wtInsBll = new BaseBll<HF_WorkTaskInstance>();
            var wfInsBll = new BaseBll<HF_WorkFlowInstance>();
            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == contentId);
            if (audit != null)
            {
                var opertionInstances = operationInsBll.All.Where(u => u.WorkFlowInstanceId == audit.workflowInsId);
                var wtInstances = wtInsBll.All.Where(u => u.WorkFlowInstanceId == audit.workflowInsId);
                var wfInstances = wfInsBll.All.Where(u => u.WorkFlowInstanceId == audit.workflowInsId);
                if (opertionInstances.Any())
                {
                    operationInsBll.Delete(opertionInstances, false);
                }
                if (wtInstances.Any())
                {
                    wtInsBll.Delete(wtInstances, false);
                }
                if (wfInstances.Any())
                {
                    wfInsBll.Delete(wfInstances, false);
                }

                _auditBll.Delete(audit);
                _auditBll.UpdateChanges();
            }
            return true;

        }
        #endregion


        #region Collection
        /// <summary>
        ///通过UserId获取该用户未操作的工作流节点
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <returns></returns>
        public IQueryable<HF_WorkTaskInstanceViewDTO> GetUnOperateWorkTasksByUserId(string userId)
        {
            var operContentViews = _operContentViewBll.All.Where(u => u.UserId == userId).Select(u => u.operContent);
            //这个和下面的一个合起来是未认领的
            var workTaskInsViews = _wtInsViewBll.All.Where(u => (operContentViews.Contains(u.OperContent) || u.OperContent == "ALL")
                     && u.OperStatus == "0" && (u.Status == "1" || u.Status == "5")
                     && u.flStatus != "4" && u.flStatus != "5").Select(u => new HF_WorkTaskInstanceViewDTO()
                     {
                         #region 赋值
                         Priority = u.Priority,
                         WorkFlowNo = u.WorkFlowNo,
                         taskStartTime = u.taskStartTime,
                         TaskInstanceCaption = u.TaskInstanceCaption,
                         FlowInstanceCaption = u.FlowInstanceCaption,
                         OperContent = u.OperContent,
                         FlowCaption = u.FlowCaption,
                         TaskCaption = u.TaskCaption,
                         UserId = u.UserId,
                         OperatorInstanceId = u.OperatorInstanceId,
                         WorkFlowId = u.WorkFlowId,
                         WorkTaskId = u.WorkTaskId,
                         WorkFlowInstanceId = u.WorkFlowInstanceId,
                         WorkTaskInstanceId = u.WorkTaskInstanceId,
                         OperType = u.OperType,
                         TaskTypeId = u.TaskTypeId,
                         IsSubWorkFlow = u.IsSubWorkFlow,
                         MainWorkTaskId = u.MainWorkTaskId,
                         MainWorkFlowInstanceId = u.MainWorkFlowInstanceId,
                         OperatedDes = u.OperatedDes,
                         OperDateTime = u.OperDateTime,
                         taskEndTime = u.taskEndTime,
                         flowStartTime = u.flowStartTime,
                         flowEndTime = u.flowEndTime,
                         pOperatedDes = u.pOperatedDes,
                         Description = u.Description,
                         OperStatus = u.OperStatus,
                         Status = u.Status,
                         flStatus = u.flStatus,
                         PreviousTaskInstanceId = u.PreviousTaskInstanceId,
                         OperContentText = u.OperContentText,
                         taskInsType = u.taskInsType,
                         SuccessMsg = u.SuccessMsg,
                         ChangeOperator = u.ChangeOperator,
                         TaskInsDescription = u.TaskInsDescription,
                         NowTaskId = u.NowTaskId,
                         UserName = u.UserName,
                         IsJumpSelf = u.IsJumpSelf,
                         IsDebug = u.IsDebug
                         #endregion
                     });

            var accreditInsViews = _accreditInsViewBll.All.Where(u => u.AccreditToUserId == userId
                && u.AccreditStatus == "1" && (u.Status == "1" || u.Status == "5")
                && u.flStatus != "4" && u.flStatus != "5").Select(u => new HF_WorkTaskInstanceViewDTO()
                {
                    #region 赋值
                    Priority = u.Priority,
                    WorkFlowNo = u.WorkFlowNo,
                    taskStartTime = u.taskStartTime,
                    TaskInstanceCaption = u.TaskInstanceCaption,
                    FlowInstanceCaption = u.FlowInstanceCaption,
                    OperContent = u.OperContent,
                    FlowCaption = u.FlowCaption,
                    TaskCaption = u.TaskCaption,
                    UserId = u.UserId,
                    OperatorInstanceId = u.OperatorInstanceId,
                    WorkFlowId = u.WorkFlowId,
                    WorkTaskId = u.WorkTaskId,
                    WorkFlowInstanceId = u.WorkFlowInstanceId,
                    WorkTaskInstanceId = u.WorkTaskInstanceId,
                    OperType = u.OperType,
                    TaskTypeId = u.TaskTypeId,
                    IsSubWorkFlow = u.IsSubWorkFlow,
                    MainWorkTaskId = u.MainWorkTaskId,
                    MainWorkFlowInstanceId = u.MainWorkFlowInstanceId,
                    OperatedDes = u.OperatedDes,
                    OperDateTime = u.OperDateTime,
                    taskEndTime = u.taskEndTime,
                    flowStartTime = u.flowStartTime,
                    flowEndTime = u.flowEndTime,
                    pOperatedDes = u.pOperatedDes,
                    Description = u.Description,
                    OperStatus = u.OperStatus,
                    Status = u.Status,
                    flStatus = u.flStatus,
                    PreviousTaskInstanceId = u.PreviousTaskInstanceId,
                    OperContentText = u.OperContentText,
                    taskInsType = u.taskInsType,
                    SuccessMsg = u.SuccessMsg,
                    ChangeOperator = u.ChangeOperator,
                    TaskInsDescription = u.TaskInsDescription,
                    NowTaskId = null,
                    UserName = null,
                    IsJumpSelf = null,
                    IsDebug = u.IsDebug
                    #endregion
                });
            //已认领的
            var claimedInsViews = _wtInsViewBll.All.Where(u => u.UserId == userId
                && (u.Status == "2" || u.Status == "4")
                && u.flStatus != "4" && u.flStatus != "5").Select(u => new HF_WorkTaskInstanceViewDTO()
                {
                    #region 赋值
                    Priority = u.Priority,
                    WorkFlowNo = u.WorkFlowNo,
                    taskStartTime = u.taskStartTime,
                    TaskInstanceCaption = u.TaskInstanceCaption,
                    FlowInstanceCaption = u.FlowInstanceCaption,
                    OperContent = u.OperContent,
                    FlowCaption = u.FlowCaption,
                    TaskCaption = u.TaskCaption,
                    UserId = u.UserId,
                    OperatorInstanceId = u.OperatorInstanceId,
                    WorkFlowId = u.WorkFlowId,
                    WorkTaskId = u.WorkTaskId,
                    WorkFlowInstanceId = u.WorkFlowInstanceId,
                    WorkTaskInstanceId = u.WorkTaskInstanceId,
                    OperType = u.OperType,
                    TaskTypeId = u.TaskTypeId,
                    IsSubWorkFlow = u.IsSubWorkFlow,
                    MainWorkTaskId = u.MainWorkTaskId,
                    MainWorkFlowInstanceId = u.MainWorkFlowInstanceId,
                    OperatedDes = u.OperatedDes,
                    OperDateTime = u.OperDateTime,
                    taskEndTime = u.taskEndTime,
                    flowStartTime = u.flowStartTime,
                    flowEndTime = u.flowEndTime,
                    pOperatedDes = u.pOperatedDes,
                    Description = u.Description,
                    OperStatus = u.OperStatus,
                    Status = u.Status,
                    flStatus = u.flStatus,
                    PreviousTaskInstanceId = u.PreviousTaskInstanceId,
                    OperContentText = u.OperContentText,
                    taskInsType = u.taskInsType,
                    SuccessMsg = u.SuccessMsg,
                    ChangeOperator = u.ChangeOperator,
                    TaskInsDescription = u.TaskInsDescription,
                    NowTaskId = u.NowTaskId,
                    UserName = u.UserName,
                    IsJumpSelf = u.IsJumpSelf,
                    IsDebug = u.IsDebug
                    #endregion
                });
            //未处理的流程的集合
            var unOperateWorkTasks = workTaskInsViews.Union(accreditInsViews).Union(claimedInsViews);
            return unOperateWorkTasks;
        }
        /// <summary>
        /// 通过UserId获取该用户申请的工作流节点
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<HF_WorkTaskInstanceView> GetStartWorkTasksByUserId(string userId)
        {
            return _wtInsViewBll.All.Where(u => u.UserId == userId && u.Status == "3" && u.flStatus != "4"
                 && u.flStatus != "5" && u.TaskTypeId == "1");
        }
        /// <summary>
        /// 通过UserId获取该用户的审批过的工作流节点
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<HF_WorkTaskInstanceView> GetAduitedWorkTasksByUserId(string userId)
        {
            var aduitWorktaskIns = _wtInsViewBll.All.Where(u => u.UserId == userId && u.Status == "3"
                  && u.flStatus != "4" && u.flStatus != "5" && u.TaskTypeId != "1");
            return aduitWorktaskIns;
        }
        /// <summary>
        /// 通过UserId获取该用户的所有的工作流节点信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<HF_WorkTaskInstanceView> GetAllWorkTasksByUserId(string userId)
        {
            return _wtInsViewBll.All.Where(u => u.UserId == userId);
        }
        /// <summary>
        /// 通过内容id读取审批信息集合
        /// </summary>
        /// <param name="contentID"></param>
        /// <returns></returns>
        public IQueryable<HF_CommAudit> GetAuditMessagesByContentID(Guid? contentID)
        {
            var auditMsgBll = new BaseBll<HF_CommAudit>();
            var audit = _auditBll.All.SingleOrDefault(u => u.ContentID == contentID);
            if (audit == null)
                return auditMsgBll.All.OrderBy(u => u.AuditTime).Take(0);
            return auditMsgBll.All.Where(u => u.WorkFlowInstanceId == audit.workflowInsId).OrderBy(u => u.AuditTime);
        }

        #endregion

        #region Static Function
        /// <summary>
        /// 通过工作流状态，得到审批信息
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetMessageByWorkflowStatus(WorkflowStatusType status)
        {
            var statusMessage = string.Empty;
            object[] attrs = typeof(WorkflowStatusType).GetField(status.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs.Length > 0)
            {
                var descriptionAttr = (DescriptionAttribute)attrs[0];
                statusMessage = descriptionAttr.Description;
            }
            return statusMessage;
        }
        /// <summary>
        /// 获取工作流状态的字典 key表示中文汉字，value表示审批状态的值
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, int> GetWorkflowStatusDic()
        {
            var dic = new Dictionary<string, int>();
            dic.Add("未提交", (int)WorkflowStatusType.NoSubmit);
            dic.Add("审批中", (int)WorkflowStatusType.Aduiting);
            dic.Add("被退回", (int)WorkflowStatusType.Back);
            dic.Add("审批通过", (int)WorkflowStatusType.Success);
            return dic;
        }
        #endregion
    }
}
