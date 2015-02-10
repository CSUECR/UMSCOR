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
using MorSun.Common.配置;
using MorSun.Common.常量集;
using HOHO18.Common.SSO;
using HOHO18.Common.WEB;
using HOHO18.Common.DEncrypt;
using Newtonsoft.Json;

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
        protected void fillOperationResult(string returnUrl, OperationResult oper, string message = "操作成功", string defAction = "I", string defController = "H")
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
        protected void fillOperationResult(string returnUrl, string SSOLink, OperationResult oper, string message = "操作成功", string defAction = "I", string defController = "H")
        {
            oper.ResultType = OperationResultType.Success;
            oper.Message = message;
            oper.AppendData = string.IsNullOrEmpty(returnUrl) ? Url.Action(defAction, defController) : returnUrl;
            oper.SSOLink = SSOLink;
        }
        #endregion  

        #region 基本信息
        /// <summary>
        /// 判断用户是否登录
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

        public static bmNewUserMB CurrentUserMabi
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
        protected static bmNewUserMB GetUserMaBiByUId(Guid userId)
        {
            //取出当前用户剩余的马币值(包括未结算)
            return new BaseBll<bmNewUserMB>().All.FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// 传入用户ID集，检测是否能取现
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
            //取出当前用户剩余的马币值(包括未结算)
            return new BaseBll<bmNewUserMB>().All.Where(p => uids.Contains(p.UserId));
        }
        #endregion

        /// <summary>
        /// 特殊超级用户
        /// </summary>
        /// <returns></returns>
        private static bool IsAU()
        {
            return UserID.ToString().Eql("F814B49DF592FAD4782E3CE0A1CDA161079357D9E9B5918C9082514F442DBED524543717D60B0369".DP());// ("AU".GX().DP());防止被串改，不从XML获取
        }

        #region 认证访问操作
        /// <summary>
        /// 是否为认证访问
        /// </summary>
        /// <param name="tok"></param>
        /// <param name="rz"></param>
        /// <returns></returns>
        protected static bool IsRZ(string tok, bool rz, HttpRequestBase rq)
        {
            try
            {
                //判断是否是正常渠道访问
                var ts = SecurityHelper.Decrypt(tok);
                //取时间戳
                var ind = ts.IndexOf(';');
                DateTime dt = DateTime.Parse(ts.Substring(0, ind));
                //用定时器执行时会延迟，5秒不够
                if (dt.AddSeconds(12) < DateTime.Now || !ts.Contains(CFG.邦马网_对接统一码))
                {//限制8秒内
                    rz = false;
                    LogHelper.Write("访问未认证" + rq.RawUrl, LogHelper.LogMessageType.Info);
                }
                else
                {
                    rz = true;
                    LogHelper.Write("时间：" + ts.Substring(0, ind), LogHelper.LogMessageType.Debug);
                }
            }
            catch
            {
                rz = false;
                LogHelper.Write("访问各种原因认证出错" + rq.RawUrl, LogHelper.LogMessageType.Error);
            }
            return rz;
        }

        /// <summary>
        /// 对象序列化为JSON并压缩
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected static string ToJsonAndCompress(object v)
        {
            var jsonV = JsonConvert.SerializeObject(v);
            return Compression.CompressString(jsonV);
        }

        /// <summary>
        /// 对JSON进行压缩加密
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
        /// 对JSON进行解密解压
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
        /// 添加用户币记录  这个方法没有问题ID
        /// </summary>
        /// <param name="uIds">用户ID集，可批量添加。</param>
        /// <param name="sr">来源</param>
        /// <param name="mbr">币种</param>
        /// <param name="mbn">币值</param>
        public void AddUMBR(AddMBRModel addMBR, bool updateChange = true)
        {
            var rbll = new BaseBll<bmUserMaBiRecord>();  
            //检测用户是否存在
            var users = new BaseBll<aspnet_Users>().All.Where(p => addMBR.UIds.Contains(p.UserId));//找得到userId 就添加
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

        #region 获取用户的绑定信息
        /// <summary>
        /// 获取用户的微信绑定作业邦信息
        /// </summary>
        /// <returns></returns>
        protected bmUserWeixin GetUserBound()
        {
            var wxyy = Guid.Parse(CFG.邦马网_当前微信应用);
            return new BaseBll<bmUserWeixin>().All.FirstOrDefault(p => p.UserId == UserID && p.WeiXinAPP == wxyy);
        }

        /// <summary>
        /// 获取用户绑定信息
        /// </summary>
        /// <returns></returns>
        protected UserBoundCache GetUserBoundCache()
        {
            var key = CFG.微信绑定前缀 + UserID.ToString();
            var ubc = CacheAccess.GetFromCache(key) as UserBoundCache;
            //如果缓存为空，重新设置值
            if(ubc == null)
            {
                ubc = new UserBoundCache();
                ubc.UserId = UserID;
                Random Rdm = new Random();
                int iRdm = 0;
                do
                {
                    iRdm = Rdm.Next(1, 999999);

                } while (GetUserBoundCodeCache(iRdm) != null);//不为空才会再生成
                //马上设置生成码缓存
                var codeKey = CFG.微信绑定前缀 + iRdm.ToString();
                var ubcc = new UserBoundCodeCache();
                ubcc.UserId = UserID;
                ubcc.BoundCode = iRdm;
                CacheAccess.InsertToCacheByTime(codeKey, ubcc, 120);//两分钟内过期
                ubc.BoundCode = iRdm;
                CacheAccess.InsertToCacheByTime(key, ubc, 120);
            }
            return ubc;
        }

        /// <summary>
        /// 根据绑定代码取要绑定的用户
        /// </summary>
        /// <param name="boundCode"></param>
        /// <returns></returns>
        protected UserBoundCodeCache GetUserBoundCodeCache(int boundCode)
        {
            var key = CFG.微信绑定前缀 + boundCode.ToString();
            return CacheAccess.GetFromCache(key) as UserBoundCodeCache;
        }
        #endregion

        #region 答题缓存处理
        /// <summary>
        /// 从数据库中取用户，并生成缓存类
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

            //增加追问的处理
            #region 刚提的问题生成马币消费记录与分配记录
            //取出所有未分配记录的问题      "提问都不一定绑定用户"
            var qaRef = Guid.Parse(Reference.问答类别_问题);
            var curWeiXinAPP = Guid.Parse(CFG.邦马网_当前微信应用);
            var nonmbQA = qabll.All.Where(p => p.QARef == qaRef && p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.bmQADistributions.Count() == 0);    //必须是提问才分配,当前应用是微信APP。

            //取出所有未生成马币记录的提问用户
            var nonmbUid = nonmbQA.Select(p => p.WeiXinId).Distinct();
            LogHelper.Write("新提问的用户数" + nonmbUid.Count().ToString(), LogHelper.LogMessageType.Debug);
            //区分出已绑定与未绑定的用户ID            
            //绑定的用户ID
            var uwU = uwbll.All.Where(p => nonmbUid.Contains(p.WeiXinId) && p.WeiXinAPP == curWeiXinAPP);
            //绑定的用户微信ID
            var uwUid = uwU.Select(p => p.WeiXinId);

            //未绑定的用户ID
            var nonuwUID = nonmbUid.Where(p => !uwUid.Contains(p));

            //已绑定的用户取邦马币值，邦马币值大于0的取出来
            var uwUSid = uwU.Select(p => p.UserId);
            var defXFMB = Convert.ToDecimal(CFG.提问默认收费马币值);
            var UserBMB = numbbll.All.Where(p => uwUSid.Contains(p.UserId) && (p.NMB > defXFMB || p.NBB > defXFMB));
            LogHelper.Write("花邦马币提问的用户数" + UserBMB.Count().ToString(), LogHelper.LogMessageType.Debug);
            //生成马币记录
            //先生成收费的马币记录，收费问题分配给默认收费答题用户。未生成收费的马币记录，直接分配给默认免费答题用户
            var mbQAIds = new List<Guid>();
            var tempMB = Convert.ToDecimal(0);
            var tempBB = Convert.ToDecimal(0);
            var tempQACount = 0;
            foreach (var u in UserBMB)
            {
                tempMB = u.NMB.Value;
                tempBB = u.NBB.Value;
                //能取出的当前用户问题数
                tempQACount = Convert.ToInt32(((tempMB + tempBB) / defXFMB));
                var uwxid = uwU.FirstOrDefault(p => p.UserId == u.UserId).WeiXinId;
                //当前用户提问数
                var uqa = nonmbQA.Where(p => p.WeiXinId == uwxid).Take(tempQACount);
                foreach (var q in uqa)
                {
                    //有消费的问题记录做标记
                    mbQAIds.Add(q.ID);
                    var umbrModel = new bmUserMaBiRecord();
                    umbrModel.SourceRef = Guid.Parse(Reference.马币来源_消费);
                    if (tempBB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.马币类别_邦币);
                        tempBB -= defXFMB;
                    }
                    else if (tempMB >= defXFMB)
                    {
                        umbrModel.MaBiRef = Guid.Parse(Reference.马币类别_马币);
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
            LogHelper.Write("花邦马币提问的问题数" + mbQAIds.Count().ToString(), LogHelper.LogMessageType.Debug);
            //生成问题分配记录，收费的到收费的默认账号，免费的到免费的默认账号
            foreach (var q in nonmbQA)
            {
                //问题分配处理                
                var qaModel = new bmQADistribution();

                qaModel.ID = Guid.NewGuid();
                qaModel.QAId = q.ID;
                qaModel.DistributionTime = DateTime.Now;

                qaModel.RegTime = DateTime.Now;
                qaModel.ModTime = DateTime.Now;
                qaModel.FlagTrashed = false;
                qaModel.FlagDeleted = false;

                qaModel.Result = Guid.Parse(Reference.分配答题操作_待解答);
                if (mbQAIds.Contains(q.ID))
                {
                    qaModel.WeiXinId = CFG.默认收费问题微信号;
                }
                else
                {
                    qaModel.WeiXinId = CFG.默认免费问题微信号;
                }
                qadisbll.Insert(qaModel, false);
            }
            //bmumbBll.UpdateChanges();
            //qadisbll.UpdateChanges();

            #endregion

            var qastate = Guid.Parse(Reference.分配答题操作_待解答);
            #region 未解决的问题重新分配
            //默认用户未解决的问题修改为待解答，以超过配置的时间为准
            var acQADmn = 0 - Convert.ToInt32(CFG.未处理问题激活时间);
            var acQADdt = DateTime.Now.AddMinutes(acQADmn);
            var nonHandleRef = Guid.Parse(Reference.分配答题操作_未处理);
            //增加微信APP的判断
            var nonACQAD = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.ModTime < acQADdt && ConstList.DefaultDISUser.Contains(p.WeiXinId) && p.Result == nonHandleRef);
            LogHelper.Write((acQADdt.ToShortTimeString() + "手动更新用户缓存时未处理的问题数量" + nonACQAD.Count().ToString()), LogHelper.LogMessageType.Debug);
            foreach (var item in nonACQAD)
            {
                item.Result = qastate;
                item.ModTime = DateTime.Now;
            }
            #endregion
            //测试看这边不更新数据库，以下代码能执行不。
            //未解决的问题修改为待解答 结束            
            var todayST = DateTime.Now.AddHours(-24);
            //取所有的待解答的问题数量
            var djdqadis = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.Result == qastate);
            LogHelper.Write("待解答的总问题数" + djdqadis.Count().ToString(), LogHelper.LogMessageType.Debug);
            ///////////////////////取数量的方式要修改，提问的方式变了，不直接生成马币//////////////////////////
            var mabiqaCount = djdqadis.Where(p => p.bmQA.bmUserMaBiRecords.Sum(m => m.MaBiNum) < 0).Count();//提问是负数
            LogHelper.Write("待解答的收费问题数" + mabiqaCount.ToString(), LogHelper.LogMessageType.Debug);
            //免费的只取24小时内的提问记录，节省服务器资源 看到未解决的问题，不必奇怪，超过24小时了
            var nonmabiqaCount = djdqadis.Where(p => p.bmQA.bmUserMaBiRecords.Count() == 0 && p.bmQA.RegTime >= todayST).Count();
            LogHelper.Write("待解答的免费问题数" + nonmabiqaCount.ToString(), LogHelper.LogMessageType.Debug);

            var state = Guid.Parse(Reference.在线状态_在线);
            //用户待答题保有量
            var qaWaitCount = Convert.ToInt32(CFG.用户待答题保有量);

            //这边要过滤掉不活跃的在线用户,超过5分钟不活跃的，则不分配题目给他
            var nondismn = 0 - Convert.ToInt32(CFG.疑似退出时间);
            var nondisdt = DateTime.Now.AddMinutes(nondismn);

            //需要强制退出的配置
            var logoutMN = 0 - Convert.ToInt32(CFG.强制退出时间);
            var logoutdt = DateTime.Now.AddMinutes(logoutMN);

            var cu = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));
            var noncu = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));

            #region 重新分配开始

            //取要强退的认证用户 '超过7分钟未活跃'
            var noActiveCU = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && p.ActiveTime < logoutdt && ConstList.DTCertificationLevel.Contains(p.CertificationLevel));
            LogHelper.Write("需要强退的认证用户数" + noActiveCU.Count().ToString(), LogHelper.LogMessageType.Debug);
            //取要强退未认证用户
            var noActiveU = bll.All.Where(p => p.WeiXinAPP != null && p.WeiXinAPP == curWeiXinAPP && p.State == state && p.ActiveTime < logoutdt && (p.CertificationLevel == null || !ConstList.DTCertificationLevel.Contains(p.CertificationLevel)));
            LogHelper.Write("需要强退的未认证用户数" + noActiveU.Count().ToString(), LogHelper.LogMessageType.Debug);

            //取强退用户ID
            //存在不活跃的认证用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
            var noActiveCUWIDS = noActiveCU.Select(p => p.WeiXinId);
            //存在不活跃的认证用户，将不活跃用户的答题分配记录标识为放弃，并将答题分配给活跃用户
            var noActiveUWIDS = noActiveU.Select(p => p.WeiXinId);

            //取所有不活跃的答题用户，然后再区分出收费问题与免费问题
            var noActiveUCUQAD = qadisbll.All.Where(p => p.bmQA.WeiXinAPP != null && p.bmQA.WeiXinAPP == curWeiXinAPP && p.Result == qastate && (noActiveCUWIDS.Contains(p.WeiXinId) || p.WeiXinId == CFG.默认收费问题微信号 || noActiveUWIDS.Contains(p.WeiXinId) || p.WeiXinId == CFG.默认免费问题微信号));
            LogHelper.Write("总的未答题数" + noActiveUCUQAD.Count().ToString(), LogHelper.LogMessageType.Debug);
            var noActiveMQAD = noActiveUCUQAD.Where(p => p.bmQA.bmUserMaBiRecords.Sum(m => m.MaBiNum) < 0).OrderBy(p => p.bmQA.RegTime);
            LogHelper.Write("收费的未答题数" + noActiveMQAD.Count().ToString(), LogHelper.LogMessageType.Debug);

            //收费问题分配
            //取活跃在线的认证答题用户,排除掉默认分配用户，省的多余操作
            var certificationUsers = cu.Where(p => p.WeiXinId != CFG.默认收费问题微信号 && p.ActiveTime >= nondisdt).OrderByDescending(p => p.ActiveNum);
            if (mabiqaCount > 0)
            {
                int selectCount = mabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                certificationUsers = certificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }
            LogHelper.Write("待分配答题的认证用户数" + certificationUsers.Count().ToString(), LogHelper.LogMessageType.Debug);

            if (certificationUsers.Count() > 0)
            {//存在在线用户时   
                if (noActiveMQAD.Count() > 0)
                {//待答题数据重新分配
                    var ouCount = certificationUsers.Count();
                    var i = 0;
                    foreach (var item in noActiveMQAD)
                    {
                        //对答题进行分配
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
            {//无在线活跃用户，系统将答题回收给默认用户                 
                if (noActiveMQAD.Count() > 0)
                { //将答题回收给默认答题用户
                    foreach (var item in noActiveMQAD)
                    {
                        item.WeiXinId = CFG.默认收费问题微信号;
                        item.ModTime = DateTime.Now;
                    }
                }

            }
            //收费问题分配结束
            //免费问题分配  当天的分配给活跃未认证用户，非当天的分配给默认用户
            var noActiveNMQAD = noActiveUCUQAD.Where(p => p.bmQA.bmUserMaBiRecords.Count() == 0).OrderBy(p => p.bmQA.RegTime);
            LogHelper.Write("免费的未答题数" + noActiveNMQAD.Count().ToString(), LogHelper.LogMessageType.Debug);

            //未认证的用户处理
            var noncertificationUsers = noncu.Where(p => p.WeiXinId != CFG.默认免费问题微信号 && p.ActiveTime >= nondisdt).OrderByDescending(p => p.ActiveNum);
            if (nonmabiqaCount > 0)
            {
                int selectCount = nonmabiqaCount / qaWaitCount;
                if (selectCount == 0)
                    selectCount = 1;
                noncertificationUsers = noncertificationUsers.Take(selectCount).OrderByDescending(p => p.ActiveNum);
            }
            LogHelper.Write("待分配答题的未认证用户数" + noncertificationUsers.Count().ToString(), LogHelper.LogMessageType.Debug);

            if (noncertificationUsers.Count() > 0)
            {
                if (noActiveNMQAD.Count() > 0)
                {//待答题数据重新分配
                    var ouCount = noncertificationUsers.Count();
                    var i = 0;
                    foreach (var item in noActiveNMQAD)
                    {
                        //判断是否是当天的问题
                        if (item.bmQA.RegTime >= todayST)
                        {
                            //对答题进行分配
                            i++;
                            var j = i % ouCount;
                            if (j == 0)
                                j = ouCount;
                            var disOU = noncertificationUsers.Skip(j - 1).Take(1).FirstOrDefault();
                            item.WeiXinId = disOU.WeiXinId;
                            item.ModTime = DateTime.Now;
                        }
                        else
                        {//非当天的问题回收给默认微信号
                            if (item.WeiXinId != CFG.默认免费问题微信号)
                                item.WeiXinId = CFG.默认免费问题微信号;
                        }
                    }
                }
            }
            else
            {
                if (noActiveNMQAD.Count() > 0)
                { //将答题回收给默认答题用户
                    foreach (var item in noActiveNMQAD)
                    {
                        item.WeiXinId = CFG.默认免费问题微信号;
                        item.ModTime = DateTime.Now;
                    }
                }
            }
            //免费问题分配结束
            #endregion 重新分配结束

            #region 移除用户和他的答题缓存，数据库更新
            //强制退出不活跃用户  并将不活跃用户的答题缓存清空  
            var tqState = Guid.Parse(Reference.在线状态_退出);
            foreach (var item in noActiveCU)
            {
                //不修改修改时间 
                item.AQEndTime = DateTime.Now;
                item.State = tqState;
                CacheAccess.RemoveCache(CFG.用户待答题缓存键前缀 + item.WeiXinId);
            }
            foreach (var item in noActiveU)
            {
                item.AQEndTime = DateTime.Now;
                item.State = tqState;
                CacheAccess.RemoveCache(CFG.用户待答题缓存键前缀 + item.WeiXinId);
            }
            //统一更新进数据库
            qadisbll.UpdateChanges();
            bll.UpdateChanges();
            //不活跃用户处理结束
            #endregion

            //生成缓存对象
            var model = new OnlineQAUserCache();
            model.RefreshTime = DateTime.Now;

            //收费待答题数量
            model.MaBiQACount = mabiqaCount;
            //免费待答题数量
            model.NonMaBiQACount = nonmabiqaCount;
            model.CertificationUser = cu.OrderByDescending(p => p.ActiveNum);
            model.NonCertificationQAUser = noncu.OrderByDescending(p => p.ActiveNum);

            LogHelper.Write("手动更新用户缓存结束", LogHelper.LogMessageType.Debug);

            return model;
        }
        #endregion

        #region 微信TOK缓存
        protected string SetWXTKCache()
        {
            var atURL = CFG.邦马网_获取AT网址.Replace("APPID", CFG.邦马网_应用ID).Replace("APPSECRET", CFG.邦马网_应用密钥);
            //LogHelper.Write("获取微信ToKen的URL" + atURL, LogHelper.LogMessageType.Info);
            var atS = GetHtmlHelper.GetPage(atURL, "");
            var wsTKJson = JsonConvert.DeserializeObject<wxTKJson>(atS);
            //LogHelper.Write("获取微信ToKen" + wsTKJson.access_token, LogHelper.LogMessageType.Info);
            if (!String.IsNullOrEmpty(wsTKJson.errcode) || !String.IsNullOrEmpty(wsTKJson.errmsg))
            {
                LogHelper.Write("获取微信ToKen失败" + wsTKJson.errcode + " " + wsTKJson.errmsg, LogHelper.LogMessageType.Error);
            }
            else
            {
                //保存到缓存中
                CacheAccess.AddToCacheByTime(CFG.邦马网_AT缓存键, wsTKJson.access_token, 5400);
            }

            return wsTKJson.access_token;
        }

        public string GetWXTKCache()
        {
            var s = CacheAccess.GetFromCache(CFG.邦马网_AT缓存键) as string;
            if(String.IsNullOrEmpty(s))
            {
                s = SetWXTKCache();
            }
            return s;
        }

        #endregion

    }
}
