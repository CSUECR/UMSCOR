using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using HOHO18.Common;
using HOHO18.Common.ExHelp;
using MorSun.Bll;
using MorSun.Controllers.Filter;
using MorSun.Model;
using MorSun.Controllers.ViewModel;
using System.Reflection;
using FastReflectionLib;
using System.Data.Objects.DataClasses;
using MorSun.Common;
using MorSun.Common.Privelege;

namespace MorSun.Controllers
{
    public class BaseController<T> : BasisController
        where T : class ,new()
    {
        protected virtual string ResourceId
        {
            get;
            set;
        }

        public BaseController()
        {
            //ViewBag.CanDoSth = CanDoSth;
            //ViewBag.IsAdmin = IsAdmin;
        }

        #region 获取默认业务逻辑对象
        BaseBll<T> _bll;
        /// <summary>
        /// 获取默认业务逻辑对象
        /// </summary>
        protected virtual BaseBll<T> Bll
        {
            get
            {
                _bll = _bll.Load();
                return _bll;
            }
            set { _bll = value; }
        }
        #endregion     

        #region 添加

        #region 显示添加页面
        /// <summary>
        /// 显示添加页面
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult Add(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
            {
                ViewBag.canDoSth = this.CanDoSth;
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        #endregion


        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        public virtual String Create(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
                return NCreate(t);
            else
                return "";//getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <param name="onPre"></param>
        /// <returns></returns>
        protected virtual String NCreate(T t, Func<T, string> onPre = null)
        {

            if (onPre == null)
            {
                onPre = OnPreCreateCK;
            }

            //添加初始化字段
            CreateInitObject(t);

            dynamic v = t;
            if (v.IsValid)
            {
                var prers = onPre(t);
                if (prers.IsWhite() || prers.Eql("true"))
                {
                    var result = NInsert(t);
                    if (result == null)
                    {
                        return "";//getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "查询失败"), "") });
                    }
                    else
                    {
                        InsertLog(GetLinkID(t), GetOperateTable(t), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("添加"), "", "");
                        return "true";
                    }

                }
                else
                    return prers;
            }
            else
            {
                return "";// getErrListJson(v.GetRuleViolations());
            }

        }

        /// <summary>
        /// 添加记录的具体实现。
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected virtual T NInsert(T t)
        {
            var result = Bll.Insert(t);
            return result;
        }

        /// <summary>
        /// 访问页面记录
        /// </summary>
        /// <param name="pageBrowse"></param>
        /// <returns></returns>
        public virtual string Visit(wmfPageBrowse pageBrowse)
        {
            InsertVisitInfo(pageBrowse);
            return "";
        }
        #endregion



        #endregion

        #region 删除

        #region 直接删除
        /// <summary>
        /// 直接删除
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual string Delete(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.删除))
                return NDelete(t);
            else
                return "";// getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });

        }

        /// <summary>
        /// 直接删除
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ck"></param>
        /// <returns></returns>
        protected virtual string NDelete(T t, Func<T, string> ck = null)
        {

            ck = ck.Load(() => OnDelCk);

            var result = "false";
            var item = Bll.GetModel(t);
            if (item != null)
            {
                var ckRs = ck(t);
                if (!ckRs.IsWhite() && !ckRs.Eql("true"))
                {
                    return ckRs;
                }
                Bll.Delete(item);
                InsertLog(null, GetOperateTable(t), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("删除"), "", "");
                result = "true";
            }
            return result;
        }
        #endregion
     


        

        #region 查看页面
        /// <summary>
        /// 显示编辑页面
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult SeeView(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.查看))
            {
                ViewBag.canDoSth = this.CanDoSth;
                ViewBag.IsAdmin = IsAdmin;
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }
        #endregion

        #region 编辑

        #region 显示编辑页面
        /// <summary>
        /// 显示编辑页面
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        public virtual ActionResult Update(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.修改))
            {
                ViewBag.canDoSth = this.CanDoSth;
                var item = SetEntity(t);
                ViewBag.IsAdmin = IsAdmin;
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }

        }
        #endregion

        #region 编辑
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        public virtual String Edit(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.修改))
            {
                return NEdit(t);
            }
            else
            {
                return "";// getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        /// <summary>
        /// 编辑通用
        /// </summary>
        /// <param name="t">实体类</param>
        /// <param name="ck">编辑前的验证</param>
        /// <returns></returns>
        protected string NEdit(T t, Func<T, string> ck = null)
        {
            if (ck == null)
            {
                ck = OnEditCK;
            }

            //编辑前附值
            EditInitObject(t);

            var result = "false";
            var model = Bll.GetModel(t);
            if (model == null)
            {
                return "";// getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "编辑失败"), "") });
            }
            //日志记录，去掉
            //var originalContent = string.Empty;
            //var afterOperateContent = string.Empty;
            //var msg = string.Empty;
            //msg = t.Equal(model, out originalContent, out afterOperateContent);
            //if (msg == "true")
            //{
            //    InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("编辑"), originalContent, afterOperateContent);
            //}

            TryUpdateModel(model);
            if (model.AsDy().IsValid)
            {
                var ckRs = ck(model);
                if (!ckRs.IsWhite() && !ckRs.Eql("true"))
                {
                    return ckRs;
                }
                Bll.Update(model);
                result = "true";
            }
            else
            {
                result = "";// getErrListJson(model.AsDy().GetRuleViolations());
            }

            return result;
        }

        /// <summary>
        /// 读取记录信息
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public virtual ActionResult GetEdit(string id, T t)
        {
            var model = Bll.GetModel(t);
            if (model == null)
            {
                return Json(null);
            }
            var js = JsHelper.Json(model);

            return Json("[" + js + "]");
        }
        #endregion



        #endregion
        #endregion
        #region 查询

        #region 详细页面
        /// <summary>
        /// 详细页面
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        //[Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Detail(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.查看))
            {
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }
        #endregion

        #region 更新实体
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual T SetEntity(T t)
        {
            var item = Bll.GetModel(t);
            item = item ?? t;
            TryUpdateModel(item);
            return item;
        }
        #endregion           

        #region 获取对象的ID
        /// <summary>
        /// 获取对象的
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public Guid? GetLinkID(T t)
        {
            Guid ID = Guid.Empty;
            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "ID")
                {
                    if (item.GetValue(t, null) != null)
                    {
                        ID = Guid.Parse(item.GetValue(t, null).ToString());
                    }
                }
            }
            return ID;
        }
        #endregion

        #region 获取对象的表名
        /// <summary>
        /// 获取对象的表名
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public string GetOperateTable(T t)
        {
            return t.GetType().Name;
        }
        #endregion        

        #region 获取对应ViewModel的类

        Type _vModelType;

        /// <summary>
        /// 获取对应ViewModel的类
        /// </summary>
        public virtual Type VModelType
        {
            get
            {
                _vModelType = _vModelType.Load(() =>
                {
                    //all types
                    var types = typeof(BaseVModel<T>).Assembly.GetTypes();
                    //获取视图模型
                    var vModelType = types.FirstOrDefault(t => t.BaseType == typeof(BaseVModel<T>));

                    if (vModelType == null)
                    {
                        vModelType = typeof(BaseVModel<T>);
                    }

                    return vModelType;
                });

                return _vModelType;
            }
            set { _vModelType = value; }
        }
        #endregion

        #region 填充实体
        /// <summary>
        /// 填充实体
        /// </summary>
        /// <param name="model">实体类</param>
        protected virtual void FillModel(object model)
        {
            var methodObj = false.MethodOf<object>(UpdateModel);
            var methodBase = methodObj.GetGenericMethodDefinition();
            var method = methodBase.MakeGenericMethod(model.GetType());
            method.FastInvoke(this, model);
        }
        #endregion        
         
        #region 回收站页面
        /// <summary>
        /// 回收站页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Recycle()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.回收站))
            {
                var vModel = VModelType.New();
                FillModel(vModel);

                if (vModel is BaseVModel<T>)
                {
                    var vm = vModel as BaseVModel<T>;
                    vm.DoSth();
                }

                return View(vModel);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }
        #endregion

        #region 首页面
        /// <summary>
        /// 首页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Index()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.查看))
            {
                var vModel = VModelType.New();
                FillModel(vModel);
                if (vModel is BaseVModel<T>)
                {
                    var vm = vModel as BaseVModel<T>;
                    vm.DoSth();
                }
                ViewBag.canDoSth = this.CanDoSth;
                return View(vModel);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }
        #endregion

        #endregion

        #region 验证

        #region 在编辑前出发的验证
        /// <summary>
        /// 在编辑前出发的验证
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual string OnEditCK(T t)
        {
            return null;
        }
        #endregion

        #region 添加之前验证
        /// <summary>
        /// 添加之前验证
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual string OnPreCreateCK(T t)
        {
            return "";
        }
        #endregion

        #region 在删除时触发的验证
        /// <summary>
        /// 在删除时触发的验证
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual string OnDelCk(T t)
        {
            return null;
        }
        #endregion                

        #endregion

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual string DelImg(Guid id)
        {
            var uploadfile = new BaseBll<wmfUploadFile>();
            var model = uploadfile.GetModel(id);
            uploadfile.Delete(model, true);
            return "true";
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 添加默认初始化RegTime，RegUser，FlagDeleted，FlagTrashed字段
        /// </summary>
        /// <param name="t"></param>
        protected virtual void CreateInitObject(T t)
        {
            dynamic dyT = t;
            try
            {
                dyT.RegTime = dyT.RegTime ?? DateTime.Now;
            }
            catch
            { }
            try
            {
                dyT.RegUser = dyT.RegUser ?? UserID;
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
        /// 修改默认初始化RegTime，RegUser，FlagDeleted，FlagTrashed字段
        /// </summary>
        /// <param name="t"></param>
        protected virtual void EditInitObject(T t)
        {
            dynamic dyT = t;

            try
            {
                dyT.ModTime = DateTime.Now;
            }
            catch
            { }

            try
            {
                dyT.ModUser = UserID;
            }
            catch
            { }
        }
        #endregion


        #region 权限统一标识
        /// <summary>
        /// 当前用户能进行的操作（如删除，编辑，查看等等）
        /// </summary>
        public virtual OperatePrivilege CanDoSth
        {
            get
            {
                var _canDoSth = new OperatePrivilege();
                if (this.ResourceId == null)
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
                    _canDoSth.CanAudit = true;
                    #endregion
                }
                else
                {
                    IList<wmfRolePrivilegesView> sessionPrivilegeList = BasisController.getSessionPrivileges();
                    foreach (var privilgeItem in sessionPrivilegeList.Where(u =>string.Compare(u.ResourcesId,this.ResourceId,true)==0))
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
                        if (string.Compare(privilgeItem.OperationId, 操作.审核, true) == 0)
                            _canDoSth.CanAudit = true;
                        if (string.Compare(privilgeItem.OperationId, 操作.打印, true) == 0)
                            _canDoSth.CanPrint = true;

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

        

    }
}
