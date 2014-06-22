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
        /// 项目Application
        /// </summary>
        public Guid AppId
        {
            get
            {
                return new Guid(ConfigHelper.GetConfigString("ProjectApplication"));
            }
        }

        #region 出错信息

        #region 获取当前错误的列表信息
        /// <summary>
        /// 获取当前错误的列表信息
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<ModelStateErrorMessage> GetErrorMessagesByModelState()
        {
            var list = new List<ModelStateErrorMessage>();

            list = ModelState.Where(u => u.Value.Errors.Any()).Select(u => new ModelStateErrorMessage() { Key = u.Key, ErrorMessages = u.Value.Errors.Select(e => e.ErrorMessage) }).ToList();
            //想要读取配置XML信息，从这开始取
            return list;
        }
        #endregion

        /// <summary>
        /// 返回json格式对象
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
        /// 获取
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

        #region 基本信息
        /// <summary>
        /// 用户ID
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
        /// 用户ID
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
        /// 是否登录
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
        /// 当前用户基本信息
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
        /// 当前用户信息
        /// </summary>
        protected static MembershipUser CurrentUser
        {
            get
            {
                return Membership.GetUser();
            }
        }
        #endregion

        #region 权限

        public static List<wmfRolePrivilegesView> getSessionPrivileges()
        {
            if (System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] == null)
                setSessionPrivileges();
            if (String.Compare("无权限", System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] as string) == 0)
                return null;
            else
                return System.Web.HttpContext.Current.Session["SessionPrivilege"] as List<wmfRolePrivilegesView>;
        }

        /// <summary>
        /// 判断当前用户是否含有对某资源的访问权限
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
        /// 判断当前用户是否含有对某资源的访问权限,多个操作判断，关系为或，只要一个为真，返回真
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
        /// 判断当前用户是否含有对某资源的访问权限,含有操作参数
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="operationId"></param>
        /// <param name="privilegeValue">权限参数</param>
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
        /// 从角色列表找出角色名列表
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
        /// 从数据库中读取全下列表
        /// <param name="userid">当前用户UserID</param>
        /// </summary>
        public static IList<wmfRolePrivilegesView> getSessionPrivilegesByDatabase(Guid userid)
        {
            var currUser = new BaseBll<aspnet_Users>().GetModel(userid);
            //取出当前用户所有的权限-角色集合 这边方法不行，用上面的
            var rolesId = currUser.aspnet_Roles.SingleOrDefault().RoleId;/*currUser.aspnet_Roles.Select(r => r.RoleId).ToArray();*/
           
            var rolePrivilegeViewBll = new BaseBll<wmfRolePrivilegesView>();
            var currentUserPrivileges = rolePrivilegeViewBll.All.Where(u => u.RoleId == rolesId).ToList();
            return currentUserPrivileges;
        }

        /// <summary>
        /// 取出当前用户的权限列表，并放在Session中。
        /// </summary>
        public static void setSessionPrivileges()
        {
            var sessionPrivilegeList = getSessionPrivilegesByDatabase(UserID);
            if (sessionPrivilegeList.Any())
            {
                System.Web.HttpContext.Current.Session["SessionPrivilege"] = sessionPrivilegeList;
                System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] = "有权限";//有权限ID集
            }
            else
            {
                System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] = "无权限";//是没有权限ID集
            }
        }

        /// <summary>
        /// 清空Session
        /// </summary>
        public static void clearSession()
        {
            System.Web.HttpContext.Current.Session["SessionPrivilege"] = null;
            System.Web.HttpContext.Current.Session["HaveSessionPrivilege"] = null;
        }




        #endregion



        #region 插入日志
        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="ID">修改记录ID</param>
        /// <param name="tableName">表名</param>
        /// <param name="operateContent">操作类型</param>
        /// <param name="originalContent">修改前</param>
        /// <param name="afterOperateContent">修改后</param>
        public void InsertLog(Guid? ID, string tableName, string operateContent, string originalContent, string afterOperateContent)
        {
            string result = string.Empty;
            var opeateBill = new BaseBll<wmfOperationalLogbook>();
            var model = new wmfOperationalLogbook();
            model.UserId = UserID;
            //model.ApplicationId = AppId;//应用程序ID
            model.RegTime = DateTime.Now;//添加时间
            model.LinkId = ID;//修改记录ID
            model.OperateTable = tableName;
            model.OriginalContent = originalContent;
            model.AfterOperateContent = afterOperateContent;
            model.UserIP = IPAddress;//用户IP地址
            model.OperateContent = operateContent;//操作类型
            opeateBill.Insert(model);
        }

        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="ID">修改记录ID</param>
        /// <param name="tableName">表名</param>
        /// <param name="operateContent">操作类型</param>
        /// <param name="originalContent">修改前</param>
        /// <param name="afterOperateContent">修改后</param>
        /// <param name="userID">用户Guid</param>
        public void InsertLog(Guid? ID, string tableName, string operateContent, string originalContent, string afterOperateContent, Guid userID)
        {
            string result = string.Empty;
            var opeateBill = new BaseBll<wmfOperationalLogbook>();
            var model = new wmfOperationalLogbook();
            model.UserId = userID;
            //model.ApplicationId = AppId;//应用程序ID
            model.RegTime = DateTime.Now;//添加时间
            model.LinkId = ID;//修改记录ID
            model.OperateTable = tableName;
            model.OriginalContent = originalContent;
            model.AfterOperateContent = afterOperateContent;
            model.UserIP = IPAddress;//用户IP地址
            model.OperateContent = operateContent;//操作类型
            opeateBill.Insert(model);
        }


        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="ID">修改记录ID</param>
        /// <param name="tableName">表名</param>
        /// <param name="operateContent">操作类型</param>
        /// <param name="originalContent">修改前</param>
        /// <param name="afterOperateContent">修改后</param>
        /// <param name="userID">用户Guid</param>
        /// <param name="businessId">业务id、较多用于操作多表的功能（如一个订单号里面包含很多的商品，那么该订单号就是businessId）</param>
        public void InsertLog(Guid? ID, string tableName, string operateContent, string originalContent, string afterOperateContent, Guid userID, Guid? businessId)
        {
            string result = string.Empty;
            var opeateBill = new BaseBll<wmfOperationalLogbook>();
            var model = new wmfOperationalLogbook();
            model.UserId = userID;
            //model.ApplicationId = AppId;//应用程序ID
            model.RegTime = DateTime.Now;//添加时间
            model.LinkId = ID;//修改记录ID
            model.OperateTable = tableName;
            model.OriginalContent = originalContent;
            model.AfterOperateContent = afterOperateContent;
            model.UserIP = IPAddress;//用户IP地址
            model.OperateContent = operateContent;//操作类型
            model.BusinessId = businessId;//业务id
            opeateBill.Insert(model);
        }



        #region 插入访问记录
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

        #region 获取用户登录IP
        /// <summary>
        /// 获取用登录IP
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

        //#region 生成省市县json
        ///// <summary>
        ///// 生成省市县json
        ///// </summary>
        ///// <returns></returns>
        //public virtual string GetAreaToJson()
        //{
        //    string saveJsPath = System.Configuration.ConfigurationManager.AppSettings["AreaJsPath"], localJsPath;
        //    if (String.IsNullOrEmpty(saveJsPath)) throw new Exception("需要在配置节点中增加AreaJsPath");
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

        #region 文件下载
        public virtual string OnPreDownload(string path, string file)
        {
            var ret = string.Empty;
            if (!System.IO.File.Exists(Server.MapPath(path)))
            {

                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("文件不存在或已删除"), "") });
            }

            ret = "true";

            return ret;
        }

        //文件下载
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

        #region 通过Guid字符串获取名称
        /// <summary>
        /// 通过Guid字符串获取名称
        /// </summary>
        /// <param name="str">Guid字符串</param>
        /// <returns>类型名称</returns>
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

        #region 通过用户GUID获取部门GUID
        /// <summary>
        /// 通过用户GUID获取部门GUID
        /// </summary>
        /// <param name="userId">用户GUID</param>
        /// <returns>部门GUID</returns>
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

        #region 通过名称模糊查询
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
        //                sb.Append("{name:'" + (item.UnitPersonal == Guid.Parse(MorSun.Common.类别.Reference.工程管理_单位OR个人用户_单位) ? item.Company : item.LinkMan) + "',id:'" + item.ID + "'}\n");
        //            }
        //        }
        //        else
        //        {
        //            sb.Append("{name:'" + (item.UnitPersonal == Guid.Parse(MorSun.Common.类别.Reference.工程管理_单位OR个人用户_单位) ? item.Company : item.LinkMan) + "',id:'" + item.ID + "'}\n");
        //        }
        //    }
        //    return Content(sb.ToString());
        //}

        #endregion

        //#region 获取部门所有人员
        ////获取部门所有人员
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

        #region 通过部门ID获取父ID
        //通过部门ID获取父ID
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

        #region 通过类型名称得到类型ID
        //通过类型名称得到类型ID
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

        #region 通过类型ID得到类型名称
        public static wmfReference GetRefModel(Guid? id)
        {
            return new BaseBll<wmfReference>().GetModel(id);
        }
        #endregion

        #region 获取多个类别ID的类别名称字符串
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

        #region 获取文件路径
        public string getServerPath(string url)
        {
            return Server.MapPath(url);
        }
        #endregion

        #region 工作流URL转换
        /// <summary>
        /// 把URL替换成工作流虚拟目录的URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String ewfUrl(string url)
        {
            //取出数据库中的考勤申请表的菜单链接，然后显示在页面上
            var domail = HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("Domail");
            var username = MorSun.Controllers.BasisController.CurrentAspNetUser.UserName;
            var userinfo = MorSun.Controllers.BasisController.CurrentAspNetUser.wmfUserInfo;
            var sid = userinfo.SID;
            //cc工作流
            var cvd = HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("Ccflow");
            //E表工作流
            var vd = HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("工作流虚拟目录");

            //用户自增长ID
            var useraid = userinfo.AutoGeneticId;

            //用户真实姓名
            var truename = userinfo.TrueName;

            //用户ID
            var m_userid = MorSun.Controllers.BasisController.CurrentAspNetUser.UserId;
            return url.Replace("{domain}", domail).Replace("{vd}", vd).Replace("{cvd}", cvd).Replace("{username}", HttpUtility.UrlEncode(username)).Replace("{sid}", HttpUtility.UrlEncode(sid)).Replace("{UserID}", HttpUtility.UrlEncode(m_userid.ToString())).Replace("{useraid}", useraid.ToString()).Replace("{truename}", HttpUtility.UrlEncode(truename));
        }
        #endregion               

        public static bool IsAdmin
        {
            get
            {
                var privilegeList = BasisController.getSessionPrivileges();
                return privilegeList.Any(u => string.Compare(u.OperationId, 操作.系统管理员, true) == 0
                    && string.Compare(u.ResourcesId, 资源.操作范围, true) == 0);
            }
        }
    }
}
