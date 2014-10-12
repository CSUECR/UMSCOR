using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;
using System.Web.Mvc;
using HOHO18.Common.Web;
using MorSun.Controllers.ViewModel;
using MorSun.Bll.Service;
using MorSun.Common.Privelege;
namespace MorSun.Controllers
{
    public class AskForLeaveController : BaseMvcController<AskForLeave>
    {
        #region Field
        private BaseBll<Aduit> aduitBll = new BaseBll<Aduit>();
        private WorkflowService _wfService = new WorkflowService();
        #endregion

        #region Property

        protected override string ResourceId
        {
            get
            {
                return 资源.工作流请假;
            }
        }

        /// <summary>
        /// 请假类别集合
        /// </summary>
        public IEnumerable<wmfReference> QingJiaTypeList
        {
            get
            {
                var refGroupBll = new BaseBll<wmfRefGroup>();
                var refGroupModel = refGroupBll.All.FirstOrDefault(u => u.RefGroupName == "请假类别");
                if (refGroupModel != null)
                {
                    return refGroupModel.wmfReferences.OrderBy(u => u.Sort).ToList();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Action

        [Authorize]
        public ActionResult Index(AskForLeaveVModel vModel)
        {
            if (!CanDoSth.CanRead)
                return Content("无权限操作");
            var hfWorkTaskBll = new BaseBll<HF_WorkTask>();
            ViewBag.StartWorkFlowResult = getStartWorkflowProResult("水务请假");
            ViewBag.AllAduit = aduitBll.All.ToList();
            ViewBag.CanDoSth = CanDoSth;
            ViewBag.VModel = vModel;
            var contentIds = _wfService.GetWorkflowStatusAuditContentIds(vModel.wfStatus, ResourceId);
            var askforLeaves = Bll.All.Where(u => u.FlagDeleted != true && u.FlagTrashed != true && u.UserID == UserID)
                //过滤条件
                .Where(u => (vModel.BeginDate != null && u.RegTime > vModel.BeginDate) || vModel.BeginDate == null)
                .Where(u => (vModel.EndDate != null && u.RegTime < vModel.EndDate) || vModel.EndDate == null)
                .WhereIf(u=> contentIds.Contains(u.ID),vModel.wfStatus!=WorkflowStatusType.NoSubmit &&vModel.wfStatus!=WorkflowStatusType.Default)
                .WhereIf(u=> !contentIds.Contains(u.ID),vModel.wfStatus==WorkflowStatusType.NoSubmit)
                .OrderByDescending(u => u.RegTime);
            var pageList = askforLeaves.ToPageList(vModel.PIndex, int.Parse(webConfigHelp.GetWebConfigValue("PageSize")));
            return View(pageList);
        }

        public ActionResult Create()
        {
            ViewBag.QingJiaTypeList = QingJiaTypeList;
            var referenceBll = new BaseBll<wmfReference>();
            var model = new AskForLeave() { UserID = UserID, UserName = User.Identity.Name };
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(AskForLeave t, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (!CanDoSth.CanCreate)
                    return Content("无权限操作");
                InitObject(t);
                t.UserID = UserID;
                var userDeptPosition = new BaseBll<wmfUserDeptPosition>().All.FirstOrDefault(u => u.UserId == UserID);
                if (userDeptPosition != null && userDeptPosition.FlagDeleted != true && userDeptPosition.FlagTrashed != true)
                {
                    t.DeptId = userDeptPosition.DeptId;
                }
                
                Bll.Insert(t);
                InsertLog(t.ID, t.GetType().Name, webConfigHelp.GetWebConfigValue("编辑"), "", t.ToPropertyString());
                if (t.IsAduit.GetValueOrDefault())
                {
                    returnUrl = _wfService.GetStartUrlFormat(getStartWorkflowProResult("水务请假"), ControllerName, "read", t.ID, ResourceId);
                }
                returnUrl = GetDefaultReturnUrlIfNull(returnUrl);
                var returnMsg = new ReturnMsg() { ReturnUrl = returnUrl, Message = "操作成功", Result = true };
                return ReturnJsonIfAjaxOrElseReturnRedirect(returnMsg);
            }
            return View(t);
        }

        public ActionResult Edit(Guid? id)
        {
            if (!CanDoSth.CanEdit)
                return Content("无权限操作");
            ViewBag.QingJiaTypeList = QingJiaTypeList;
            var model = Bll.GetModel(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(AskForLeave t, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (!CanDoSth.CanEdit)
                    return Content("无权限操作");
                var originalT = Bll.GetModel(t);
                originalT.ModTime = DateTime.Now;
                //是否点击了
                if (t.IsAduit.GetValueOrDefault())
                {
                    returnUrl = _wfService.GetStartUrlOrBeBackedUrl(t.ID,ResourceId,getStartWorkflowProResult("水务请假"), ControllerName, "read");
                }
                var originalContent = originalT.ToPropertyString();
                if (TryUpdateModel(originalT))
                {
                    Bll.UpdateChanges();
                    InsertLog(originalT.ID, t.GetType().Name, webConfigHelp.GetWebConfigValue("编辑"), originalContent,t.ToPropertyString());
                    returnUrl = GetDefaultReturnUrlIfNull(returnUrl);
                    var returnMsg = new ReturnMsg() { ReturnUrl = returnUrl, Result = true, Message = "编辑成功" };
                    return ReturnJsonIfAjaxOrElseReturnRedirect(returnMsg);
                }
                //更新不成功
                else
                {
                    ModelState.AddModelError("", "更新模型失败");
                }
            }
            return View(t);
        }

        public ActionResult Read(Guid? id)
        {
            ViewBag.QingJiaTypeList = QingJiaTypeList;
            var model = Bll.GetModel(id);
            return View(model);
        }
        #endregion
    }
}
