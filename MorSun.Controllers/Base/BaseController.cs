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
            var model = Bll.GetModel(t);
            model = model ?? t;
            TryUpdateModel(model);
            return model;
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
        protected string GetCheckId(T t)
        {
            string checkedId = string.Empty;
            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "CheckedId")
                {
                    if (item.GetValue(t, null) != null)
                    {
                        checkedId = item.GetValue(t, null).ToString();
                    }
                }
            }
            return checkedId;
        }

        /// <summary>
        /// ��ʾ���ҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult Add(T t,string returnUrl)
        {
            if (ResourceId.HP(����.���))
            {                
                var model = SetEntity(t);
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl;                 
                return View(model);
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
        public virtual ActionResult Create(T t, string returnUrl)
        {
            if (ResourceId.HP(����.���))
            {
                var oper = new OperationResult(OperationResultType.Error, "���ʧ��"); 
                var prers = OnAddCK(t);
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
        public virtual ActionResult Edit(T t, string returnUrl)
        {
            if (ResourceId.HP(����.�޸�))
            {                
                var model = SetEntity(t);
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl; 
                return View(model);
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
        public virtual ActionResult Update(T t, string returnUrl)
        {
            if (ResourceId.HP(����.�޸�))
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
                var ckRs = OnEditCK(model);
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
            if (ResourceId.HP(����.����ɾ��))
            {
                var oper = new OperationResult(OperationResultType.Error, "ɾ��ʧ��");
                ck = ck.Load(() => OnDelCk);
                var ckRs = ck(t);
                if (ModelState.IsValid)
                {
                    var model = Bll.GetModel(t);
                    if (model != null)
                    {
                        Bll.Delete(model);
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

        /// <summary>
        /// ����ɾ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult BatchDelete(T t, string returnUrl)
        {
            if (ResourceId.HP(����.����ɾ��))
            {
                var oper = new OperationResult(OperationResultType.Error, "����ʧ��");
                //ck = ck.Load(() => OnFTCk);
                //var ckRs = ck(t);
                //ǰ��ID��GUID����
                var ids = GetCheckId(t).ToGuidList(",");
                var errs = "";
                if (ids.Count() < 0)
                {
                    errs += "����ʧ��";
                    "".AE("����ʧ��", ModelState);
                }
                if (String.IsNullOrEmpty(errs))
                {                    
                    foreach (var id in ids)
                    {
                        var model = Bll.GetModel(id);
                        if (model != null)
                            Bll.Delete(model,false); 
                    }
                    Bll.UpdateChanges();                    
                    fillOperationResult(returnUrl, oper, "�����ɹ�");
                }
                else
                {
                    "".AE("����ʧ��", ModelState);
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

        #region ɾ��������վ��ԭ
        /// <summary>
        /// ɾ��������վ��ԭ
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult TrashList(T t, bool? flag, string returnUrl)
        {
            if (ResourceId.HP(����.ɾ��))
            {
                var oper = new OperationResult(OperationResultType.Error, "����ʧ��");
                //ck = ck.Load(() => OnFTCk);
                //var ckRs = ck(t);
                //ǰ��ID��GUID����
                var ids = GetCheckId(t).ToGuidList(",");
                var errs = "";
                if (ids.Count() < 0)
                {
                    errs += "����ʧ��";
                    "".AE("����ʧ��", ModelState);
                }
                if (String.IsNullOrEmpty(errs))
                {        
                    if(flag != null)
                    {
                        if (flag != true)
                            flag = false;
                        foreach (var id in ids)
                        {                        
                            var model = Bll.GetModel(id);
                            if (model != null)
                                SetFT(model, flag.Value);
                        }
                        Bll.UpdateChanges();
                    }
                    fillOperationResult(returnUrl, oper, "�����ɹ�");
                }
                else
                {
                    "".AE("����ʧ��", ModelState);
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

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="t"></param>
        /// <param name="sort"></param>
        private void SetFT(T t, bool flag)
        {
            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "FlagTrashed")
                {
                    item.SetValue(t, flag, null);
                }
            }
        }
        #endregion      

        #region ����
        [Authorize]
        [HttpPost]
        public virtual ActionResult SortList(T t, string returnUrl)
        {
            if (ResourceId.HP(����.�޸�))
            {
                var oper = new OperationResult(OperationResultType.Error, "����ʧ��");
                //ǰ��ID��GUID����
                var ids = GetCheckId(t).ToGuidList(",");
                var errs = "";
                if (ids.Count() < 0)
                {
                    errs += "����ʧ��";
                    "".AE("����ʧ��", ModelState);
                }
                if (String.IsNullOrEmpty(errs))
                {                    
                    int i = 0;
                    foreach (var id in ids)
                    {
                        i++;
                        var model = Bll.GetModel(id);                        
                        if (model != null)
                            SetSort(model,i);
                    }
                    Bll.UpdateChanges();
                    fillOperationResult(returnUrl, oper, "����ɹ�");
                }
                else
                {
                    "".AE("����ʧ��", ModelState);
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

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="t"></param>
        /// <param name="sort"></param>
        private void SetSort(T t,int sort)
        {
            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "Sort")
                {
                    item.SetValue(t, sort, null);                      
                }
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
        public virtual ActionResult Detail(T t,string returnUrl)
        {
            if (ResourceId.HP(����.�鿴))
            {
                var model = SetEntity(t);
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl; 
                return View(model);
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
        /// ��ϸҳ�� �������鿴ҳ�����
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        //[Authorize]
        [ExceptionFilter()]
        public virtual ActionResult SeeView(T t, string returnUrl)
        {
            if (ResourceId.HP(����.�鿴))
            {
                var model = SetEntity(t);
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl; 
                return View(model);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
            }
        } 
       
        ///// <summary>
        ///// ����վҳ��
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[ExceptionFilter()]
        //public virtual ActionResult Recycle()
        //{
        //    if (ResourceId.HP(����.����վ))
        //    {
        //        var vModel = VModelType.New();
        //        FillModel(vModel);

        //        if (vModel is BaseVModel<T>)
        //        {
        //            var vm = vModel as BaseVModel<T>;
        //            vm.DoSth();
        //        }
        //        ViewBag.RS = ResourceId;
        //        return View(vModel);
        //    }
        //    else
        //    {
        //        "".AE("��Ȩ��", ModelState);
        //        var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
        //        oper.AppendData = ModelState.GE();
        //        return Json(oper);
        //        //return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
        //    }
        //}
        
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Index()
        {
            if (ResourceId.HP(����.�鿴))
            {
                var vModel = VModelType.New();
                FillModel(vModel);
                if (vModel is BaseVModel<T>)
                {
                    var vm = vModel as BaseVModel<T>;
                    vm.DoSth();
                }
                ViewBag.RS = ResourceId;
                return View(vModel);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
                //return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }

        /// <summary>
        /// ����ҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Sort(string returnUrl)
        {
            if (ResourceId.HP(����.�޸�))
            {
                var vModel = VModelType.New();
                FillModel(vModel);
                if (vModel is BaseVModel<T>)
                {
                    var vm = vModel as BaseVModel<T>;
                    vm.DoSth();
                }
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl;                 
                return View(vModel);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
                //return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }

        /// <summary>
        /// �ƶ���
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Move(string returnUrl)
        {
            if (ResourceId.HP(����.�޸�))
            {
                var vModel = VModelType.New();
                FillModel(vModel);
                if (vModel is BaseVModel<T>)
                {
                    var vm = vModel as BaseVModel<T>;
                    vm.DoSth();
                }
                ViewBag.RS = ResourceId;
                ViewBag.ReturnUrl = returnUrl;                 
                return View(vModel);
            }
            else
            {
                "".AE("��Ȩ��", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "��Ȩ��");
                oper.AppendData = ModelState.GE();
                return Json(oper);
                //return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
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

        /// <summary>
        /// ��ɾ��ʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnFTCk(T t)
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
