using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HOHO18.Common;
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
//using MorSun.Controllers.HttpModules;


namespace MorSun.Controllers
{
    public class BasisController : Controller
    {

        public const String SearchPaAppValueColumn = "key";

        /// <summary>
        /// ��ĿApplication
        /// </summary>
        public Guid AppId
        {
            get
            {
                return new Guid(ConfigHelper.GetConfigString("ProjectApplication"));
            }
        }

        #region ������Ϣ

        #region ��ȡ��ǰ������б���Ϣ
        /// <summary>
        /// ��ȡ��ǰ������б���Ϣ
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<ModelStateErrorMessage> GetErrorMessagesByModelState()
        {
            var list = new List<ModelStateErrorMessage>();

            list = ModelState.Where(u => u.Value.Errors.Any()).Select(u => new ModelStateErrorMessage() { Key = u.Key, ErrorMessages = u.Value.Errors.Select(e => e.ErrorMessage) }).ToList();
            //��Ҫ��ȡ����XML��Ϣ�����⿪ʼȡ
            return list;
        }
        #endregion

        /// <summary>
        /// ����json��ʽ����
        /// </summary>
        /// <param name="errs"></param>
        /// <returns></returns>
        protected static string getErrListJson(IEnumerable<RuleViolation> errs)
        {
            var jsonb = new StringBuilder("[");
            var s = "";
            foreach (var err in errs)
            {
                var errJson = getErrJson(err);
                jsonb.Append(s).Append(errJson);
                s = ",";
            }
            jsonb.Append("]");
            return jsonb.ToString();
        }

        /// <summary>
        /// ��ȡ
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected static string getErrListJson(IModel mode)
        {
            return getErrListJson(mode.GetRuleViolations());
        }

        protected static string getErrJson(RuleViolation err)
        {
            var jsonb = new StringBuilder();
            jsonb.Append("{");
            jsonb.Append("ErrorMessage:").AppendFormat("'{0}',", err.ErrorMessage);
            jsonb.Append("PropertyName:").AppendFormat("'{0}'", err.PropertyName);
            jsonb.Append("}");
            return jsonb.ToString();
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
        /// �û�ID
        /// </summary>        
        protected static Guid? UserIdOrNull
        {
            get
            {
                string name = System.Web.HttpContext.Current.User.Identity.Name;
                if (string.IsNullOrEmpty(name))
                    return null;
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return null;
                return new Guid(user.ProviderUserKey.ToString());
            }
        }
        /// <summary>
        /// �Ƿ��¼
        /// </summary>        
        protected static bool IsLogin
        {
            get
            {
                if (System.Web.HttpContext.Current.User == null || System.Web.HttpContext.Current.User.Identity == null || String.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                    return false;
                return true;
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

        /// <summary>
        /// ��ǰ�û���Ϣ
        /// </summary>
        protected static MembershipUser CurrentUser
        {
            get
            {
                return Membership.GetUser();
            }
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
            if (BasisController.getSessionPrivileges() == null)
                return false;
            else
            {
                return BasisController.getSessionPrivileges().Any(p => string.Compare(p.OperationId, operationId, true) == 0
                    && string.Compare(p.ResourcesId, resourceId, true) == 0);
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
                /*&& p.privilegeValuesArray.Contains(privilegeValue.ToLower())).FirstOrDefault() != null*/
                ;
            }
        }

        /// <summary>
        /// �ӽ�ɫ�б��ҳ���ɫ���б�
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected String[] getOperatRoleNames(List<aspnet_Roles> list)
        {
            var hs = new HashSet<string>();
            foreach (aspnet_Roles l in list)
            {
                hs.Add(l.RoleName.ToString());
            }
            var t = new String[hs.Count];
            hs.CopyTo(t);
            return t;
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

        /// <summary>
        /// ���Session
        /// </summary>
        public static void clearSession()
        {
            System.Web.HttpContext.Current.Session["SessionPrivilege"] = null;
            System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] = null;
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



        #region ������ʼ�¼
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

        //#region ����ʡ����json
        ///// <summary>
        ///// ����ʡ����json
        ///// </summary>
        ///// <returns></returns>
        //public virtual string GetAreaToJson()
        //{
        //    string saveJsPath = System.Configuration.ConfigurationManager.AppSettings["AreaJsPath"], localJsPath;
        //    if (String.IsNullOrEmpty(saveJsPath)) throw new Exception("��Ҫ�����ýڵ�������AreaJsPath");
        //    try
        //    {
        //        localJsPath = Server.MapPath(saveJsPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    var json = new StringBuilder();

        //    var provinceBill = new BaseBll<wmfProvince>();
        //    var provinces = provinceBill.All.Where(p => p.FlagTrashed == false).OrderBy(p => p.Sort);

        //    var cityBill = new BaseBll<wmfCity>();
        //    var citys = cityBill.All.Where(p => p.FlagTrashed == false).OrderBy(p => p.Sort);

        //    var countyBill = new BaseBll<wmfCounty>();
        //    var countys = countyBill.All.Where(p => p.FlagTrashed == false).OrderBy(p => p.Sort);

        //    var townBill = new BaseBll<wmfTown>();
        //    var towns = townBill.All.Where(p => p.FlagTrashed == false).OrderBy(p => p.Sort);

        //    var villageBill = new BaseBll<wmfVillage>();
        //    var villages = villageBill.All.Where(p => p.FlagTrashed == false).OrderBy(p => p.Sort);

        //    json.Append("var provinceList={");
        //    foreach (var province in provinces)
        //    {
        //        json.Append("'" + province.ID.ToString().ToLower() + "':[");
        //        var tempCitys = citys.Where(c => c.ProvinceId == province.ID).OrderBy(c => c.Sort);
        //        foreach (var tempCity in tempCitys)
        //        {
        //            json.Append("'" + tempCity.ID.ToString().ToLower() + "','");
        //            json.Append("" + tempCity.CityName + "',");
        //        }
        //        if (tempCitys.Count() > 0) json.Remove(json.Length - 1, 1);

        //        json.Append("],");
        //    }
        //    if (provinces.Count() > 0) json.Remove(json.Length - 1, 1);
        //    json.Append("};");

        //    json.Append("var cityList={");
        //    foreach (var city in citys)
        //    {
        //        json.Append("'" + city.ID.ToString().ToLower() + "':[");
        //        var tempCountys = countys.Where(c => c.CityId == city.ID).OrderBy(c => c.Sort); ;
        //        foreach (var tempCounty in tempCountys)
        //        {
        //            json.Append("'" + tempCounty.ID.ToString().ToLower() + "','");
        //            json.Append("" + tempCounty.CountyName + "',");
        //        }
        //        if (tempCountys.Count() > 0) json.Remove(json.Length - 1, 1);

        //        json.Append("],");
        //    }
        //    if (citys.Count() > 0) json.Remove(json.Length - 1, 1);
        //    json.Append("};");

        //    json.Append("var countyList={");
        //    foreach (var county in countys)
        //    {
        //        json.Append("'" + county.ID.ToString().ToLower() + "':[");
        //        var tempTowns = towns.Where(t => t.CountyId == county.ID).OrderBy(c => c.Sort); ;
        //        foreach (var tempTown in tempTowns)
        //        {
        //            json.Append("'" + tempTown.ID.ToString().ToLower() + "','");
        //            json.Append("" + tempTown.TownName + "',");
        //        }
        //        if (tempTowns.Count() > 0) json.Remove(json.Length - 1, 1);

        //        json.Append("],");
        //    }
        //    if (countys.Count() > 0) json.Remove(json.Length - 1, 1);
        //    json.Append("};");

        //    json.Append("var townList={");
        //    foreach (var town in towns)
        //    {
        //        json.Append("'" + town.ID.ToString().ToLower() + "':[");
        //        var tempVillages = villages.Where(t => t.TownId == town.ID).OrderBy(c => c.Sort); ;
        //        foreach (var tempVillage in tempVillages)
        //        {
        //            json.Append("'" + tempVillage.ID.ToString().ToLower() + "','");
        //            json.Append("" + tempVillage.VillageName + "',");
        //        }
        //        if (tempVillages.Count() > 0) json.Remove(json.Length - 1, 1);

        //        json.Append("],");
        //    }
        //    if (towns.Count() > 0) json.Remove(json.Length - 1, 1);
        //    json.Append("};");

        //    System.IO.StreamWriter sw = null;
        //    try
        //    {
        //        if (System.IO.File.Exists(localJsPath))
        //        {
        //            if (System.IO.File.GetAttributes(localJsPath).ToString().IndexOf("ReadOnly") != -1)
        //            {
        //                System.IO.File.SetAttributes(localJsPath, System.IO.FileAttributes.Normal);
        //            }

        //            System.IO.File.Delete(localJsPath);
        //        }
        //        System.IO.FileStream fs = new System.IO.FileStream(localJsPath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);
        //        sw = new System.IO.StreamWriter(fs);
        //        sw.Write(json.ToString());
        //        sw.Close();
        //        sw.Dispose();
        //    }
        //    catch
        //    {
        //        if (sw != null)
        //        {
        //            sw.Close();
        //            sw.Dispose();
        //        }
        //    }
        //    return "true";
        //}
        //#endregion

        #region �ļ�����
        public virtual string OnPreDownload(string path, string file)
        {
            var ret = string.Empty;
            if (!System.IO.File.Exists(Server.MapPath(path)))
            {

                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("�ļ������ڻ���ɾ��"), "") });
            }

            ret = "true";

            return ret;
        }

        //�ļ�����
        public virtual FilePathResult DownloadFile(string path, string file)
        {
            if (!System.IO.File.Exists(Server.MapPath(path)))
            {
                return null;
            }
            var explore = Request.Browser.Browser.ToUpper();
            if (explore == "IE")
            {
                return File(Server.MapPath(path), "text/plain", Url.Encode(file));
            }
            else
            {
                return File(Server.MapPath(path), "text/plain", file);
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
                        ret += refList.Where(p => p.ID == ID).FirstOrDefault().ItemInfo + ",";
                    }
                }
                ret = ret.TrimEnd(',');
            }
            return ret;
        }
        #endregion

        #region ͨ���û�GUID��ȡ����GUID
        /// <summary>
        /// ͨ���û�GUID��ȡ����GUID
        /// </summary>
        /// <param name="userId">�û�GUID</param>
        /// <returns>����GUID</returns>
        public Guid GetDeptIdByUserId(Guid userId)
        {
            var ret = Guid.Empty;
            var l = new wmfUserDeptPositionVModel().All.Where(p => p.aspnet_Users.wmfUserInfo.IsNoCheck == false);
            if (userId != null && userId != Guid.Empty)
            {
                var model = l.Where(p => p.UserId == userId).FirstOrDefault();
                if (model != null && model.DeptId.HasValue)
                {
                    ret = model.DeptId.Value;
                }
            }
            return ret;
        }


        #endregion

        #region ͨ������ģ����ѯ
        public ContentResult GetUserNameString(String q)
        {
            var sb = new StringBuilder();
            var l = new UserVModel().All.Where(p => p.wmfUserInfo.FlagTrashed == false && p.wmfUserInfo.FlagDeleted == false);
            var deptList = new DeptVModel().List;
            if (!string.IsNullOrEmpty(q))
            {
                l = l.Where(p => p.UserName.Contains(q) || p.wmfUserDeptPositions.Count(r => r.wmfDept.DeptName.Contains(q)) > 0);
            }
            foreach (var item in l)
            {
                sb.Append("{name:'" + item.UserName + "',id:'" + item.UserId + "',deptName:'" + (item.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault() != null ? item.wmfUserDeptPositions.Where(p => p.UserId == item.UserId).FirstOrDefault().wmfDept.DeptName : "") + "'}\n");
            }
            return Content(sb.ToString());
        }       

        //public ContentResult GetProjectCustomerNameString(String q, string projectId)
        //{
        //    var sb = new StringBuilder();

        //    var l = new gcCustomerVModel().All;
        //    var projectCustomerList = new ywProjectCustomerVModel().All;
        //    if (!string.IsNullOrEmpty(q))
        //    {
        //        l = l.Where(p => p.Company.Contains(q) || p.LinkMan.Contains(q));
        //    }
        //    foreach (var item in l)
        //    {
        //        if (!string.IsNullOrEmpty(projectId))
        //        {
        //            var ID = Guid.Parse(projectId);
        //            var Model = projectCustomerList.Where(p=>p.CustomerId==item.ID&&p.ProjectId==ID&&p.IsEntrust==false).FirstOrDefault();
        //            if (Model == null)
        //            {
        //                sb.Append("{name:'" + (item.UnitPersonal == Guid.Parse(MorSun.Common.���.Reference.���̹���_��λOR�����û�_��λ) ? item.Company : item.LinkMan) + "',id:'" + item.ID + "'}\n");
        //            }
        //        }
        //        else
        //        {
        //            sb.Append("{name:'" + (item.UnitPersonal == Guid.Parse(MorSun.Common.���.Reference.���̹���_��λOR�����û�_��λ) ? item.Company : item.LinkMan) + "',id:'" + item.ID + "'}\n");
        //        }
        //    }
        //    return Content(sb.ToString());
        //}

        #endregion

        //#region ��ȡ����������Ա
        ////��ȡ����������Ա
        //public List<wmfUserInfo> GetUserByDeptId(Guid? deptId)
        //{
        //    var ret = new List<wmfUserInfo>();
        //    var l = new wmfUserDeptPositionVModel().All.Where(p => p.aspnet_Users.wmfUserInfo.IsNoCheck == false);
        //    if (deptId != null && deptId != Guid.Empty)
        //    {
        //        l = l.Where(p => p.DeptId == deptId);
        //    }
        //    foreach (var item in l)
        //    {
        //        var userInfo = new wmfUserInfoVModel().All.Where(p => p.ID == item.UserId).FirstOrDefault();
        //        userInfo.deptId = item.DeptId.Value;
        //        ret.Add(userInfo);
        //    }
        //    return ret;
        //}
        //#endregion

        #region ͨ������ID��ȡ��ID
        //ͨ������ID��ȡ��ID
        public Guid? GetParentDeptId(Guid deptId)
        {
            var ret = default(Guid?);
            var deptList = new DeptVModel().List;
            var deptModel = deptList.Where(p => p.ID == deptId).FirstOrDefault();
            if (deptModel != null)
            {
                ret = deptModel.ParentId;
            }
            return ret;
        }
        #endregion

        #region ͨ���������Ƶõ�����ID
        //ͨ���������Ƶõ�����ID
        public string GetReferGuidByName(string referName)
        {
            var ret = string.Empty;
            var refList = new ReferenceVModel().All.OrderBy(t => t.Sort);
            var model = refList.Where(p => p.ItemInfo.Contains(referName)).FirstOrDefault();
            if (model != null)
            {
                ret = model.ID.ToString();
            }
            return ret;
        }
        #endregion

        #region ͨ������ID�õ���������
        public static wmfReference GetRefModel(Guid? id)
        {
            return new BaseBll<wmfReference>().GetModel(id);
        }
        #endregion

        #region ��ȡ������ID����������ַ���
        public virtual string GetReferName(string guids, IQueryable<wmfReference> list)
        {
            var ret = string.Empty;

            if (string.IsNullOrEmpty(guids))
            {
                return "";
            }
            var guidStr = guids.Split(',');
            for (int i = 0; i < guidStr.Length; i++)
            {
                if (!string.IsNullOrEmpty(guidStr[i]))
                {
                    try
                    {
                        Guid sid = Guid.Parse(guidStr[i]);
                        if (sid != null && sid != Guid.Empty)
                        {
                            var sch = list.Where(r => r.ID == sid).FirstOrDefault();
                            if (sch != null)
                            {
                                ret += sch.ItemInfo + ",";
                            }
                        }
                    }
                    catch { }
                }
            }

            return ret;
        }
        #endregion        

        #region ��ȡ�ļ�·��
        public string getServerPath(string url)
        {
            return Server.MapPath(url);
        }
        #endregion

        #region ������URLת��
        /// <summary>
        /// ��URL�滻�ɹ���������Ŀ¼��URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String ewfUrl(string url)
        {
            //ȡ�����ݿ��еĿ��������Ĳ˵����ӣ�Ȼ����ʾ��ҳ����
            var domail = HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("Domail");
            var username = MorSun.Controllers.BasisController.CurrentAspNetUser.UserName;
            var userinfo = MorSun.Controllers.BasisController.CurrentAspNetUser.wmfUserInfo;
            var sid = userinfo.SID;
            //cc������
            var cvd = HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("Ccflow");
            //E������
            var vd = HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("����������Ŀ¼");

            //�û�������ID
            var useraid = userinfo.AutoGeneticId;

            //�û���ʵ����
            var truename = userinfo.TrueName;

            //�û�ID
            var m_userid = MorSun.Controllers.BasisController.CurrentAspNetUser.UserId;
            return url.Replace("{domain}", domail).Replace("{vd}", vd).Replace("{cvd}", cvd).Replace("{username}", HttpUtility.UrlEncode(username)).Replace("{sid}", HttpUtility.UrlEncode(sid)).Replace("{UserID}", HttpUtility.UrlEncode(m_userid.ToString())).Replace("{useraid}", useraid.ToString()).Replace("{truename}", HttpUtility.UrlEncode(truename));
        }
        #endregion               

        public static bool IsAdmin
        {
            get
            {
                var privilegeList = BasisController.getSessionPrivileges();
                return privilegeList.Any(u => string.Compare(u.OperationId, ����.ϵͳ����Ա, true) == 0
                    && string.Compare(u.ResourcesId, ��Դ.������Χ, true) == 0);
            }
        }
    }
}
