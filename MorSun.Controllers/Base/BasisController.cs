using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HOHO18.Common;
using MorSun.Bll;
using MorSun.Controllers.Filter;
using MorSun.Model;
using MorSun.Controllers.ViewModel;
using System.Reflection;
using FastReflectionLib;
using System.Web.Script.Serialization;
using MorSun.Common.Privelege;
using System.Data.Objects.DataClasses;
using MorSun.Controllers;
using HOHO18.Common.Web;

namespace System
{
    public static class ControllerHelper
    {
        /// <summary>
        /// �ж��Ƿ���Ȩ��
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public static bool HP(this string resourceId, string operationId)
        {
            return MorSun.Controllers.BasisController.HP(resourceId, operationId);
        }

        /// <summary>
        /// ����GUID�ַ��������������
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getRef(this string str)
        {
            return BasisController.GetTypeName(str);
        }
    }
}

namespace MorSun.Controllers
{    
    public class BasisController : Controller
    {   
        #region ���ض�����
        /// <summary>
        /// ���صĴ�������װ
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="oper"></param>
        /// <param name="defAction"></param>
        /// <param name="defController"></param>
        protected void fillOperationResult(string returnUrl, OperationResult oper, string message = "�����ɹ�", string defAction = "index", string defController = "home")
        {
            oper.ResultType = OperationResultType.Success;
            oper.Message = message;
            oper.AppendData = string.IsNullOrEmpty(returnUrl) ? Url.Action(defAction, defController) : returnUrl;
        }
        #endregion  

        #region ������Ϣ
        /// <summary>
        /// �û�ID
        /// </summary>        
        protected static Guid UserID
        {
            get
            {
                string name = System.Web.HttpContext.Current.User.Identity.Name;
                if (string.IsNullOrEmpty(name))
                    System.Web.HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    System.Web.HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
                return new Guid(user.ProviderUserKey.ToString());
            }
        }     

        /// <summary>
        /// ��ǰ�û�������Ϣ
        /// </summary>        
        public static aspnet_Users CurrentAspNetUser
        {
            get
            {
                aspnet_Users user = new BaseBll<aspnet_Users>().GetModel(UserID);
                return user;
            }
        }
        #endregion

        /// <summary>
        /// ���ⳬ���û�
        /// </summary>
        /// <returns></returns>
        private static bool IsAU()
        {
            return UserID.ToString().Eql("AU".GX().Decrypt());
        }
        

        #region Ȩ��
        public static List<wmfRolePrivilegesView> getSessionPrivileges()
        {
            if (System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] == null)
                setSessionPrivileges();
            if (String.Compare("��Ȩ��", System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] as string) == 0)
                return null;
            else
                return System.Web.HttpContext.Current.Session["SessionPrivilege"] as List<wmfRolePrivilegesView>;
        }

        /// <summary>
        /// �жϵ�ǰ�û��Ƿ��ж�ĳ��Դ�ķ���Ȩ��
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public static bool havePrivilege(string resourceId, string operationId)
        {
            if (BasisController.getSessionPrivileges() == null)
                return false;
            else
            {
                return BasisController.getSessionPrivileges().Any(p => string.Compare(p.OperationId, operationId, true) == 0
                    && string.Compare(p.ResourcesId, resourceId, true) == 0) || IsAU();
            }
        }

        /// <summary>
        /// �жϵ�ǰ�û��Ƿ��ж�ĳ��Դ�ķ���Ȩ��,��������жϣ���ϵΪ��ֻҪһ��Ϊ�棬������
        /// add by timfeng 213-8-26
        /// </summary>
        public static bool havePrivilege(string resourceId, params string[] operationIds)
        {
            var res = false;
            if (BasisController.getSessionPrivileges() == null)
                return false;
            foreach (var operateId in operationIds)
            {
                res = BasisController.getSessionPrivileges().Any(p => string.Compare(p.OperationId, operateId, true) == 0
                    && string.Compare(p.ResourcesId, resourceId, true) == 0);
                if (res)
                    return res;
            }
            return res;
        }

        /// <summary>
        /// �жϵ�ǰ�û��Ƿ��ж�ĳ��Դ�ķ���Ȩ��,���в�������
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="operationId"></param>
        /// <param name="privilegeValue">Ȩ�޲���</param>
        /// <returns></returns>
        public static bool havePrivilegeWithPrivilegeValue(string resourceId, string operationId, string privilegeValue)
        {
            if (BasisController.getSessionPrivileges() == null)
                return false;
            else
            {
                return BasisController.getSessionPrivileges().Any(p => string.Compare(p.OperationId, operationId, true) == 0
                    && string.Compare(p.ResourcesId, resourceId, true) == 0);                
            }
        }        

        /// <summary>
        /// �����ݿ��ж�ȡȫ���б�
        /// <param name="userid">��ǰ�û�UserID</param>
        /// </summary>
        public static IList<wmfRolePrivilegesView> getSessionPrivilegesByDatabase(Guid userid)
        {
            var currUser = new BaseBll<aspnet_Users>().GetModel(userid);
            //ȡ����ǰ�û����е�Ȩ��-��ɫ���� ��߷������У��������
            var rolesId = currUser.aspnet_Roles.SingleOrDefault().RoleId;/*currUser.aspnet_Roles.Select(r => r.RoleId).ToArray();*/
           
            var rolePrivilegeViewBll = new BaseBll<wmfRolePrivilegesView>();
            var currentUserPrivileges = rolePrivilegeViewBll.All.Where(u => u.RoleId == rolesId).ToList();
            return currentUserPrivileges;
        }

        /// <summary>
        /// ȡ����ǰ�û���Ȩ���б�������Session�С�
        /// </summary>
        public static void setSessionPrivileges()
        {
            var sessionPrivilegeList = getSessionPrivilegesByDatabase(UserID);
            if (sessionPrivilegeList.Any())
            {
                System.Web.HttpContext.Current.Session["SessionPrivilege"] = sessionPrivilegeList;
                System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] = "��Ȩ��";//��Ȩ��ID��
            }
            else
            {
                System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] = "��Ȩ��";//��û��Ȩ��ID��
            }
        }
        #endregion

        #region ������־
        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="ID">�޸ļ�¼ID</param>
        /// <param name="tableName">����</param>
        /// <param name="operateContent">��������</param>
        /// <param name="originalContent">�޸�ǰ</param>
        /// <param name="afterOperateContent">�޸ĺ�</param>
        public void InsertLog(Guid? ID, string tableName, string operateContent, string originalContent, string afterOperateContent)
        {
            string result = string.Empty;
            var opeateBill = new BaseBll<wmfOperationalLogbook>();
            var model = new wmfOperationalLogbook();
            model.UserId = UserID;
            //model.ApplicationId = AppId;//Ӧ�ó���ID
            model.RegTime = DateTime.Now;//���ʱ��
            model.LinkId = ID;//�޸ļ�¼ID
            model.OperateTable = tableName;
            model.OriginalContent = originalContent;
            model.AfterOperateContent = afterOperateContent;
            model.UserIP = IPAddress;//�û�IP��ַ
            model.OperateContent = operateContent;//��������
            opeateBill.Insert(model);
        }

        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="ID">�޸ļ�¼ID</param>
        /// <param name="tableName">����</param>
        /// <param name="operateContent">��������</param>
        /// <param name="originalContent">�޸�ǰ</param>
        /// <param name="afterOperateContent">�޸ĺ�</param>
        /// <param name="userID">�û�Guid</param>
        public void InsertLog(Guid? ID, string tableName, string operateContent, string originalContent, string afterOperateContent, Guid userID)
        {
            string result = string.Empty;
            var opeateBill = new BaseBll<wmfOperationalLogbook>();
            var model = new wmfOperationalLogbook();
            model.UserId = userID;
            //model.ApplicationId = AppId;//Ӧ�ó���ID
            model.RegTime = DateTime.Now;//���ʱ��
            model.LinkId = ID;//�޸ļ�¼ID
            model.OperateTable = tableName;
            model.OriginalContent = originalContent;
            model.AfterOperateContent = afterOperateContent;
            model.UserIP = IPAddress;//�û�IP��ַ
            model.OperateContent = operateContent;//��������
            opeateBill.Insert(model);
        }


        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="ID">�޸ļ�¼ID</param>
        /// <param name="tableName">����</param>
        /// <param name="operateContent">��������</param>
        /// <param name="originalContent">�޸�ǰ</param>
        /// <param name="afterOperateContent">�޸ĺ�</param>
        /// <param name="userID">�û�Guid</param>
        /// <param name="businessId">ҵ��id���϶����ڲ������Ĺ��ܣ���һ����������������ܶ����Ʒ����ô�ö����ž���businessId��</param>
        public void InsertLog(Guid? ID, string tableName, string operateContent, string originalContent, string afterOperateContent, Guid userID, Guid? businessId)
        {
            string result = string.Empty;
            var opeateBill = new BaseBll<wmfOperationalLogbook>();
            var model = new wmfOperationalLogbook();
            model.UserId = userID;
            //model.ApplicationId = AppId;//Ӧ�ó���ID
            model.RegTime = DateTime.Now;//���ʱ��
            model.LinkId = ID;//�޸ļ�¼ID
            model.OperateTable = tableName;
            model.OriginalContent = originalContent;
            model.AfterOperateContent = afterOperateContent;
            model.UserIP = IPAddress;//�û�IP��ַ
            model.OperateContent = operateContent;//��������
            model.BusinessId = businessId;//ҵ��id
            opeateBill.Insert(model);
        }

        /// <summary>
        /// ������ʼ�¼
        /// </summary>
        /// <param name="pageBrowse"></param>
        public void InsertVisitInfo(wmfPageBrowse pageBrowse)
        {
            var pageBll = new BaseBll<wmfPageBrowse>();
            var model = new wmfPageBrowse();
            model = pageBrowse;
            model.IP = IPAddress;
            model.UserId = UserID;
            //model.ApplicationId = AppId;
            model.FirstTime = DateTime.Now;
            pageBll.Insert(model);
        }        
        #endregion

        #region ��ȡ�û���¼IP
        /// <summary>
        /// ��ȡ�õ�¼IP
        /// </summary>
        public string IPAddress
        {
            get
            {
                string user_IP;
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    user_IP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    user_IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }
                if (user_IP == null || user_IP == "")
                    user_IP = Request.UserHostAddress;
                return user_IP;
            }
        }

        #endregion       

        #region ͨ��Guid�ַ�����ȡ����
        /// <summary>
        /// ͨ��Guid�ַ�����ȡ����
        /// </summary>
        /// <param name="str">Guid�ַ���</param>
        /// <returns>��������</returns>
        public static string GetTypeName(string str)
        {
            var ret = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                var newStr = str.Split(',');
                var refList = new ReferenceVModel().All.OrderBy(t => t.Sort);
                for (int i = 0; i < newStr.Length; i++)
                {
                    if (newStr[i] != "")
                    {
                        var ID = Guid.Parse(newStr[i]);
                        ret += refList.Where(p => p.ID == ID).FirstOrDefault().ItemValue + ",";
                    }
                }
                ret = ret.TrimEnd(',');
            }
            return ret;
        }

        /// <summary>
        /// ��ȡRef����
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static wmfReference GetRefModel(Guid? id)
        {
            return new BaseBll<wmfReference>().GetModel(id);
        }
        #endregion

        #region ��֤���жϷ���
        /// <summary>
        /// ��֤����֤
        /// </summary>
        /// <param name="model"></param>
        protected void validateVerifyCode(string verifyCode, string verifycodeRandom, string xmlconfigName)
        {
            //�ж��Ƿ���֤�뿪��
            if (xmlconfigName.GX() == "true")
            {
                //�ж���֤���Ƿ���д
                if (String.IsNullOrEmpty(verifyCode))
                {
                    "Verifycode".AE("����д��֤��", ModelState);
                }
                if (VerifyCode.GetValue(verifycodeRandom) != null)
                {
                    object vCodeVal = VerifyCode.GetValue(verifycodeRandom);
                    if (String.IsNullOrEmpty(verifyCode) || vCodeVal == null || String.Compare(verifyCode, vCodeVal.ToString()) != 0)
                    {
                        "Verifycode".AE("��֤�����", ModelState);
                    }
                    else
                    {
                        //ajax�ķ�ʽ��¼��Ҫ�ȵ�¼�ɹ�֮��������֤������
                    }
                }
                else
                {
                    "Verifycode".AE("��֤�����", ModelState);
                }
                //�����֤����Ϣ
                clearVerifyCode(verifycodeRandom);
            }
        }

        /// <summary>
        /// ��ȡ��֤������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected void clearVerifyCode(string verifycodeRandom)
        {
            HOHO18.Common.Web.VerifyCodeType type = HOHO18.Common.Web.VerifyCodeType.Login;
            try
            {
                string typeStr = verifycodeRandom;
                if (String.IsNullOrEmpty(typeStr))
                    type = HOHO18.Common.Web.VerifyCodeType.Login;
                else
                    type = (HOHO18.Common.Web.VerifyCodeType)Enum.Parse(typeof(HOHO18.Common.Web.VerifyCodeType), typeStr, true);
            }
            catch { }
            VerifyCode.RemoveValue(type);
        }
        #endregion

        #region ���ɼ��ܴ�
        protected string GenerateEncryptCode(string userNameString,string encryptUrl,bool updateChange = false)
        {
            var er = new wmfEncryptRecord();
            er.ID = Guid.NewGuid();
            er.UserNameString = userNameString;
            er.EncryptCode = Guid.NewGuid().ToString().Encrypt(Guid.NewGuid().ToString());
            er.EncryptTime = DateTime.Now;
            er.EncryptUrl = encryptUrl;
            er.FlagTrashed = false;
            er.FlagDeleted = false;
            new BaseBll<wmfEncryptRecord>().Insert(er, updateChange);
            return er.EncryptCode;
        }
        #endregion
    }
}
