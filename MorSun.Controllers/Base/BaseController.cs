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
        #region 基础调用
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

        #region 基础操作
        #region 添加
        /// <summary>
        /// 显示添加页面
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult Add(T t)
        {
            if (ResourceId.havePrivilege(操作.添加))
            {                
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [ExceptionFilter()]
        public virtual ActionResult Add(T t, string returnUrl, Func<T, string> ck = null)
        {
            if (ResourceId.havePrivilege(操作.添加))
            {
                var oper = new OperationResult(OperationResultType.Error, "添加失败");
                ck = ck.Load(() => OnAddCK);
                dynamic v = t;
                var prers = ck(t);
                //注意：用ModelState收集错误，v.GetRuleViolations()放到Pre里去做，这边动态获取不了            
                //if (!v.IsValid)
                //{
                //    ModelState.PR(v.GetRuleViolations());
                //}
                if (ModelState.IsValid)
                {
                    //添加初始化字段
                    CreateInitObject(t);
                    var result = Bll.Insert(t);
                    if (result == null)
                    {
                        "".AE("添加失败", ModelState);
                        oper.AppendData = ModelState.GE();
                    }
                    else
                    {
                        fillOperationResult(returnUrl, oper, "添加成功");
                    }
                }
                return Json(oper);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }     
        #endregion

        #region 编辑
        /// <summary>
        /// 显示编辑页面
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]        
        [ValidateInput(false)]        
        [ExceptionFilter()]
        public virtual ActionResult Edit(T t)
        {
            if (ResourceId.havePrivilege(操作.修改))
            {                
                var item = SetEntity(t);                
                return View(item);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }

        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(T t, string returnUrl, Func<T, string> ck = null)
        {
            if (ResourceId.havePrivilege(操作.修改))
            {
                var oper = new OperationResult(OperationResultType.Error, "修改失败");
                //编辑前附值
                EditInitObject(t);
                var model = Bll.GetModel(t);
                if (model == null)
                {
                    "".AE("修改失败", ModelState);
                }
                //日志记录，去掉
                //var originalContent = string.Empty;
                //var afterOperateContent = string.Empty;
                //var msg = string.Empty;
                //msg = t.Equal(model, out originalContent, out afterOperateContent);
                //if (msg == "true")
                //{
                //    InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.XMLConfigHelp.GetWebConfigValue("编辑"), originalContent, afterOperateContent);
                //}

                TryUpdateModel(model);
                ck = ck.Load(() => OnEditCK);
                var ckRs = ck(model);
                if (ModelState.IsValid)
                {
                    Bll.Update(model);
                    fillOperationResult(returnUrl, oper, "修改成功");
                }
                else
                {
                    "".AE("修改失败", ModelState);
                    oper.AppendData = ModelState.GE();
                }

                return Json(oper);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 直接删除
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Delete(T t, string returnUrl, Func<T, string> ck = null)
        {
            if (ResourceId.havePrivilege(操作.删除))
            {
                var oper = new OperationResult(OperationResultType.Error, "删除失败");
                ck = ck.Load(() => OnDelCk);
                var ckRs = ck(t);
                if (ModelState.IsValid)
                {
                    var item = Bll.GetModel(t);
                    if (item != null)
                    {
                        Bll.Delete(item);
                        fillOperationResult(returnUrl, oper, "删除成功");
                    }
                    else
                    {
                        "".AE("删除失败", ModelState);
                        oper.AppendData = ModelState.GE();
                    }
                }
                else
                {
                    "".AE("添加失败", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper);
            }                
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }

        }
        
        #endregion      

        #region 查询
        /// <summary>
        /// 详细页面
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        //[Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Detail(T t)
        {
            if (ResourceId.havePrivilege(操作.查看))
            {
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                "".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }    
       
        /// <summary>
        /// 回收站页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Recycle()
        {
            if (ResourceId.havePrivilege(操作.回收站))
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
        
        /// <summary>
        /// 首页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Index()
        {
            if (ResourceId.havePrivilege(操作.查看))
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
        #endregion

        #region 验证
        /// <summary>
        /// 在编辑前出发的验证
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual string OnEditCK(T t)
        {
            return "";
        }
        
        /// <summary>
        /// 添加之前验证
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual string OnAddCK(T t)
        {
            return "";
        }
        
        /// <summary>
        /// 在删除时触发的验证
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        protected virtual string OnDelCk(T t)
        {
            return "";
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
        /// 修改默认初始化ModTime、ModUser字段
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
    }
}
