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
        #region ��������
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
        /// ��ȡĬ��ҵ���߼�����
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
        /// ����ʵ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual T SetEntity(T t)
        {
            var item = Bll.GetModel(t);
            item = item ?? t;
            TryUpdateModel(item);
            return item;
        }  
        
        /// <summary>
        /// ��ȡ�����
        /// </summary>
        /// <param name="t">ʵ����</param>
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
        /// ��ȡ����ı���
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public string GetOperateTable(T t)
        {
            return t.GetType().Name;
        }       

        Type _vModelType;

        /// <summary>
        /// ��ȡ��ӦViewModel����
        /// </summary>
        public virtual Type VModelType
        {
            get
            {
                _vModelType = _vModelType.Load(() =>
                {
                    //all types
                    var types = typeof(BaseVModel<T>).Assembly.GetTypes();
                    //��ȡ��ͼģ��
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
        /// ���ʵ��
        /// </summary>
        /// <param name="model">ʵ����</param>
        protected virtual void FillModel(object model)
        {
            var methodObj = false.MethodOf<object>(UpdateModel);
            var methodBase = methodObj.GetGenericMethodDefinition();
            var method = methodBase.MakeGenericMethod(model.GetType());
            method.FastInvoke(this, model);
        }    
        #endregion

        #region ��������
        #region ���
        /// <summary>
        /// ��ʾ���ҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult Add(T t)
        {
            if (ResourceId.havePrivilege(����.���))
            {                
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }

        /// <summary>
        /// ���
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [ExceptionFilter()]
        public virtual ActionResult Add(T t, string returnUrl, Func<T, string> ck = null)
        {
            if (ResourceId.havePrivilege(����.���))
            {
                var oper = new OperationResult(OperationResultType.Error, "���ʧ��");
                ck = ck.Load(() => OnAddCK);
                dynamic v = t;
                var prers = ck(t);
                //ע�⣺��ModelState�ռ�����v.GetRuleViolations()�ŵ�Pre��ȥ������߶�̬��ȡ����            
                //if (!v.IsValid)
                //{
                //    ModelState.PR(v.GetRuleViolations());
                //}
                if (ModelState.IsValid)
                {
                    //��ӳ�ʼ���ֶ�
                    CreateInitObject(t);
                    var result = Bll.Insert(t);
                    if (result == null)
                    {
                        "".AE("���ʧ��", ModelState);
                        oper.AppendData = ModelState.GE();
                    }
                    else
                    {
                        fillOperationResult(returnUrl, oper, "��ӳɹ�");
                    }
                }
                return Json(oper);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }     
        #endregion

        #region �༭
        /// <summary>
        /// ��ʾ�༭ҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]        
        [ValidateInput(false)]        
        [ExceptionFilter()]
        public virtual ActionResult Edit(T t)
        {
            if (ResourceId.havePrivilege(����.�޸�))
            {                
                var item = SetEntity(t);                
                return View(item);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }

        }


        /// <summary>
        /// �༭
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(T t, string returnUrl, Func<T, string> ck = null)
        {
            if (ResourceId.havePrivilege(����.�޸�))
            {
                var oper = new OperationResult(OperationResultType.Error, "�޸�ʧ��");
                //�༭ǰ��ֵ
                EditInitObject(t);
                var model = Bll.GetModel(t);
                if (model == null)
                {
                    "".AE("�޸�ʧ��", ModelState);
                }
                //��־��¼��ȥ��
                //var originalContent = string.Empty;
                //var afterOperateContent = string.Empty;
                //var msg = string.Empty;
                //msg = t.Equal(model, out originalContent, out afterOperateContent);
                //if (msg == "true")
                //{
                //    InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.XMLConfigHelp.GetWebConfigValue("�༭"), originalContent, afterOperateContent);
                //}

                TryUpdateModel(model);
                ck = ck.Load(() => OnEditCK);
                var ckRs = ck(model);
                if (ModelState.IsValid)
                {
                    Bll.Update(model);
                    fillOperationResult(returnUrl, oper, "�޸ĳɹ�");
                }
                else
                {
                    "".AE("�޸�ʧ��", ModelState);
                    oper.AppendData = ModelState.GE();
                }

                return Json(oper);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }
        #endregion

        #region ɾ��
        /// <summary>
        /// ֱ��ɾ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Delete(T t, string returnUrl, Func<T, string> ck = null)
        {
            if (ResourceId.havePrivilege(����.ɾ��))
            {
                var oper = new OperationResult(OperationResultType.Error, "ɾ��ʧ��");
                ck = ck.Load(() => OnDelCk);
                var ckRs = ck(t);
                if (ModelState.IsValid)
                {
                    var item = Bll.GetModel(t);
                    if (item != null)
                    {
                        Bll.Delete(item);
                        fillOperationResult(returnUrl, oper, "ɾ���ɹ�");
                    }
                    else
                    {
                        "".AE("ɾ��ʧ��", ModelState);
                        oper.AppendData = ModelState.GE();
                    }
                }
                else
                {
                    "".AE("���ʧ��", ModelState);
                    oper.AppendData = ModelState.GE();
                }
                return Json(oper);
            }                
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }

        }
        
        #endregion      

        #region ��ѯ
        /// <summary>
        /// ��ϸҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        //[Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Detail(T t)
        {
            if (ResourceId.havePrivilege(����.�鿴))
            {
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        }    
       
        /// <summary>
        /// ����վҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Recycle()
        {
            if (ResourceId.havePrivilege(����.����վ))
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
                return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }
        
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Index()
        {
            if (ResourceId.havePrivilege(����.�鿴))
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
                return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }
        
        #endregion
        #endregion

        #region ��֤
        /// <summary>
        /// �ڱ༭ǰ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnEditCK(T t)
        {
            return "";
        }
        
        /// <summary>
        /// ���֮ǰ��֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnAddCK(T t)
        {
            return "";
        }
        
        /// <summary>
        /// ��ɾ��ʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnDelCk(T t)
        {
            return "";
        }  
        #endregion       

        #region ��ʼ��
        /// <summary>
        /// ���Ĭ�ϳ�ʼ��RegTime��RegUser��FlagDeleted��FlagTrashed�ֶ�
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
        /// �޸�Ĭ�ϳ�ʼ��ModTime��ModUser�ֶ�
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
