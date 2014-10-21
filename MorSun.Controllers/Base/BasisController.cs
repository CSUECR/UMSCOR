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
using MorSun.Common.类别;

namespace System
{
    public static class ControllerHelper
    {
        /// <summary>
        /// 判断是否有权限
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public static bool HP(this string resourceId, string operationId)
        {
            return MorSun.Controllers.BasisController.havePrivilege(resourceId, operationId);
        }

        /// <summary>
        /// 根据GUID字符串返回类别名称
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
        #region 返回对象处理
        /// <summary>
        /// 返回的错误对象封装
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="oper"></param>
        /// <param name="defAction"></param>
        /// <param name="defController"></param>
        protected void fillOperationResult(string returnUrl, OperationResult oper, string message = "操作成功", string defAction = "index", string defController = "home")
        {
            oper.ResultType = OperationResultType.Success;
            oper.Message = message;
            oper.AppendData = string.IsNullOrEmpty(returnUrl) ? Url.Action(defAction, defController) : returnUrl;
        }
        /// <summary>
        /// SSO登录时使用，需要访问子网站
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="SSOLink"></param>
        /// <param name="oper"></param>
        /// <param name="message"></param>
        /// <param name="defAction"></param>
        /// <param name="defController"></param>
        protected void fillOperationResult(string returnUrl, string SSOLink, OperationResult oper, string message = "操作成功", string defAction = "index", string defController = "home")
        {
            oper.ResultType = OperationResultType.Success;
            oper.Message = message;
            oper.AppendData = string.IsNullOrEmpty(returnUrl) ? Url.Action(defAction, defController) : returnUrl;
            oper.SSOLink = SSOLink;
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

        public static UserMaBi CurrentUserMabi
        {
            get
            {
                return GetUserMaBiByUId(UserID);
            }
        }

        /// <summary>
        /// 根据用户ID取各种马币值
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static UserMaBi GetUserMaBiByUId(Guid userId)
        {
            //取出当前用户已结算的马币值
            var umb = new BaseBll<bmUserMaBi>().All.Where(p => p.UserId == userId);
            //币种
            var mabi = Guid.Parse(Reference.马币类别_马币);
            var bbi = Guid.Parse(Reference.马币类别_邦币);
            var banbi = Guid.Parse(Reference.马币类别_绑币);

            var mabiO = umb.FirstOrDefault(p => p.MaBiRef == mabi);
            var bbiO = umb.FirstOrDefault(p => p.MaBiRef == bbi);
            var banbiO = umb.FirstOrDefault(p => p.MaBiRef == banbi);
            var userMaBi = new UserMaBi();
            userMaBi.mabi = mabiO == null ? 0 : mabiO.MaBiNum.Value;
            userMaBi.bbi = bbiO == null ? 0 : bbiO.MaBiNum.Value;
            userMaBi.banbi = banbiO == null ? 0 : banbiO.MaBiNum.Value;

            //减去未结算的马币值
            var rbll = new BaseBll<bmUserMaBiRecord>();
            var nonSettleMBR = rbll.All.Where(p => p.IsSettle == false && p.UserId == userId);
            
            //加马币
            var mabisum = nonSettleMBR.Where(p => p.MaBiRef == mabi).Sum(p => p.MaBiNum);
            //加邦币
            var bbisum = nonSettleMBR.Where(p => p.MaBiRef == bbi).Sum(p => p.MaBiNum);
            //加绑币
            var banbisum = nonSettleMBR.Where(p => p.MaBiRef == banbi).Sum(p => p.MaBiNum);
            if (mabisum != null && mabisum > 0)
                userMaBi.mabi += mabisum == null ? 0 : mabisum.Value;
            if (bbisum != null && bbisum > 0)
                userMaBi.bbi += bbisum == null ? 0 : bbisum.Value;
            if (banbisum != null && banbisum > 0)
                userMaBi.banbi += banbisum == null ? 0 : banbisum.Value;

            return userMaBi;
        }
        #endregion

        /// <summary>
        /// 特殊超级用户
        /// </summary>
        /// <returns></returns>
        private static bool IsAU()
        {
            return UserID.ToString().Eql("3FF6345278DFEE3DA3828594935FE5DC340586EA327A09F4F31B7C8B2A652189FED639A959BB4504".DP());// ("AU".GX().DP());防止被串改，不从XML获取
        }
        

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
                    && string.Compare(p.ResourceId, resourceId, true) == 0);
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
                    && string.Compare(p.ResourceId, resourceId, true) == 0);                
            }
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

        /// <summary>
        /// 插入访问记录
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
                        ret += refList.Where(p => p.ID == ID).FirstOrDefault().ItemValue + ",";
                    }
                }
                ret = ret.TrimEnd(',');
            }
            return ret;
        }

        /// <summary>
        /// 获取Ref对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static wmfReference GetRefModel(Guid? id)
        {
            return new BaseBll<wmfReference>().GetModel(id);
        }
        #endregion

        #region 验证码判断方法
        /// <summary>
        /// 验证码验证
        /// </summary>
        /// <param name="model"></param>
        protected void validateVerifyCode(string verifyCode, string verifycodeRandom, string xmlconfigName)
        {
            //判断是否验证码开启
            if (xmlconfigName.GX() == "true")
            {
                //判断验证码是否填写
                if (String.IsNullOrEmpty(verifyCode))
                {
                    "Verifycode".AE("请填写验证码", ModelState);
                }
                if (VerifyCode.GetValue(verifycodeRandom) != null)
                {
                    object vCodeVal = VerifyCode.GetValue(verifycodeRandom);
                    if (String.IsNullOrEmpty(verifyCode) || vCodeVal == null || String.Compare(verifyCode, vCodeVal.ToString()) != 0)
                    {
                        "Verifycode".AE("验证码填错", ModelState);
                    }
                    else
                    {
                        //ajax的方式登录，要等登录成功之后才清除验证码数据
                    }
                }
                else
                {
                    "Verifycode".AE("验证码填错", ModelState);
                }
                //清除验证码信息
                clearVerifyCode(verifycodeRandom);
            }
        }

        /// <summary>
        /// 获取验证码类型
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

        #region 生成加密串
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

        #region 随机生成样式
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

        #region 用户币记录
        /// <summary>
        /// 添加用户币记录
        /// </summary>
        /// <param name="uIds">用户ID集，可批量添加。</param>
        /// <param name="sr">来源</param>
        /// <param name="mbr">币种</param>
        /// <param name="mbn">币值</param>
        public void AddUMBR(AddMBRModel addMBR, bool updateChange = true)
        {
            var rbll = new BaseBll<bmUserMaBiRecord>();  
            //检测用户是否存在
            var users = new BaseBll<aspnet_Users>().All.Where(p => addMBR.uIds.Contains(p.UserId));//找得到userId 就添加
            foreach (var u in users)
            {
                var model = new bmUserMaBiRecord();
                model.SourceRef = addMBR.sr;
                model.MaBiRef = addMBR.mbr;
                model.MaBiNum = addMBR.mbn;
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
        /// 批量设置为已结算
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
        /// 马币即时统计 更新数据库版
        /// </summary>
        protected void SettleMaBi()
        {
            var rbll = new BaseBll<bmUserMaBiRecord>();
            var umbbll = new BaseBll<bmUserMaBi>();
            //用户币增减记录
            var nonSettleMBR = rbll.All.Where(p => p.IsSettle == false && p.UserId != null);            
            var nonSMGroup = nonSettleMBR.GroupBy(p => p.UserId);
            var userIds = nonSMGroup.Select(p => p.Key);
            //用户各种币
            var userMabi = umbbll.All.Where(p => userIds.Contains(p.UserId));
            //币种
            var mabi = Guid.Parse(Reference.马币类别_马币);
            var bbi = Guid.Parse(Reference.马币类别_邦币);
            var banbi = Guid.Parse(Reference.马币类别_绑币);

            foreach(var smg in nonSMGroup)
            {
                //加马币
                var mabisum = smg.Where(p => p.MaBiRef == mabi).Sum(p => p.MaBiNum);
                if(mabisum != 0)
                {
                    var thisUserMabi = userMabi.Where(p => p.UserId == smg.Key && p.MaBiRef == mabi).FirstOrDefault();
                    if(thisUserMabi == null)
                    {//系统还没有添加此马币的情况                        
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
                //加邦币
                var bbisum = smg.Where(p => p.MaBiRef == bbi).Sum(p => p.MaBiNum);
                if (bbisum != 0)
                {
                    var thisUserMabi = userMabi.Where(p => p.UserId == smg.Key && p.MaBiRef == bbi).FirstOrDefault();
                    if (thisUserMabi == null)
                    {//系统还没有添加此马币的情况  
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


                //加绑币
                var banbisum = smg.Where(p => p.MaBiRef == banbi).Sum(p => p.MaBiNum);
                if (banbisum != 0)
                {
                    var thisUserMabi = userMabi.Where(p => p.UserId == smg.Key && p.MaBiRef == banbi).FirstOrDefault();
                    if (thisUserMabi == null)
                    {//系统还没有添加此马币的情况                        
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

            //将用户币记录设置为已结算
            foreach(var item in nonSettleMBR)
            {
                item.IsSettle = true;
                item.ModTime = DateTime.Now;
            }

            rbll.UpdateChanges();
        }
        /// <summary>
        /// 根据传入的参数生成用户马币对象
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
    }
}
