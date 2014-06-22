using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Bll;
using System.Web.Mvc;
using FastReflectionLib;
using HOHO18.Common.Model;
using MorSun.Common.Privelege;
using MorSun.Controllers.ViewModel;
using MorSun.Model;
using HOHO18.Common.Web;
using MorSun.Controllers.Filter;
using System.Reflection;
namespace MorSun.Controllers
{
    /// <summary>
    /// 基于mvc原有的一个页面两个处理ActionResult（一个显示和一个提交的ActionResult）
    /// </summary>
    public class BaseMvcController<T> : BasisController
         where T : class,new()
    {
        #region Field

        private BaseBll<T> _bll;

        #endregion



        #region Property

        protected virtual string ResourceId { get; set; }

        /// <summary>
        /// 业务逻辑层对象
        /// </summary>
        protected virtual BaseBll<T> Bll
        {
            get
            {
                return new BaseBll<T>();
            }
        }

        /// <summary>
        /// 是ajax提交
        /// </summary>
        protected bool IsAjax
        {
            get
            {
                return Request.IsAjaxRequest();
            }
        }
        /// <summary>
        /// 当前的控制器名称
        /// </summary>
        protected string ControllerName
        {
            get
            {
                return RouteData.Values["controller"].ToString();
            }
        }

        #region 权限
        /// <summary>
        /// 当前用户能进行的操作（如删除，编辑，查看等等）
        /// </summary>
        public virtual OperatePrivilege CanDoSth
        {
            get
            {
                var _canDoSth = new OperatePrivilege();
                if (string.IsNullOrEmpty(this.ResourceId))
                {
                    #region 权限默认开关，开发时,到发布版本的时候，需要统一添加资源和权限
                    _canDoSth.CanRead = true;
                    _canDoSth.CanCreate = true;
                    _canDoSth.CanEdit = true;
                    _canDoSth.CanFlagDelete = true;
                    _canDoSth.CanDelete = true;
                    _canDoSth.CanArrange = true;
                    _canDoSth.CanRevoke = true;
                    _canDoSth.CanConfirm = true;
                    _canDoSth.CanRecycle = true;
                    _canDoSth.CanOrder = true;
                    _canDoSth.CanGenerate = true;
                    _canDoSth.CanImport = true;
                    //_canDoSth.CanUnLock = true;
                    _canDoSth.CanConfigure = true;
                    _canDoSth.CanFeedback = true;
                    #endregion
                }
                else
                {
                    IList<wmfRolePrivilegesView> sessionPrivilegeList = BasisController.getSessionPrivileges();
                    var resourcePrivs = sessionPrivilegeList.Where(u => string.Compare(u.ResourcesId, this.ResourceId, true) == 0);
                    foreach (var privilgeItem in resourcePrivs)
                    {
                        if (string.Compare(privilgeItem.OperationId, 操作.查看, true) == 0)
                            _canDoSth.CanRead = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.添加, true) == 0)
                            _canDoSth.CanCreate = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.修改, true) == 0)
                            _canDoSth.CanEdit = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.删除, true) == 0)
                            _canDoSth.CanFlagDelete = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.彻底删除, true) == 0)
                            _canDoSth.CanDelete = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.安排, true) == 0)
                            _canDoSth.CanArrange = true;

                        //if (string.Compare(privilgeItem.operationId, 操作.撤回, true) == 0)
                        //    _canDoSth.CanRevoke = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.确认, true) == 0)
                            _canDoSth.CanConfirm = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.回收站, true) == 0)
                            _canDoSth.CanRecycle = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.排序, true) == 0)
                            _canDoSth.CanOrder = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.生成, true) == 0)
                            _canDoSth.CanGenerate = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.导入, true) == 0)
                            _canDoSth.CanImport = true;

                        //if (string.Compare(privilgeItem.operationId, 操作.解锁, true) == 0)
                        //    _canDoSth.CanUnLock = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.配置, true) == 0)
                            _canDoSth.CanConfigure = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.反馈, true) == 0)
                            _canDoSth.CanFeedback = true;

                        if (string.Compare(privilgeItem.OperationId, 操作.维修, true) == 0)
                            _canDoSth.CanMaintain = true;
                    }
                }
                return _canDoSth;
            }
        }
        /// <summary>
        /// 是操作管理员
        /// </summary>
        public virtual bool IsAdmin
        {
            get
            {
                var privilegeList = BasisController.getSessionPrivileges();
                return privilegeList.Any(u => string.Compare(u.OperationId, 操作.系统管理员, true) == 0
                    && string.Compare(u.ResourcesId, 资源.操作范围, true) == 0);
            }
        }

        #endregion


        #endregion

        #region Function

        protected IEnumerable<ModelStateErrorMessage> GetErrorMessagesByModelState()
        {
            var list = new List<ModelStateErrorMessage>();

            list = ModelState.Where(u => u.Value.Errors.Any()).Select(u => new ModelStateErrorMessage() { Key = u.Key, ErrorMessages = u.Value.Errors.Select(e => e.ErrorMessage) }).ToList();

            return list;
        }

        /// <summary>
        /// 获取对象的ID
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected Guid? getModelID(T t)
        {
            dynamic dynamicT = t;
            return dynamicT.ID;
        }

        /// <summary>
        /// 默认初始化ID,RegTime，ModTime，FlagDeleted，FlagTrashed字段
        /// </summary>
        /// <param name="t"></param>
        protected virtual void InitObject(T t)
        {
            dynamic dyT = t;
            //dyT.ID = Guid.NewGuid();



            try
            {
                dyT.RegTime = DateTime.Now;
            }
            catch
            { }
            try
            {
                dyT.ModTime = DateTime.Now;
            }
            catch
            { }
            try
            {
                dyT.FlagDeleted = false;
            }
            catch
            { }
            try
            {
                dyT.FlagTrashed = false;
            }
            catch
            { }

        }


        /// <summary>
        /// 通过流程标题获取启动流程对象
        /// </summary>
        /// <param name="flowCaption"></param>
        /// <returns></returns>
        protected HF_GetAllowsStartWorkFlowsPro_Result getStartWorkflowProResult(string flowCaption)
        {
            var startWF = Bll.Db.HF_GetAllowsStartWorkFlowsPro(UserID.ToString()).FirstOrDefault(u => u.FlowCaption.Contains(flowCaption));
            return startWF;
        }
        /// <summary>
        /// 当是ajax提交的时候返回json，否则就直接跳转到指定的url
        /// </summary>
        /// <param name="msg">存储返回信息的对象</param>
        /// <returns></returns>
        protected ActionResult ReturnJsonIfAjaxOrElseReturnRedirect(ReturnMsg msg)
        {
            if (IsAjax)
            {
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Redirect(msg.ReturnUrl);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">存储返回信息的对象</param>
        /// <returns></returns>
        

        /// <summary>
        /// 当是ajax提交的时候返回json，否则返回对应ViewResult
        /// </summary>
        /// <param name="model">要返回的Model</param>
        /// <param name="rtnMsg">默认可为空</param>
        /// <returns></returns>
        protected ActionResult ReturnJsonIfAjaxOrElseReturnView(object model, ReturnMsg rtnMsg = null)
        {
            if (IsAjax)
            {
                if (rtnMsg == null)
                {
                    rtnMsg = new ReturnMsg() { AppendData = GetErrorMessagesByModelState(), Message = "操作失败", Result = false };
                }
                else
                {
                    rtnMsg.AppendData = GetErrorMessagesByModelState();
                }
                return Json(rtnMsg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View(model);
            }
        }

        /// <summary>
        /// 当returnUrl为空的时候，直接格式化ReturnUrl到默认/controllername/actionName
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName">默认为当前的controllerName</param>
        /// <returns></returns>
        protected string GetDefaultReturnUrlIfNull(string returnUrl, string actionName = "", string controllerName = "")
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                controllerName = this.ControllerName;
            }
            if (string.IsNullOrEmpty(actionName))
            {
                actionName = "Index";
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                //默认跳转到来路页面 edit by timfeng 2017-6-8
                returnUrl = Request.UrlReferrer.ToString();
                //returnUrl = string.Format("/{0}/{1}", controllerName, actionName);
            }
            return returnUrl;
        }

        #endregion


        #region ACTION ADD BY TIMFENG 2014-2-25

        //删除到回收站
        [Authorize]
        [ExceptionFilter()]
        public virtual string DelToRecycle(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.删除))
            {
                return NDelToRecycle(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        /// <summary>
        /// 删除到回收站
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual string NDelToRecycle(T t, Func<T, string> ck = null)
        {
            T model = Bll.GetModel(t);
            var ckRs = ck(model);
            if (!ckRs.IsWhite() && !ckRs.Eql("true"))
            {
                return ckRs;
            }

            dynamic dynamicT = model;

            dynamicT.FlagTrashed = true;
            Bll.Update(dynamicT);
            //插入操作日志
            InsertLog(dynamicT.ID, t.GetType().Name, HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("删除到回收站"), "", "");
            return "true";
        }


        #endregion
    }

}
