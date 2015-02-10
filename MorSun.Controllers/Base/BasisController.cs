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
using MorSun.Common.���;
using MorSun.Common.����;
using MorSun.Common.������;
using HOHO18.Common.SSO;
using HOHO18.Common.WEB;
using HOHO18.Common.DEncrypt;
using Newtonsoft.Json;

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
            return MorSun.Controllers.BasisController.havePrivilege(resourceId, operationId);
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

        public static string getBC(this string str,string css)
        {
            return BasisController.GBSC(str,css);
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
        protected void fillOperationResult(string returnUrl, OperationResult oper, string message = "�����ɹ�", string defAction = "I", string defController = "H")
        {
            oper.ResultType = OperationResultType.Success;
            oper.Message = message;
            oper.AppendData = string.IsNullOrEmpty(returnUrl) ? Url.Action(defAction, defController) : returnUrl;
        }
        /// <summary>
        /// SSO��¼ʱʹ�ã���Ҫ��������վ
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="SSOLink"></param>
        /// <param name="oper"></param>
        /// <param name="message"></param>
        /// <param name="defAction"></param>
        /// <param name="defController"></param>
        protected void fillOperationResult(string returnUrl, string SSOLink, OperationResult oper, string message = "�����ɹ�", string defAction = "I", string defController = "H")
        {
            oper.ResultType = OperationResultType.Success;
            oper.Message = message;
            oper.AppendData = string.IsNullOrEmpty(returnUrl) ? Url.Action(defAction, defController) : returnUrl;
            oper.SSOLink = SSOLink;
        }
        #endregion  

        #region ������Ϣ
        /// <summary>
        /// �ж��û��Ƿ��¼
        /// </summary>
        public static bool IsLogin
        {
            get
            {
                string name = System.Web.HttpContext.Current.User.Identity.Name;
                if (string.IsNullOrEmpty(name))
                    return false;
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return false;
                return true;
            }
        }

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

        public static bmNewUserMB CurrentUserMabi
        {
            get
            {
                return GetUserMaBiByUId(UserID);
            }
        }

        /// <summary>
        /// �����û�IDȡ�������ֵ
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected static bmNewUserMB GetUserMaBiByUId(Guid userId)
        {
            //ȡ����ǰ�û�ʣ������ֵ(����δ����)
            return new BaseBll<bmNewUserMB>().All.FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// �����û�ID��������Ƿ���ȡ��
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        protected static IQueryable<bmNewUserMB> GetUserMaBiByUIds(List<Guid?> userIds)
        {
            var uids = new List<Guid>();
            foreach (var l in userIds)
            {
                if (l != null)
                {
                    uids.Add(l.Value);
                }
            }
            //ȡ����ǰ�û�ʣ������ֵ(����δ����)
            return new BaseBll<bmNewUserMB>().All.Where(p => uids.Contains(p.UserId));
        }
        #endregion

        /// <summary>
        /// ���ⳬ���û�
        /// </summary>
        /// <returns></returns>
        private static bool IsAU()
        {
            return UserID.ToString().Eql("F814B49DF592FAD4782E3CE0A1CDA161079357D9E9B5918C9082514F442DBED524543717D60B0369".DP());// ("AU".GX().DP());��ֹ�����ģ�����XML��ȡ
        }

        #region ��֤���ʲ���
        /// <summary>
        /// �Ƿ�Ϊ��֤����
        /// </summary>
        /// <param name="tok"></param>
        /// <param name="rz"></param>
        /// <returns></returns>
        protected static bool IsRZ(string tok, bool rz, HttpRequestBase rq)
        {
            try
            {
                //�ж��Ƿ���������������
                var ts = SecurityHelper.Decrypt(tok);
                //ȡʱ���
                var ind = ts.IndexOf(';');
                DateTime dt = DateTime.Parse(ts.Substring(0, ind));
                //�ö�ʱ��ִ��ʱ���ӳ٣�5�벻��
                if (dt.AddSeconds(12) < DateTime.Now || !ts.Contains(CFG.������_�Խ�ͳһ��))
                {//����8����
                    rz = false;
                    LogHelper.Write("����δ��֤" + rq.RawUrl, LogHelper.LogMessageType.Info);
                }
                else
                {
                    rz = true;
                    LogHelper.Write("ʱ�䣺" + ts.Substring(0, ind), LogHelper.LogMessageType.Debug);
                }
            }
            catch
            {
                rz = false;
                LogHelper.Write("���ʸ���ԭ����֤����" + rq.RawUrl, LogHelper.LogMessageType.Error);
            }
            return rz;
        }

        /// <summary>
        /// �������л�ΪJSON��ѹ��
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected static string ToJsonAndCompress(object v)
        {
            var jsonV = JsonConvert.SerializeObject(v);
            return Compression.CompressString(jsonV);
        }

        /// <summary>
        /// ��JSON����ѹ������
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected static string EncodeJson(string s)
        {
            //var ys = Compression.CompressString(s);
            var dts = DateTime.Now.ToString();
            var eys = SecurityHelper.Encrypt(dts + ";" + s);
            return eys;
        }
        /// <summary>
        /// ��JSON���н��ܽ�ѹ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected static string DecodeJson(string id)
        {
            var eys = SecurityHelper.Decrypt(id);
            var ys = eys.Substring(eys.IndexOf(';') + 1);
            //var s = Compression.DecompressString(ys);
            return ys;
        }
        #endregion

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
            if (IsAU())
                return true;
            if (BasisController.getSessionPrivileges() == null)
                return false;
            else
            {
                return BasisController.getSessionPrivileges().Any(p => string.Compare(p.OperationId, operationId, true) == 0
                    && string.Compare(p.ResourceId, resourceId, true) == 0);
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
                    && string.Compare(p.ResourceId, resourceId, true) == 0);
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
                    && string.Compare(p.ResourceId, resourceId, true) == 0);                
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
            er.EncryptCode = Guid.NewGuid().ToString().EP(Guid.NewGuid().ToString());
            er.EncryptTime = DateTime.Now;
            er.EncryptUrl = encryptUrl;
            er.FlagTrashed = false;
            er.FlagDeleted = false;
            new BaseBll<wmfEncryptRecord>().Insert(er, updateChange);
            return er.EncryptCode;
        }
        #endregion

        #region ���������ʽ
        public static string GBSC(string str,string css)
        {
            var s = css.GX().Split(',');
            if (s.Count() > 0)
            {
                var i = s.Count();
                Random rd = new Random();
                int j = rd.Next(0,i);
                return s[j];
            }
            else
                return str;
        }
        #endregion

        #region �û��Ҽ�¼
        /// <summary>
        /// ����û��Ҽ�¼  �������û������ID
        /// </summary>
        /// <param name="uIds">�û�ID������������ӡ�</param>
        /// <param name="sr">��Դ</param>
        /// <param name="mbr">����</param>
        /// <param name="mbn">��ֵ</param>
        public void AddUMBR(AddMBRModel addMBR, bool updateChange = true)
        {
            var rbll = new BaseBll<bmUserMaBiRecord>();  
            //����û��Ƿ����
            var users = new BaseBll<aspnet_Users>().All.Where(p => addMBR.UIds.Contains(p.UserId));//�ҵõ�userId �����
            foreach (var u in users)
            {
                var model = new bmUserMaBiRecord();
                model.SourceRef = addMBR.SR;
                model.MaBiRef = addMBR.MBR;
                model.MaBiNum = addMBR.MBN;
                model.IsSettle = false;

                model.RegTime = DateTime.Now;
                model.ModTime = DateTime.Now;
                model.FlagTrashed = false;
                model.FlagDeleted = false;

                model.ID = Guid.NewGuid();
                model.UserId = u.UserId;
                if (User != null && User.Identity.IsAuthenticated)
                    model.RegUser = UserID;
                else
                    model.RegUser = u.UserId;
                rbll.Insert(model, false);
            }
            if (updateChange)
                rbll.UpdateChanges();
        }

        /// <summary>
        /// ��������Ϊ�ѽ���
        /// </summary>
        /// <param name="mbList"></param>
        protected void setUMBRSettle(IQueryable<bmUserMaBiRecord> mbList, bool updateChange = true)
        {
            var rbll = new BaseBll<bmUserMaBiRecord>();
            foreach(var m in mbList)
            {
                m.IsSettle = true;
                m.ModTime = DateTime.Now;
            }
            if (updateChange)
                rbll.UpdateChanges();
        }

        /// <summary>
        /// ��Ҽ�ʱͳ�� �������ݿ��
        /// </summary>
        protected void SettleMaBi()
        {
            var rbll = new BaseBll<bmUserMaBiRecord>();
            var umbbll = new BaseBll<bmUserMaBi>();
            //�û���������¼
            var nonSettleMBR = rbll.All.Where(p => p.IsSettle == false && p.UserId != null);            
            var nonSMGroup = nonSettleMBR.GroupBy(p => p.UserId);
            var userIds = nonSMGroup.Select(p => p.Key);
            //�û����ֱ�
            var userMabi = umbbll.All.Where(p => userIds.Contains(p.UserId));
            //����
            var mabi = Guid.Parse(Reference.������_���);
            var bbi = Guid.Parse(Reference.������_���);
            var banbi = Guid.Parse(Reference.������_���);

            foreach(var smg in nonSMGroup)
            {
                //�����
                var mabisum = smg.Where(p => p.MaBiRef == mabi).Sum(p => p.MaBiNum);
                if(mabisum != 0)
                {
                    var thisUserMabi = userMabi.Where(p => p.UserId == smg.Key && p.MaBiRef == mabi).FirstOrDefault();
                    if(thisUserMabi == null)
                    {//ϵͳ��û����Ӵ���ҵ����                        
                        var model = GenerateUserMaBi(mabi, mabisum, smg.Key);
                        umbbll.Insert(model, false);
                    }
                    else
                    {
                        thisUserMabi.MaBiNum += mabisum;
                        thisUserMabi.SettleTime = DateTime.Now;
                        thisUserMabi.ModTime = DateTime.Now;
                    }
                }
                //�Ӱ��
                var bbisum = smg.Where(p => p.MaBiRef == bbi).Sum(p => p.MaBiNum);
                if (bbisum != 0)
                {
                    var thisUserMabi = userMabi.Where(p => p.UserId == smg.Key && p.MaBiRef == bbi).FirstOrDefault();
                    if (thisUserMabi == null)
                    {//ϵͳ��û����Ӵ���ҵ����  
                        var model = GenerateUserMaBi(bbi, bbisum, smg.Key);
                        umbbll.Insert(model, false);
                    }
                    else
                    {
                        thisUserMabi.MaBiNum += bbisum;
                        thisUserMabi.SettleTime = DateTime.Now;
                        thisUserMabi.ModTime = DateTime.Now;
                    }
                }


                //�Ӱ��
                var banbisum = smg.Where(p => p.MaBiRef == banbi).Sum(p => p.MaBiNum);
                if (banbisum != 0)
                {
                    var thisUserMabi = userMabi.Where(p => p.UserId == smg.Key && p.MaBiRef == banbi).FirstOrDefault();
                    if (thisUserMabi == null)
                    {//ϵͳ��û����Ӵ���ҵ����                        
                        var model = GenerateUserMaBi(banbi, banbisum, smg.Key);
                        umbbll.Insert(model, false);
                    }
                    else
                    {
                        thisUserMabi.MaBiNum += banbisum;
                        thisUserMabi.SettleTime = DateTime.Now;
                        thisUserMabi.ModTime = DateTime.Now;
                    }
                }
            }

            //���û��Ҽ�¼����Ϊ�ѽ���
            foreach(var item in nonSettleMBR)
            {
                item.IsSettle = true;
                item.ModTime = DateTime.Now;
            }

            rbll.UpdateChanges();
        }
        /// <summary>
        /// ���ݴ���Ĳ��������û���Ҷ���
        /// </summary>
        /// <param name="mabi"></param>
        /// <param name="mabisum"></param>
        /// <param name="mbUserId"></param>
        /// <returns></returns>
        private static bmUserMaBi GenerateUserMaBi(Guid mabi, decimal? mabisum, Guid? mbUserId)
        {
            var model = new bmUserMaBi();
            model.ID = Guid.NewGuid();
            model.UserId = mbUserId;
            model.MaBiRef = mabi;
            model.MaBiNum = mabisum;
            model.SettleTime = DateTime.Now;

            model.RegUser = mbUserId;
            model.RegTime = DateTime.Now;
            model.ModTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
            return model;
        }
        #endregion

        #region ��ȡ�û��İ���Ϣ
        /// <summary>
        /// ��ȡ�û���΢�Ű���ҵ����Ϣ
        /// </summary>
        /// <returns></returns>
        protected bmUserWeixin GetUserBound()
        {
            var wxyy = Guid.Parse(CFG.������_��ǰ΢��Ӧ��);
            return new BaseBll<bmUserWeixin>().All.FirstOrDefault(p => p.UserId == UserID && p.WeiXinAPP == wxyy);
        }

        /// <summary>
        /// ��ȡ�û�����Ϣ
        /// </summary>
        /// <returns></returns>
        protected UserBoundCache GetUserBoundCache()
        {
            var key = CFG.΢�Ű�ǰ׺ + UserID.ToString();
            var ubc = CacheAccess.GetFromCache(key) as UserBoundCache;
            //�������Ϊ�գ���������ֵ
            if(ubc == null)
            {
                ubc = new UserBoundCache();
                ubc.UserId = UserID;
                Random Rdm = new Random();
                int iRdm = 0;
                do
                {
                    iRdm = Rdm.Next(1, 999999);

                } while (GetUserBoundCodeCache(iRdm) != null);//��Ϊ�ղŻ�������
                //�������������뻺��
                var codeKey = CFG.΢�Ű�ǰ׺ + iRdm.ToString();
                var ubcc = new UserBoundCodeCache();
                ubcc.UserId = UserID;
                ubcc.BoundCode = iRdm;
                CacheAccess.InsertToCacheByTime(codeKey, ubcc, 120);//�������ڹ���
                ubc.BoundCode = iRdm;
                CacheAccess.InsertToCacheByTime(key, ubc, 120);
            }
            return ubc;
        }

        /// <summary>
        /// ���ݰ󶨴���ȡҪ�󶨵��û�
        /// </summary>
        /// <param name="boundCode"></param>
        /// <returns></returns>
        protected UserBoundCodeCache GetUserBoundCodeCache(int boundCode)
        {
            var key = CFG.΢�Ű�ǰ׺ + boundCode.ToString();
            return CacheAccess.GetFromCache(key) as UserBoundCodeCache;
        }
        #endregion

        #region ���⻺�洦��
        /// <summary>
        /// �����ݿ���ȡ�û��������ɻ�����
        /// </summary>
        /// <returns></returns>
        public static OnlineQAUserCache GenerateQAUserCache()
        {
            var bll = new BaseBll<bmOnlineQAUser>();
            var qadisbll = new BaseBll<bmQADistribution>();
            var qabll = new BaseBll<bmQA>();
            var bmumbBll = new BaseBll<bmUserMaBiRecord>();
            var uwbll = new BaseBll<bmUserWeixin>();
            var numbbll = new BaseBll<bmNewUserMB>();

            //����׷�ʵĴ���
            #region �������������������Ѽ�¼������¼
            //ȡ������δ�����¼������      "���ʶ���һ�����û�"
            var qaRef = Guid.Parse(Reference.�ʴ����_����);
            var curWeiXinAPP = Guid.Parse(CFG.������_��ǰ΢��Ӧ��);
            var nonmbQA = qabll.All.Where(p => p.QARef == qaRef && p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.bmQADistributions.Count() == 0);    //���������ʲŷ���,��ǰӦ����΢��APP��

            //ȡ������δ������Ҽ�¼�������û�
            var nonmbUid = nonmbQA.Select(p => p.WeiXinId).Distinct();
            LogHelper.Write("�����ʵ��û���" + nonmbUid.Count().ToString(), LogHelper.LogMessageType.Debug);
            //���ֳ��Ѱ���δ�󶨵��û�ID            
            //�󶨵��û�ID
            var uwU = uwbll.All.Where(p => nonmbUid.Contains(p.WeiXinId) && p.WeiXinAPP == curWeiXinAPP);
            //�󶨵��û�΢��ID
            var uwUid = uwU.Select(p => p.WeiXinId);

            //δ�󶨵��û�ID
            var nonuwUID = nonmbUid.Where(p => !uwUid.Contains(p));

            //�Ѱ󶨵��û�ȡ�����ֵ�������ֵ����0��ȡ����
            var uwUSid = uwU.Select(p => p.UserId);
            var defXFMB = Convert.ToDecimal(CFG.����Ĭ���շ����ֵ);
            var UserBMB = numbbll.All.Where(p => uwUSid.Contains(p.UserId) && (p.NMB > defXFMB || p.NBB > defXFMB));
            LogHelper.Write("����������ʵ��û���" + UserBMB.Count().ToString(), LogHelper.LogMessageType.Debug);
            //������Ҽ�¼
            //�������շѵ���Ҽ�¼���շ���������Ĭ���շѴ����û���δ�����շѵ���Ҽ�¼��ֱ�ӷ����Ĭ����Ѵ����û�
            var mbQAIds = new List<Guid>();
            var tempMB = Convert.ToDecimal(0);
            var tempBB = Convert.ToDecimal(0);
            var tempQACount = 0;
            foreach (var u in UserBMB)
            {
                tempMB = u.NMB.Value;
                tempBB = u.NBB.Value;
                //��ȡ���ĵ�ǰ�û�������
                tempQACount = Convert.ToInt32(((tempMB + tempBB) / defXFMB));
                var uwxid = uwU.FirstOrDefault(p => p.UserId == u.UserId).WeiXinId;
                //��ǰ�û�������
                var uqa = nonmbQA.Where(p => p.WeiXinId == uwxid).Take(tempQACount);
                foreach (var q in uqa)
                {
                    //�����ѵ������¼�����
                    mbQAIds.Add(q.ID);
                    var umbrModel = new bmUserMaBiRecord();
                    umbrModel.SourceRef = Guid.Parse(Reference.�����Դ_����);
                    if (tempBB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.������_���);
                        tempBB -= defXFMB;
                    }
                    else if (tempMB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.������_���);
                        tempMB -= defXFMB;
                    }
                    umbrModel.MaBiNum = 0 - defXFMB;
                    umbrModel.QAId = q.ID;

                    umbrModel.IsSettle = false;
                    umbrModel.RegTime = DateTime.Now;
                    umbrModel.ModTime = DateTime.Now;
                    umbrModel.FlagTrashed = false;
                    umbrModel.FlagDeleted = false;

                    umbrModel.ID = Guid.NewGuid();
                    umbrModel.UserId = u.UserId;
                    umbrModel.RegUser = u.UserId;

                    bmumbBll.Insert(umbrModel, false);
                }
            }
            LogHelper.Write("����������ʵ�������" + mbQAIds.Count().ToString(), LogHelper.LogMessageType.Debug);
            //������������¼���շѵĵ��շѵ�Ĭ���˺ţ���ѵĵ���ѵ�Ĭ���˺�
            foreach (var q in nonmbQA)
            {
                //������䴦��                
                var qaModel = new bmQADistribution();

                qaModel.ID = Guid.NewGuid();
                qaModel.QAId = q.ID;
                qaModel.DistributionTime = DateTime.Now;

                qaModel.RegTime = DateTime.Now;
                qaModel.ModTime = DateTime.Now;
                qaModel.FlagTrashed = false;
                qaModel.FlagDeleted = false;

                qaModel.Result = Guid.Parse(Reference.����������_�����);
                if (mbQAIds.Contains(q.ID))
                {
                    qaModel.WeiXinId = CFG.Ĭ���շ�����΢�ź�;
                }
                else
                {
                    qaModel.WeiXinId = CFG.Ĭ���������΢�ź�;
                }
                qadisbll.Insert(qaModel, false);
            }
            //bmumbBll.UpdateChanges();
            //qadisbll.UpdateChanges();

            #endregion

            var qastate = Guid.Parse(Reference.����������_�����);
            #region δ������������·���
            //Ĭ���û�δ����������޸�Ϊ������Գ������õ�ʱ��Ϊ׼
            var acQADmn = 0 - Convert.ToInt32(CFG.δ�������⼤��ʱ��);
            var acQADdt = DateTime.Now.AddMinutes(acQADmn);
            var nonHandleRef = Guid.Parse(Reference.����������_δ����);
            //����΢��APP���ж�
            var nonACQAD = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.ModTime < acQADdt && ConstList.DefaultDISUser.Contains(p.WeiXinId) && p.Result == nonHandleRef);
            LogHelper.Write((acQADdt.ToShortTimeString() + "�ֶ������û�����ʱδ�������������" + nonACQAD.Count().ToString()), LogHelper.LogMessageType.Debug);
            foreach (var item in nonACQAD)
            {
                item.Result = qastate;
                item.ModTime = DateTime.Now;
            }
            #endregion
            //���Կ���߲��������ݿ⣬���´�����ִ�в���
            //δ����������޸�Ϊ����� ����            
            var todayST = DateTime.Now.AddHours(-24);
            //ȡ���еĴ�������������
            var djdqadis = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.Result == qastate);
            LogHelper.Write("��������������" + djdqadis.Count().ToString(), LogHelper.LogMessageType.Debug);
            ///////////////////////ȡ�����ķ�ʽҪ�޸ģ����ʵķ�ʽ���ˣ���ֱ���������//////////////////////////
            var mabiqaCount = djdqadis.Where(p => p.bmQA.bmUserMaBiRecords.Sum(m => m.MaBiNum) < 0).Count();//�����Ǹ���
            LogHelper.Write("�������շ�������" + mabiqaCount.ToString(), LogHelper.LogMessageType.Debug);
            //��ѵ�ֻȡ24Сʱ�ڵ����ʼ�¼����ʡ��������Դ ����δ��������⣬������֣�����24Сʱ��
            var nonmabiqaCount = djdqadis.Where(p => p.bmQA.bmUserMaBiRecords.Count() == 0 && p.bmQA.RegTime >= todayST).Count();
            LogHelper.Write("���������������" + nonmabiqaCount.ToString(), LogHelper.LogMessageType.Debug);

            var state = Guid.Parse(Reference.����״̬_����);
            //�û������Ᵽ����
            var qaWaitCount = Convert.ToInt32(CFG.�û������Ᵽ����);

            //���Ҫ���˵�����Ծ�������û�,����5���Ӳ���Ծ�ģ��򲻷�����Ŀ����
            var nondismn = 0 - Convert.ToInt32(CFG.�����˳�ʱ��);
            var nondisdt = DateTime.Now.AddMinutes(nondismn);

            //��Ҫǿ���˳�������
            var logoutMN = 0 - Convert.ToInt32(CFG.ǿ���˳�ʱ��);
            var logoutdt = DateTime.Now.AddMinutes(logoutMN);

            var cu = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));
            var noncu = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));

            #region ���·��俪ʼ

            //ȡҪǿ�˵���֤�û� '����7����δ��Ծ'
            var noActiveCU = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && p.ActiveTime < logoutdt && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));
            LogHelper.Write("��Ҫǿ�˵���֤�û���" + noActiveCU.Count().ToString(), LogHelper.LogMessageType.Debug);
            //ȡҪǿ��δ��֤�û�
            var noActiveU = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && p.ActiveTime < logoutdt && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));
            LogHelper.Write("��Ҫǿ�˵�δ��֤�û���" + noActiveU.Count().ToString(), LogHelper.LogMessageType.Debug);

            //ȡǿ���û�ID
            //���ڲ���Ծ����֤�û���������Ծ�û��Ĵ�������¼��ʶΪ��������������������Ծ�û�
            var noActiveCUWIDS = noActiveCU.Select(p => p.WeiXinId);
            //���ڲ���Ծ����֤�û���������Ծ�û��Ĵ�������¼��ʶΪ��������������������Ծ�û�
            var noActiveUWIDS = noActiveU.Select(p => p.WeiXinId);

            //ȡ���в���Ծ�Ĵ����û���Ȼ�������ֳ��շ��������������
            var noActiveUCUQAD = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.Result == qastate && (noActiveCUWIDS.Contains(p.WeiXinId) || p.WeiXinId == CFG.Ĭ���շ�����΢�ź� || noActiveUWIDS.Contains(p.WeiXinId) || p.WeiXinId == CFG.Ĭ���������΢�ź�));
            LogHelper.Write("�ܵ�δ������" + noActiveUCUQAD.Count().ToString(), LogHelper.LogMessageType.Debug);
            var noActiveMQAD = noActiveUCUQAD.Where(p => p.bmQA.bmUserMaBiRecords.Sum(m => m.MaBiNum) < 0).OrderBy(p => p.bmQA.RegTime);
            LogHelper.Write("�շѵ�δ������" + noActiveMQAD.Count().ToString(), LogHelper.LogMessageType.Debug);

            //�շ��������
            //ȡ��Ծ���ߵ���֤�����û�,�ų���Ĭ�Ϸ����û���ʡ�Ķ������
            var certificationUsers = cu.Where(p => p.WeiXinId != CFG.Ĭ���շ�����΢�ź� && p.ActiveTime >= nondisdt).OrderByDescending(p => p.ActiveNum);
            if (mabiqaCount > 0)
            {
                int selectCount = mabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                certificationUsers = certificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }
            LogHelper.Write("������������֤�û���" + certificationUsers.Count().ToString(), LogHelper.LogMessageType.Debug);

            if (certificationUsers.Count() > 0)
            {//���������û�ʱ   
                if (noActiveMQAD.Count() > 0)
                {//�������������·���
                    var ouCount = certificationUsers.Count();
                    var i = 0;
                    foreach (var item in noActiveMQAD)
                    {
                        //�Դ�����з���
                        i++;
                        var j = i % ouCount;
                        if (j == 0)
                            j = ouCount;
                        var disOU = certificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                        item.WeiXinId = disOU.WeiXinId;
                        item.ModTime = DateTime.Now;
                    }
                }
            }
            else
            {//�����߻�Ծ�û���ϵͳ��������ո�Ĭ���û�                 
                if (noActiveMQAD.Count() > 0)
                { //��������ո�Ĭ�ϴ����û�
                    foreach (var item in noActiveMQAD)
                    {
                        item.WeiXinId = CFG.Ĭ���շ�����΢�ź�;
                        item.ModTime = DateTime.Now;
                    }
                }

            }
            //�շ�����������
            //����������  ����ķ������Ծδ��֤�û����ǵ���ķ����Ĭ���û�
            var noActiveNMQAD = noActiveUCUQAD.Where(p => p.bmQA.bmUserMaBiRecords.Count() == 0).OrderBy(p => p.bmQA.RegTime);
            LogHelper.Write("��ѵ�δ������" + noActiveNMQAD.Count().ToString(), LogHelper.LogMessageType.Debug);

            //δ��֤���û�����
            var noncertificationUsers = noncu.Where(p => p.WeiXinId != CFG.Ĭ���������΢�ź� && p.ActiveTime >= nondisdt).OrderByDescending(p => p.ActiveNum);
            if (nonmabiqaCount > 0)
            {
                int selectCount = nonmabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                noncertificationUsers = noncertificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }
            LogHelper.Write("����������δ��֤�û���" + noncertificationUsers.Count().ToString(), LogHelper.LogMessageType.Debug);

            if (noncertificationUsers.Count() > 0)
            {
                if (noActiveNMQAD.Count() > 0)
                {//�������������·���
                    var ouCount = noncertificationUsers.Count();
                    var i = 0;
                    foreach (var item in noActiveNMQAD)
                    {
                        //�ж��Ƿ��ǵ��������
                        if (item.bmQA.RegTime >= todayST)
                        {
                            //�Դ�����з���
                            i++;
                            var j = i % ouCount;
                            if (j == 0)
                                j = ouCount;
                            var disOU = noncertificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                            item.WeiXinId = disOU.WeiXinId;
                            item.ModTime = DateTime.Now;
                        }
                        else
                        {//�ǵ����������ո�Ĭ��΢�ź�
                            if (item.WeiXinId != CFG.Ĭ���������΢�ź�)
                                item.WeiXinId = CFG.Ĭ���������΢�ź�;
                        }
                    }
                }
            }
            else
            {
                if (noActiveNMQAD.Count() > 0)
                { //��������ո�Ĭ�ϴ����û�
                    foreach (var item in noActiveNMQAD)
                    {
                        item.WeiXinId = CFG.Ĭ���������΢�ź�;
                        item.ModTime = DateTime.Now;
                    }
                }
            }
            //�������������
            #endregion ���·������

            #region �Ƴ��û������Ĵ��⻺�棬���ݿ����
            //ǿ���˳�����Ծ�û�  ��������Ծ�û��Ĵ��⻺�����  
            var tqState = Guid.Parse(Reference.����״̬_�˳�);
            foreach (var item in noActiveCU)
            {
                //���޸��޸�ʱ�� 
                item.AQEndTime = DateTime.Now;
                item.State = tqState;
                CacheAccess.RemoveCache(CFG.�û������⻺���ǰ׺ + item.WeiXinId);
            }
            foreach (var item in noActiveU)
            {
                item.AQEndTime = DateTime.Now;
                item.State = tqState;
                CacheAccess.RemoveCache(CFG.�û������⻺���ǰ׺ + item.WeiXinId);
            }
            //ͳһ���½����ݿ�
            qadisbll.UpdateChanges();
            bll.UpdateChanges();
            //����Ծ�û��������
            #endregion

            //���ɻ������
            var model = new OnlineQAUserCache();
            model.RefreshTime = DateTime.Now;

            //�շѴ���������
            model.MaBiQACount = mabiqaCount;
            //��Ѵ���������
            model.NonMaBiQACount = nonmabiqaCount;
            model.CertificationUser = cu.OrderByDescending(p => p.ActiveNum);
            model.NonCertificationQAUser = noncu.OrderByDescending(p => p.ActiveNum);

            LogHelper.Write("�ֶ������û��������", LogHelper.LogMessageType.Debug);

            return model;
        }
        #endregion

        #region ΢��TOK����
        protected string SetWXTKCache()
        {
            var atURL = CFG.������_��ȡAT��ַ.Replace("APPID", CFG.������_Ӧ��ID).Replace("APPSECRET", CFG.������_Ӧ����Կ);
            //LogHelper.Write("��ȡ΢��ToKen��URL" + atURL, LogHelper.LogMessageType.Info);
            var atS = GetHtmlHelper.GetPage(atURL, "");
            var wsTKJson = JsonConvert.DeserializeObject<wxTKJson>(atS);
            //LogHelper.Write("��ȡ΢��ToKen" + wsTKJson.access_token, LogHelper.LogMessageType.Info);
            if (!String.IsNullOrEmpty(wsTKJson.errcode) || !String.IsNullOrEmpty(wsTKJson.errmsg))
            {
                LogHelper.Write("��ȡ΢��ToKenʧ��" + wsTKJson.errcode + " " + wsTKJson.errmsg, LogHelper.LogMessageType.Error);
            }
            else
            {
                //���浽������
                CacheAccess.AddToCacheByTime(CFG.������_AT�����, wsTKJson.access_token, 5400);
            }

            return wsTKJson.access_token;
        }

        public string GetWXTKCache()
        {
            var s = CacheAccess.GetFromCache(CFG.������_AT�����) as string;
            if(String.IsNullOrEmpty(s))
            {
                s = SetWXTKCache();
            }
            return s;
        }

        #endregion

    }
}
