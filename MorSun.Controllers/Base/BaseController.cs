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

        #region ��ȡĬ��ҵ���߼�����
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
        #endregion        

        #region ���
        /// <summary>
        /// ���
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual string Audit(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.���))
            {
                return NBatchAudit(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }


        /// <summary>
        /// ���
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string NBatchAudit(T t, Func<T, string> chAll = null, Func<T, string> ck = null)
        {
            chAll = chAll.Load(() => OnBatchAuditCk);
            ck = ck.Load(() => OnAuditToRecycle);
            string msg = "���ʧ��";
            string result = string.Empty;
            string[] ids = GetCheckId(t).Split(',');
            if (ids[0] == "")
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��ѡ��Ҫ��˵���"), "") });
            }
            var ckRs2 = chAll(t);
            if (ckRs2.IsWhite() || ckRs2.Eql("true"))
            {
                for (int i = ids.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        T model = Bll.GetModel(ids[i]);
                        var ckRs = ck(model);
                        if (ckRs.IsWhite() || ckRs.Eql("true"))
                        {
                            SetFlagAudit(model);
                            if (GetTreeFlag(t) == "true")
                            {
                                Bll.UpdateChanges();
                            }
                            //���������־
                            InsertLog(GetLinkID(model), GetOperateTable(model), "���", "", "");
                        }
                        else
                        {
                            result += ckRs;
                            break;
                        }
                    }
                }
            }
            else
            {
                return ckRs2;
            }
            if (GetTreeFlag(t) != "true")
            {
                Bll.UpdateChanges();
            }
            if (result != "")
            {
                return result;
            }
            msg = "true";
            return msg;
        }



        /// <summary>
        /// ���ǰ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnBatchAuditCk(T t)
        {
            return null;
        }



        /// <summary>
        /// ��ʶΪ���״̬
        /// </summary>
        /// <param name="t">ʵ����</param>
        [Authorize]
        [ExceptionFilter()]
        private void SetFlagAudit(T t)
        {
            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "AuditUser")
                {
                    if (item.GetValue(t, null) == null)
                    {
                        item.SetValue(t, UserID, null);
                    }
                }
                if (item.Name == "AuditTime")
                {
                    if (item.GetValue(t, null) == null)
                    {
                        item.SetValue(t, DateTime.Now, null);
                    }
                }
                if (item.Name == "IsCheck")
                {
                    item.SetValue(t, true, null);
                }
            }
        }
        #endregion

        #region ���

        #region ��ʾ���ҳ��
        /// <summary>
        /// ��ʾ���ҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult Add(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.���))
            {
                ViewBag.canDoSth = this.CanDoSth;
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }

        #endregion


        #region ���
        /// <summary>
        /// ���
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        public virtual String Create(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.���))
                return NCreate(t);
            else
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
        }
        /// <summary>
        /// ���
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

            //��ӳ�ʼ���ֶ�
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
                        return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��ѯʧ��"), "") });
                    }
                    else
                    {
                        InsertLog(GetLinkID(t), GetOperateTable(t), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("���"), "", "");
                        return "true";
                    }

                }
                else
                    return prers;
            }
            else
            {
                return getErrListJson(v.GetRuleViolations());
            }

        }

        /// <summary>
        /// ��Ӽ�¼�ľ���ʵ�֡�
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected virtual T NInsert(T t)
        {
            var result = Bll.Insert(t);
            return result;
        }

        /// <summary>
        /// ����ҳ���¼
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

        #region ɾ��

        #region ֱ��ɾ��
        /// <summary>
        /// ֱ��ɾ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual string Delete(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.ɾ��))
                return NDelete(t);
            else
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });

        }

        /// <summary>
        /// ֱ��ɾ��
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
                InsertLog(null, GetOperateTable(t), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("ɾ��"), "", "");
                result = "true";
            }
            return result;
        }
        #endregion

        #region ɾ��������վ
        /// <summary>
        /// ɾ��������վ
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string NDelToRecycle(T t, Func<T, string> ck = null)
        {
            ck = ck.Load(() => OnDelToRecycle);
            T model = Bll.GetModel(t);
            var ckRs = ck(model);
            if (!ckRs.IsWhite() && !ckRs.Eql("true"))
            {
                return ckRs;
            }
            SetFlagTrashed(model);
            Bll.Update(model);
            //���������־
            InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("ɾ��������վ"), "", "");
            return "true";
        }

        //ɾ��������վ
        [Authorize]
        [ExceptionFilter()]
        public virtual string DelToRecycle(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.ɾ��))
            {
                return NDelToRecycle(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }
        #endregion

        #region ����վ��ԭ
        /// <summary>
        /// ����վ��ԭ
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string NRecovery(T t, Func<T, string> ck = null)
        {
            ck = ck.Load(() => OnRecoveryCk);
            T model = Bll.GetModel(t);
            var ckRs = ck(model);
            if (!ckRs.IsWhite() && !ckRs.Eql("true"))
            {
                return ckRs;
            }
            SetFlagTrashedFalse(model);
            //���������־
            InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("����վ��ԭ"), "", "");
            Bll.Update(model);
            return "true";
        }

        [Authorize]
        [ExceptionFilter()]
        public virtual string Recovery(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.����վ))
            {
                return NRecovery(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }
        #endregion

        #region ������ԭ����վ
        /// <summary>
        /// ������ԭ����վ
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string NBatchRecovery(T t, Func<T, string> chAll = null, Func<T, string> ck = null)
        {
            chAll = chAll.Load(() => OnBatchRecoveryCk);
            ck = ck.Load(() => OnRecoveryCk);
            string msg = "��ѡ��Ҫ��ԭ����";
            string result = string.Empty;
            string[] ids = GetCheckId(t).Split(',');
            if (ids[0] == "")
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��ѡ��Ҫ��ԭ����"), "") });
            }
            var ckRs2 = chAll(t);
            if (ckRs2.IsWhite() || ckRs2.Eql("true"))
            {
                for (int i = ids.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        T model = Bll.GetModel(ids[i]);
                        var ckRs = ck(model);
                        if (ckRs.IsWhite() || ckRs.Eql("true"))
                        {
                            SetFlagTrashedFalse(model);
                            if (GetTreeFlag(t) == "true")
                            {
                                Bll.UpdateChanges();
                            }
                            //���������־
                            InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("����վ������ԭ"), "", "");
                        }
                        else
                        {
                            result += ckRs;
                            break;
                        }
                    }
                }
            }
            else
            {
                return ckRs2;
            }
            if (GetTreeFlag(t) != "true")
            {
                Bll.UpdateChanges();
            }
            if (result != "")
            {
                return result;
            }
            msg = "true";
            return msg;
        }

        //������ԭ����վ
        [Authorize]
        [ExceptionFilter()]
        public virtual string BatchRecovery(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.����վ))
            {
                return NBatchRecovery(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }
        #endregion

        #region ����ֱ��ɾ��
        /// <summary>
        /// ����ֱ��ɾ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string NBatchDelete(T t, Func<T, string> chAll = null, Func<T, string> ck = null)
        {
            chAll = chAll.Load(() => OnBatchDelCk);
            ck = ck.Load(() => OnDelCk);
            string msg = "ɾ��ʧ��";
            string result = string.Empty;
            string[] ids = GetCheckId(t).Split(',');
            if (ids[0] == "")
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��ѡ��Ҫɾ������"), "") });
            }
            var ckRs2 = chAll(t);
            if (ckRs2.IsWhite() || ckRs2.Eql("true"))
            {
                for (int i = ids.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        T model = Bll.GetModel(ids[i]);
                        var ckRs = ck(model);
                        if (ckRs.IsWhite() || ckRs.Eql("true"))
                        {
                            Bll.Delete(model, false);
                            if (GetTreeFlag(t) == "true")
                            {
                                Bll.UpdateChanges();
                            }
                        }
                        else
                        {
                            result += ckRs;
                            break;
                        }

                    }
                }
            }
            else
            {
                return ckRs2;
            }
            if (GetTreeFlag(t) != "true")
            {
                Bll.UpdateChanges();
            }
            if (result != "")
            {
                return result;
            }
            //���������־
            InsertLog(null, GetOperateTable(t), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("����ɾ��"), "", "");
            msg = "true";
            return msg;
        }

        //����ɾ��
        [Authorize]
        [ExceptionFilter()]
        public virtual string BatchDelete(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.ɾ��))
            {
                return NBatchDelete(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }

        }
        #endregion

        #region ����ɾ��������վ
        /// <summary>
        /// ����ɾ��������վ
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string NBatchRecycle(T t, Func<T, string> chAll = null, Func<T, string> ck = null)
        {
            chAll = chAll.Load(() => OnBatchRecycleCk);
            ck = ck.Load(() => OnDelToRecycle);
            string msg = "ɾ��ʧ��";
            string result = string.Empty;
            string[] ids = GetCheckId(t).Split(',');
            if (ids[0] == "")
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��ѡ��Ҫɾ������"), "") });
            }
            var ckRs2 = chAll(t);
            if (ckRs2.IsWhite() || ckRs2.Eql("true"))
            {
                for (int i = ids.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        T model = Bll.GetModel(ids[i]);
                        var ckRs = ck(model);
                        if (ckRs.IsWhite() || ckRs.Eql("true"))
                        {
                            SetFlagTrashed(model);
                            if (GetTreeFlag(t) == "true")
                            {
                                Bll.UpdateChanges();
                            }
                            //���������־
                            InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("����ɾ��������վ"), "", "");
                        }
                        else
                        {
                            result += ckRs;
                            break;
                        }
                    }
                }
            }
            else
            {
                return ckRs2;
            }
            if (GetTreeFlag(t) != "true")
            {
                Bll.UpdateChanges();
            }
            if (result != "")
            {
                return result;
            }
            msg = "true";
            return msg;
        }

        /// <summary>
        ///  ����ɾ��������վ
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual string BatchRecycle(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.ɾ��))
            {
                return NBatchRecycle(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }

        #endregion



        #endregion

        #region �鿴ҳ��
        /// <summary>
        /// ��ʾ�༭ҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult SeeView(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.�鿴))
            {
                ViewBag.canDoSth = this.CanDoSth;
                ViewBag.IsAdmin = IsAdmin;
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }
        #endregion

        #region �༭

        #region ��ʾ�༭ҳ��
        /// <summary>
        /// ��ʾ�༭ҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        public virtual ActionResult Update(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.�޸�))
            {
                ViewBag.canDoSth = this.CanDoSth;
                var item = SetEntity(t);
                ViewBag.IsAdmin = IsAdmin;
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }

        }
        #endregion

        #region �༭
        /// <summary>
        /// �༭
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        [ExceptionFilter()]
        public virtual String Edit(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.�޸�))
            {
                return NEdit(t);
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"), "") });
            }
        }

        /// <summary>
        /// �༭ͨ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <param name="ck">�༭ǰ����֤</param>
        /// <returns></returns>
        protected string NEdit(T t, Func<T, string> ck = null)
        {
            if (ck == null)
            {
                ck = OnEditCK;
            }

            //�༭ǰ��ֵ
            EditInitObject(t);

            var result = "false";
            var model = Bll.GetModel(t);
            if (model == null)
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "�༭ʧ��"), "") });
            }
            //��־��¼��ȥ��
            //var originalContent = string.Empty;
            //var afterOperateContent = string.Empty;
            //var msg = string.Empty;
            //msg = t.Equal(model, out originalContent, out afterOperateContent);
            //if (msg == "true")
            //{
            //    InsertLog(GetLinkID(model), GetOperateTable(model), HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("�༭"), originalContent, afterOperateContent);
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
                result = getErrListJson(model.AsDy().GetRuleViolations());
            }

            return result;
        }

        /// <summary>
        /// ��ȡ��¼��Ϣ
        /// </summary>
        /// <param name="id">��¼ID</param>
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

        #region ��ѯ

        #region ��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual string GetSortableList(T t)
        {
            return "";
        }

        #endregion

        #region ��ϸҳ��

        /// <summary>
        /// �첽��ȡ��ϸ��Ϣ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public ActionResult GetView(T t)
        {
            var model = Bll.GetModel(t);
            if (model == null)
            {
                return Json(null);
            }
            var js = JsHelper.Json(model);

            return Json("[" + js + "]");
        }

        /// <summary>
        /// ��ϸҳ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        //[Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Detail(T t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.�鿴))
            {
                var item = SetEntity(t);
                return View(item);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }
        #endregion

        #region ����ʵ��
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
        #endregion

        #region ��ȡ��ѡ�е�ID
        /// <summary>
        /// ��ȡ��ѡ�е�ID
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public string GetCheckId(T t)
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
        #endregion

        #region ��ʶΪ��
        /// <summary>
        /// ��ʶΪ��
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        private string GetTreeFlag(T t)
        {
            string checkedId = "false";
            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "isTree")
                {
                    if (item.GetValue(t, null) != null)
                    {
                        checkedId = item.GetValue(t, null).ToString();
                    }
                }
            }
            return checkedId;
        }
        #endregion

        #region ��ʶΪɾ��������վ״̬
        /// <summary>
        /// ��ʶΪɾ��������վ״̬
        /// </summary>
        /// <param name="t">ʵ����</param>
        [Authorize]
        [ExceptionFilter()]
        private void SetFlagTrashed(T t)
        {
            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "FlagTrashed")
                {
                    if (item.GetValue(t, null) != null)
                    {
                        item.SetValue(t, true, null);
                    }
                }
            }
        }
        #endregion

        #region ��ȡ�����ID
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
        #endregion

        #region ��ȡ����ı���
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
        #endregion

        #region ��ʶΪ����״̬
        /// <summary>
        /// ��ʶΪ����״̬
        /// </summary>
        /// <param name="t">ʵ����</param>
        [Authorize]
        [ExceptionFilter()]
        private void SetFlagTrashedFalse(T t)
        {

            PropertyInfo[] properites = t.GetType().GetProperties();
            foreach (PropertyInfo item in properites)
            {
                if (item.Name == "FlagTrashed")
                {
                    if (item.GetValue(t, null) != null)
                    {
                        item.SetValue(t, false, null);
                    }
                }
            }
        }
        #endregion

        #region ��ȡ��ӦViewModel����

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
        #endregion

        #region ���ʵ��
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

        #region ��ʾ�б�ҳ��
        /// <summary>
        /// ��ʾ�б�ҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult List()
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
        #endregion

        #region ����ҳ��
        /// <summary>
        /// ����ҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult SortList()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.����))
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

        #region ����վҳ��
        /// <summary>
        /// ����վҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Recycle()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.����վ))
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

        #region ����ɾ��������վҳ��
        /// <summary>
        /// ����ɾ��������վҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public ActionResult BatchRecycleList()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.ɾ��))
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


        #region ��ҳ��
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Index()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.�鿴))
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
                return Content(XmlHelper.GetKeyNameValidation("��Ŀ��ʾ", "��Ȩ�޲���"));
            }
        }

        /// <summary>
        /// ������ҳ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Index2()
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.����.�鿴))
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

        #region �ڱ༭ǰ��������֤
        /// <summary>
        /// �ڱ༭ǰ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnEditCK(T t)
        {
            return null;
        }
        #endregion

        #region ���֮ǰ��֤
        /// <summary>
        /// ���֮ǰ��֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnPreCreateCK(T t)
        {
            return "";
        }
        #endregion

        #region ��ɾ��ʱ��������֤
        /// <summary>
        /// ��ɾ��ʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnDelCk(T t)
        {
            return null;
        }
        #endregion

        #region ����ֱ��ɾ��ʱ��������֤
        /// <summary>
        /// ����ֱ��ɾ��ʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnBatchDelCk(T t)
        {
            return null;
        }
        #endregion

        #region ����ɾ��������վʱ��������֤
        /// <summary>
        /// ����ɾ��������վʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnBatchRecycleCk(T t)
        {
            return null;
        }
        #endregion

        #region ��������վ��ԭʱ��������֤
        /// <summary>
        /// ��������վ��ԭʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnBatchRecoveryCk(T t)
        {
            return null;
        }
        #endregion

        #region ����վ��ԭʱ��������֤
        /// <summary>
        /// ����վ��ԭʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnRecoveryCk(T t)
        {
            return null;
        }
        #endregion

        #region ����վ��ԭʱ��������֤
        /// <summary>
        /// ����վ��ԭʱ��������֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnDelToRecycle(T t)
        {
            return null;
        }
        #endregion

        #region ���ʱ��֤
        /// <summary>
        /// ���ʱ��֤
        /// </summary>
        /// <param name="t">ʵ����</param>
        /// <returns></returns>
        protected virtual string OnAuditToRecycle(T t)
        {
            return null;
        }
        #endregion

        #endregion

        #region ɾ���ļ�
        /// <summary>
        /// ɾ���ļ�
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
        /// �޸�Ĭ�ϳ�ʼ��RegTime��RegUser��FlagDeleted��FlagTrashed�ֶ�
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



        #region Ȩ�ޣ���BaseMvcController���ƹ���
        /// <summary>
        /// ��ǰ�û��ܽ��еĲ�������ɾ�����༭���鿴�ȵȣ�
        /// </summary>
        public virtual OperatePrivilege CanDoSth
        {
            get
            {
                var _canDoSth = new OperatePrivilege();
                if (this.ResourceId == null)
                {
                    #region Ȩ��Ĭ�Ͽ��أ�����ʱ,�������汾��ʱ����Ҫͳһ�����Դ��Ȩ��
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
                        if (string.Compare(privilgeItem.OperationId, ����.�鿴, true) == 0)
                            _canDoSth.CanRead = true;

                        if (string.Compare(privilgeItem.OperationId, ����.���, true) == 0)
                            _canDoSth.CanCreate = true;

                        if (string.Compare(privilgeItem.OperationId, ����.�޸�, true) == 0)
                            _canDoSth.CanEdit = true;

                        if (string.Compare(privilgeItem.OperationId, ����.ɾ��, true) == 0)
                            _canDoSth.CanFlagDelete = true;

                        if (string.Compare(privilgeItem.OperationId, ����.����ɾ��, true) == 0)
                            _canDoSth.CanDelete = true;

                        if (string.Compare(privilgeItem.OperationId, ����.����, true) == 0)
                            _canDoSth.CanArrange = true;

                        //if (string.Compare(privilgeItem.operationId, ����.����, true) == 0)
                        //    _canDoSth.CanRevoke = true;

                        if (string.Compare(privilgeItem.OperationId, ����.ȷ��, true) == 0)
                            _canDoSth.CanConfirm = true;

                        if (string.Compare(privilgeItem.OperationId, ����.����վ, true) == 0)
                            _canDoSth.CanRecycle = true;

                        if (string.Compare(privilgeItem.OperationId, ����.����, true) == 0)
                            _canDoSth.CanOrder = true;

                        if (string.Compare(privilgeItem.OperationId, ����.����, true) == 0)
                            _canDoSth.CanGenerate = true;

                        if (string.Compare(privilgeItem.OperationId, ����.����, true) == 0)
                            _canDoSth.CanImport = true;

                        //if (string.Compare(privilgeItem.operationId, ����.����, true) == 0)
                        //    _canDoSth.CanUnLock = true;

                        if (string.Compare(privilgeItem.OperationId, ����.����, true) == 0)
                            _canDoSth.CanConfigure = true;
                        if (string.Compare(privilgeItem.OperationId, ����.���, true) == 0)
                            _canDoSth.CanAudit = true;
                        if (string.Compare(privilgeItem.OperationId, ����.��ӡ, true) == 0)
                            _canDoSth.CanPrint = true;

                    }
                }
                return _canDoSth;
            }
        }
        /// <summary>
        /// �ǲ�������Ա
        /// </summary>
        public virtual bool IsAdmin
        {
            get
            {
                var privilegeList = BasisController.getSessionPrivileges();
                return privilegeList.Any(u => string.Compare(u.OperationId, ����.ϵͳ����Ա, true) == 0
                    && string.Compare(u.ResourcesId, ��Դ.������Χ, true) == 0);
            }
        }
        #endregion

        

    }
}
