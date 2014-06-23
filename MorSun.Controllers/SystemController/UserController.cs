using System.Web.Mvc;
using dotNetMembership = System.Web.Security.Membership;
using dotNetRoles = System.Web.Security.Roles;
using System;
using System.Text;
using MorSun.Model;
using MorSun.Bll;
using System.Collections.Generic;
using System.Web.Security;
using MorSun.Controllers.Filter;
using System.Collections;
using MorSun.Common;
using System.Linq;
using HOHO18.Common.DEncrypt;
using MorSun.Controllers.ViewModel;
using HOHO18.Common;
using HOHO18.Common.Base;
using System.Data;


namespace MorSun.Controllers
{
    [HandleError]
    public class UserController : BasisController
    {

        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
            base.Initialize(requestContext);
        }


        /// <summary>
        /// 获取用于自动补全autoComplete的JSON数据，有固定的格式要求
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAutocompleteList(UserVModel vmodel)
        {
            vmodel.IsApproved = "1";
            var res = vmodel.List;
            return Json((from u in res
                         select new
                         {
                             value = u.UserId
                             ,
                             label = u.wmfUserInfo.TrueName,
                         }).Take(10), JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Index(UserVModel model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.查看))
            {                
                return View(model);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }

        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult Detial(Guid? UId, int? PIndex)
        {
            if (UId == null || UId == Guid.Empty)
            {
                if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.个人资料, MorSun.Common.Privelege.操作.查看))
                {
                    var model = new BaseBll<aspnet_Users>().All.FirstOrDefault(r => r.UserName == User.Identity.Name);
                    return View(model);
                }
                else
                {
                    return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
                }
            }
            else
            {
                if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.个人资料, MorSun.Common.Privelege.操作.修改))
                {
                    //员工管理编辑
                    var model = new BaseBll<aspnet_Users>().All.FirstOrDefault(r => r.UserId == UId);
                    model.State = "manage";
                    //返回页码
                    model.PIndex = PIndex;
                    return View(model);
                }
                else
                {
                    return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
                }
            }
        }

        [Authorize]
        [ExceptionFilter()]
        public virtual ActionResult DocumentManage(Guid? UId, string scrollTo, int? PIndex)
        {
            if (UId == null || UId == Guid.Empty)
            {
                if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.个人资料, MorSun.Common.Privelege.操作.查看))
                {
                    var model = new BaseBll<aspnet_Users>().All.FirstOrDefault(r => r.UserName == User.Identity.Name);
                    return View(model);
                }
                else
                {
                    return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
                }
            }
            else
            {
                if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.个人资料, MorSun.Common.Privelege.操作.修改))
                {
                    //员工管理编辑
                    var model = new BaseBll<aspnet_Users>().All.FirstOrDefault(r => r.UserId == UId);
                    model.State = "manage";
                    model.scrollTo = scrollTo;
                    //返回页码
                    model.PIndex = PIndex;
                    return View(model);
                }
                else
                {
                    return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
                }
            }
        }


        //创建新员工
        public virtual string Create(aspnet_Users model, wmfUserInfo userinfoModel, wmfUserDeptPosition userDeptPositionModel)
        {

            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.添加))
            {
                //判断
                string msg = string.Empty;
                if (String.IsNullOrEmpty(model.UserName) || model.UserName.Trim() == "")
                    msg += XmlHelper.GetKeyNameValidation<aspnet_Users>("用户名不能为空") + "<br/>";
                else
                {
                    if (!String.IsNullOrEmpty(model.UserName) && !ModelStateValidate.IsUsername(model.UserName.Trim()))
                        msg += XmlHelper.GetKeyNameValidation<aspnet_Users>("用户名长度") + "<br/>";
                }
                if (String.IsNullOrEmpty(model.Password) || model.Password.Trim() == "")
                    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码不能为空") + "<br/>";
                else
                {
                    if (model.Password.Length < 6)
                        msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码长度") + "<br/>";
                }
                if (String.IsNullOrEmpty(model.Password2) || model.Password2.Trim() == "")
                    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("确认密码不能为空") + "<br/>";
                else
                {
                    if (!String.IsNullOrEmpty(model.Password) && !String.IsNullOrEmpty(model.Password2) && model.Password != model.Password2)
                        msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码不一致") + "<br/>";
                }
                if (String.IsNullOrEmpty(userinfoModel.TrueName) || userinfoModel.TrueName.Trim() == "")
                    msg += XmlHelper.GetKeyNameValidation<wmfUserInfo>("真实姓名不能为空") + "<br/>";


                if (String.IsNullOrEmpty(model.Email) || model.Email.Trim() == "")
                    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("电子邮箱不能为空") + "<br/>";
                if (!String.IsNullOrEmpty(model.Email) && !ModelStateValidate.IsEmail(model.Email.Trim()))
                    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("电子邮箱格式不正确") + "<br/>";

                var aspnetUserBll = new BaseBll<aspnet_Users>();

                var aspnetusers = aspnetUserBll.All.FirstOrDefault(r => r.UserName == model.UserName);
                if (aspnetusers != null)
                {
                    //该用户名已经存在，请重新输入！
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("用户名已经存在！"), "") });
                }

                if (msg != string.Empty)
                {
                    return getErrListJson(new[] { new RuleViolation(msg, "") });
                }

                // 尝试注册用户
                var createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);
                //水务工程这边有问题，一判断就出错
                //if (createStatus == MembershipCreateStatus.Success)
                //{
                    
                    //查询出新注册的用户信息
                    var user = Membership.GetUser(model.UserName);
                    var userinfobll = new BaseBll<wmfUserInfo>();

                    var userDeptPositionBll = new BaseBll<wmfUserDeptPosition>();

                    //密码加密
                    userinfoModel.UserPassword = DESEncrypt.Encrypt(model.Password);
                    userinfoModel.OperatePassword = DESEncrypt.Encrypt(model.Password);
                    userinfoModel.ID = user.ProviderUserKey.ToAs<Guid>();

                    if (model.RoleId != null)
                    {
                        //水务这边不支持下面注释的代码
                        //Guid roleID = Guid.Parse(model.RoleId);
                        //var aspnetroles = new RoleVModel().All.FirstOrDefault(t => t.RoleId == roleID);

                        ////添加角色
                        //dotNetRoles.AddUserToRole(model.UserName, aspnetroles.RoleName);

                        var constr = @"Insert Into aspnet_UsersInRoles ([UserId],[RoleId])  VALUES ('" + userinfoModel.ID + "','" + model.RoleId + "')";
                        //var constr1 = @"INSERT INTO aspnet_UsersInRoles ([UserId] ,[RoleId])  VALUES  ('" + model.UserId + "'  ,'" + aspnetroles.RoleId + "')";
                        aspnetUserBll.Db.ExecuteStoreCommand(constr);
                    }
                    //昵称默认为用户的登录名
                    userinfoModel.NickName = model.UserName;
                    //注册时间
                    userinfoModel.RegTime = DateTime.Now;
                    //最后退出时间
                    userinfoModel.LastLogTime = DateTime.Now;

                    var userDeptPosition = new wmfUserDeptPosition();
                    userDeptPosition.UserId = user.ProviderUserKey.ToAs<Guid>();
                    if (userDeptPositionModel.DeptId != null && userDeptPositionModel.DeptId != Guid.Empty)
                    {
                        userDeptPosition.DeptId = userDeptPositionModel.DeptId;
                    }
                    if (userDeptPositionModel.PostionId != null && userDeptPositionModel.PostionId != Guid.Empty)
                    {
                        userDeptPosition.PostionId = userDeptPositionModel.PostionId;
                    }

                    //保存用户信息到 wmfuserinfo 表中
                    userinfobll.Insert(userinfoModel, false);

                    //插入操作日志
                    InsertLog(userDeptPosition.ID, "wmfUserDeptPosition", "添加", "", "");



                    //保存到员工与部门表
                    userDeptPositionBll.Insert(userDeptPosition);

                    //插入操作日志
                    InsertLog(userinfoModel.ID, "wmfuserinfo", "添加", "", "");
                    return "true";
                //}
                //else
                //{
                //    //该用户名已经存在，请重新输入！
                //    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("用户注册出错！" + createStatus + ""), "") });
                //}
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        /// <summary>
        /// 导入员工数据
        /// </summary>
        /// <returns></returns>
        public virtual string ImportUserData()
        {
            var ret = string.Empty;


            var roleId = HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("RoleName");
            ExcleHelper exHelper = new ExcleHelper();

            var dataSource = Server.MapPath("/UploadFile/ImportDataFile/用户档案.xls");
            if (!System.IO.File.Exists(dataSource))
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请上传导入文档"), "") });
            }
            var dsList = exHelper.ExcleToDataSet(dataSource, "Sheet1");

            var dsTable = exHelper.DataSetAddColumnName(dsList); ;

            if (dsTable != null && dsTable.Rows.Count > 0)
            {
                for (int i = 0; i < dsTable.Rows.Count; i++)
                {
                    aspnet_Users model = new aspnet_Users();
                    wmfUserInfo userinfoModel = new wmfUserInfo();
                    wmfUserDeptPosition userDeptPositionModel = new wmfUserDeptPosition();
                    DataRowToModel(dsTable.Rows[i], model, userinfoModel, userDeptPositionModel, roleId);
                    Create(model, userinfoModel, userDeptPositionModel);
                }
                ret = "true";
            }

            return ret;
        }

        /// <summary>
        /// 填空数据
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="model">aspnet_Users</param>
        /// <param name="userinfoModel">wmfUserInfo</param>
        /// <param name="userDeptPositionModel">wmfUserDeptPosition</param>
        /// <param name="roleId">角色ID</param>
        public virtual void DataRowToModel(DataRow dr, aspnet_Users model, wmfUserInfo userinfoModel, wmfUserDeptPosition userDeptPositionModel, string roleId)
        {
            model.UserName = dr["姓名"].ToString();
            model.Password = "123456";
            model.Password2 = model.Password;
            model.Email = "123@qq.com";
            userinfoModel.TrueName = dr["姓名"].ToString();
            userinfoModel.Sex = dr["性别"].ToString();
            userinfoModel.CheckNumber = dr["考勤号"].ToString();
            userinfoModel.BirthDay = ToDateTimeFromString(dr["出生年月"].ToString());
            //userinfoModel.PoliticsStatus = dr["政治面貌"].ToString();
            //userinfoModel.JoinWorkTime = ToDateTimeFromString(dr["参加工作时间"].ToString());
            userinfoModel.IdCard = dr["身份证"].ToString();

            #region 通过部门名查询部门ID，并填充到userDeptPositionModel中
            var deptName = dr["部门"].ToString();
            var companyName = dr["公司名"].ToString();
            var companyID = Guid.Empty;
            var deptBll = new BaseBll<wmfDept>();
            //判断公司名是否为空
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                companyName = companyName.Trim();
                var companyModel = deptBll.All.FirstOrDefault(u => string.Compare(u.DeptName, companyName, true) == 0);
                if (companyModel != null)
                {
                    companyID = companyModel.ID;
                }
            }
            if (!string.IsNullOrEmpty(deptName))
            {
                deptName = deptName.Trim();
                wmfDept deptModel = null;
                if (companyID != Guid.Empty)
                {
                    deptModel = deptBll.All.FirstOrDefault(u =>u.ParentId==companyID && string.Compare(u.DeptName, deptName, true) == 0);
                }
                else
                {
                    deptModel = deptBll.All.FirstOrDefault(u => string.Compare(u.DeptName, deptName, true) == 0);
                }
                if (deptModel != null)
                {
                    userDeptPositionModel.DeptId = deptModel.ID;
                }
            }

            #endregion

            #region 通过岗位名查询出岗位ID，并填充到userDeptPosition

            var positionName = dr["岗位"].ToString();
            if (!string.IsNullOrWhiteSpace(positionName))
            {
                var positionBll = new BaseBll<wmfPosition>().All.Where(u => u.FlagDeleted != true && u.FlagTrashed != true);
                //筛选出同一个部门同岗位名称的positionModel
                var positionModel = positionBll.FirstOrDefault(u => string.Compare(u.PositionName, positionName, true) == 0 && u.DeptId == userDeptPositionModel.DeptId);
                if (positionModel != null)
                {
                    userDeptPositionModel.PostionId = positionModel.ID;
                }
            }

            #endregion

            //userinfoModel.TypeOfWork = dr["工种"].ToString();
            //userinfoModel.StandardOfCulture = dr["文化程度"].ToString();

            #region 毕业院校、专业、毕业时间等

            var schoolsStr = dr["毕业院校"].ToString();
            var professionsStr = dr["专业"].ToString();
            var schoolEndtimesStr = dr["毕业时间"].ToString();

            //毕业院校不为空
            if (!string.IsNullOrWhiteSpace(schoolsStr))
            {
                var schoolArr = schoolsStr.Split(',');
                var professionArr = professionsStr.Split(',');
                var schoolEndtimesArr = schoolEndtimesStr.Split(',');
                var schoolArrCount = schoolArr.Count();
                var professionArrCount = professionArr.Count();
                var schoolEndtimeArrCount = schoolEndtimesArr.Count();

                //if (schoolArrCount == professionArrCount && schoolArrCount == schoolEndtimeArrCount)
                //{
                //    //毕业院校、专业、毕业时间的总数相同
                //    switch (schoolArrCount)
                //    {
                //        case 1:
                //            userinfoModel.School1 = schoolArr[0];
                //            userinfoModel.SchoolEnd1 = ToDateTimeFromString(schoolEndtimesArr[0]);
                //            userinfoModel.Profession1 = professionArr[0];
                //            break;
                //        case 2:
                //            userinfoModel.School1 = schoolArr[0];
                //            userinfoModel.SchoolEnd1 = ToDateTimeFromString(schoolEndtimesArr[0]);
                //            userinfoModel.Profession1 = professionArr[0];

                //            userinfoModel.School2 = schoolArr[1];
                //            userinfoModel.SchoolEnd2 = ToDateTimeFromString(schoolEndtimesArr[1]);
                //            userinfoModel.Profession2 = professionArr[1];
                //            break;
                //        case 3:
                //            userinfoModel.School1 = schoolArr[0];
                //            userinfoModel.SchoolEnd1 = ToDateTimeFromString(schoolEndtimesArr[0]);
                //            userinfoModel.Profession1 = professionArr[0];

                //            userinfoModel.School2 = schoolArr[1];
                //            userinfoModel.SchoolEnd2 = ToDateTimeFromString(schoolEndtimesArr[1]);
                //            userinfoModel.Profession2 = professionArr[1];

                //            userinfoModel.School3 = schoolArr[2];
                //            userinfoModel.SchoolEnd3 = ToDateTimeFromString(schoolEndtimesArr[2]);
                //            userinfoModel.Profession3 = professionArr[2];
                //            break;
                //        default:
                //            throw new Exception("院校太多" + schoolArrCount);
                //            break;
                //    }
                //}
            }
            #endregion

            //userinfoModel.Job = dr["职务\\职称"].ToString();

            #region 证书
            //userinfoModel.Certificate1 = dr["证书名1"].ToString();
            //userinfoModel.CertificateID1 = dr["证书编号1"].ToString();
            //double rt1Double = 0;
            //if (double.TryParse(dr["证书复检时间1"].ToString(), out rt1Double))
            //{
            //    userinfoModel.RecheckTime1 = DateTime.FromOADate(rt1Double);
            //}

            //userinfoModel.Certificate2 = dr["证书名2"].ToString();
            //userinfoModel.CertificateID2 = dr["证书编号2"].ToString();
            //double rt2Double = 0;
            //if (double.TryParse(dr["证书复检时间2"].ToString(), out rt2Double))
            //{
            //    userinfoModel.RecheckTime2 = DateTime.FromOADate(rt1Double);
            //}

            //userinfoModel.Certificate3 = dr["证书名3"].ToString();
            //userinfoModel.CertificateID3 = dr["证书编号3"].ToString();
            //double rt3Double = 0;
            //if (double.TryParse(dr["证书复检时间3"].ToString(), out rt3Double))
            //{
            //    userinfoModel.RecheckTime3 = DateTime.FromOADate(rt3Double);
            //}

            //userinfoModel.Certificate4 = dr["证书名4"].ToString();
            //userinfoModel.CertificateID4 = dr["证书编号4"].ToString();
            //double rt4Double = 0;
            //if (double.TryParse(dr["证书复检时间4"].ToString(), out rt4Double))
            //{
            //    userinfoModel.RecheckTime4 = DateTime.FromOADate(rt4Double);
            //}

            //userinfoModel.Certificate5 = dr["证书名5"].ToString();
            //userinfoModel.CertificateID5 = dr["证书编号5"].ToString();
            //double rt5Double = 0;
            //if (double.TryParse(dr["证书复检时间5"].ToString(), out rt5Double))
            //{
            //    userinfoModel.RecheckTime5 = DateTime.FromOADate(rt5Double);
            //}

            //userinfoModel.Certificate6 = dr["证书名6"].ToString();
            //userinfoModel.CertificateID6 = dr["证书编号6"].ToString();
            //double rt6Double = 0;
            //if (double.TryParse(dr["证书复检时间6"].ToString(), out rt6Double))
            //{
            //    userinfoModel.RecheckTime6 = DateTime.FromOADate(rt6Double);
            //}

            //userinfoModel.Certificate7 = dr["证书名7"].ToString();
            //userinfoModel.CertificateID7 = dr["证书编号7"].ToString();
            //double rt7Double = 0;
            //if (double.TryParse(dr["证书复检时间7"].ToString(), out rt7Double))
            //{
            //    userinfoModel.RecheckTime7 = DateTime.FromOADate(rt7Double);
            //}

            //userinfoModel.Certificate8 = dr["证书名8"].ToString();
            //userinfoModel.CertificateID8 = dr["证书编号8"].ToString();
            //double rt8Double = 0;
            //if (double.TryParse(dr["证书复检时间8"].ToString(), out rt8Double))
            //{
            //    userinfoModel.RecheckTime8 = DateTime.FromOADate(rt8Double);
            //}
            #endregion

            model.RoleId = roleId;
        }
        /// <summary>
        /// 将原有的字符串（如：1998.07）转换成日期类型
        /// </summary>
        /// <param name="originalStr"></param>
        /// <returns></returns>
        private DateTime? ToDateTimeFromString(string originalStr)
        {
            DateTime? result = null;
            if (!string.IsNullOrWhiteSpace(originalStr))
            {
                var tpDate = DateTime.MinValue;
                if (DateTime.TryParse(originalStr, out tpDate))
                {
                    result = tpDate;
                }
            }
            return result;
        }

        //彻底删除员工
        public virtual string Delete(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.彻底删除))
            {
                var userinfobll = new BaseBll<wmfUserInfo>();
                var userinfoModel = userinfobll.GetModel(model.UserId);
                if (userinfoModel != null)
                {
                    userinfobll.Delete(userinfoModel, false);
                }
                var userDeptPositionBll = new BaseBll<wmfUserDeptPosition>();
                var userDeptPositionModel = userDeptPositionBll.All.FirstOrDefault(r => r.UserId == model.UserId);
                if (userDeptPositionModel != null)
                {
                    userDeptPositionBll.Delete(userDeptPositionModel, false);
                    //插入操作日志
                    InsertLog(null, "wmfUserDeptPosition", "删除", "", "");
                }
                var userBll = new BaseBll<aspnet_Users>();
                var userModel = userBll.GetModel(model.UserId);
                if (userModel != null)
                {
                    userBll.Delete(userModel);
                    //插入操作日志
                    InsertLog(null, "aspnet_Users", "删除", "", "");
                }
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //批量彻底删除员工
        public virtual string BatchDelete(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.彻底删除))
            {
                if (string.IsNullOrEmpty(model.CheckedId))
                {
                    //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量彻底删除的员工"), "") });
                }
                string[] ids = model.CheckedId.Split(',');
                if (ids[0] == "")
                {
                    //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量彻底删除的员工"), "") });
                }

                var userinfobll = new BaseBll<wmfUserInfo>();
                var userDeptPositionBll = new BaseBll<wmfUserDeptPosition>();
                var userBll = new BaseBll<aspnet_Users>();
                wmfUserInfo userinfoModel = null;
                wmfUserDeptPosition userDeptPositionModel = null;
                aspnet_Users userModel = null;
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        Guid userid = Guid.Parse(ids[i]);
                        userinfoModel = userinfobll.GetModel(userid);
                        if (userinfoModel != null)
                        {
                            userinfobll.Delete(userinfoModel, false);
                        }

                        userDeptPositionModel = userDeptPositionBll.All.FirstOrDefault(r => r.UserId == userid);
                        if (userDeptPositionModel != null)
                        {
                            userDeptPositionBll.Delete(userDeptPositionModel, false);
                        }

                        userModel = userBll.GetModel(userid);
                        if (userModel != null)
                        {
                            userBll.Delete(userModel, false);
                        }
                    }
                }
                userBll.UpdateChanges();
                //插入操作日志
                InsertLog(null, "aspnet_Users", "批量删除", "", "");
                return "true";
            }
            else
            {
                //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //更新
        public virtual string Edit(aspnet_Users model, wmfUserInfo userinfoModel, wmfUserDeptPosition userDeptPositionModel)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.个人资料, MorSun.Common.Privelege.操作.修改) || MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.修改))
            {
                //判断
                string msg = string.Empty;
                if (String.IsNullOrEmpty(model.UserName) || model.UserName.Trim() == "")
                    msg += XmlHelper.GetKeyNameValidation<aspnet_Users>("用户名不能为空") + "<br/>";
                else
                {
                    if (!String.IsNullOrEmpty(model.UserName) && !ModelStateValidate.IsUsername(model.UserName.Trim()))
                        msg += XmlHelper.GetKeyNameValidation<aspnet_Users>("用户名长度") + "<br/>";
                }
                //if (String.IsNullOrEmpty(model.Password) || model.Password.Trim() == "")
                //    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码不能为空") + "<br/>";
                //else
                //{
                //    if (model.Password.Length < 6)
                //        msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码长度") + "<br/>";
                //}
                //if (String.IsNullOrEmpty(model.Password2) || model.Password2.Trim() == "")
                //    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("确认密码不能为空") + "<br/>";
                //else
                //{
                //    if (!String.IsNullOrEmpty(model.Password) && !String.IsNullOrEmpty(model.Password2) && model.Password != model.Password2)
                //        msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码不一致") + "<br/>";
                //}
                //if (String.IsNullOrEmpty(userinfoModel.TrueName) || userinfoModel.TrueName.Trim() == "")
                //    msg += XmlHelper.GetKeyNameValidation<wmfUserInfo>("真实姓名不能为空") + "<br/>";
                //if (String.IsNullOrEmpty(model.Email) || model.Email.Trim() == "")
                //    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("电子邮箱不能为空") + "<br/>";
                //if (!String.IsNullOrEmpty(model.Email) && !ModelStateValidate.IsEmail(model.Email.Trim()))
                //    msg += XmlHelper.GetKeyNameValidation<aspnet_Membership>("电子邮箱格式不正确") + "<br/>";

                if (msg != string.Empty)
                {
                    //return getErrListJson(new[] { new RuleViolation(msg, "") });
                }

                //查询出新注册的用户信息
                var user = Membership.GetUser(model.UserName);
                var userinfobll = new BaseBll<wmfUserInfo>();
                var aspnetUserBll = new BaseBll<aspnet_Users>();
                var userDeptPositionBll = new BaseBll<wmfUserDeptPosition>();
                var memberBll = new BaseBll<aspnet_Membership>();
               
                var userinfo = userinfobll.GetModel(model.UserId);
                var deptposition = userDeptPositionBll.All.FirstOrDefault(r => r.UserId == model.UserId);
                var membership = memberBll.GetModel(model.UserId);
                var membershipBaseBll = new BaseSQL<aspnet_Membership>();
                var oldMembership = membershipBaseBll.GetModelByID(model.UserId.ToString(), "Aspnet_Membership", "UserId");
                var userinfoBaseBll = new BaseSQL<wmfUserInfo>();
                var oldUserinfo = userinfoBaseBll.GetModelByID(model.UserId.ToString(), "wmfUserInfo", "ID");
                var deptPositionBaseBll = new BaseSQL<wmfUserDeptPosition>();
                var oldDeptPosition = deptPositionBaseBll.GetModelByID(model.UserId.ToString(), "wmfUserDeptPosition", "UserId");

                //密码加密
                //userinfo.UserPassword = DESEncrypt.Encrypt(model.Password);
                //membership.Email = model.Email;



                TryUpdateModel<aspnet_Membership>(membership);
                TryUpdateModel<wmfUserInfo>(userinfo);
                if (deptposition == null)
                {
                    var userDeptPosition = new wmfUserDeptPosition();
                    userDeptPosition.UserId = user.ProviderUserKey.ToAs<Guid>();
                    if (userDeptPositionModel.DeptId != null && userDeptPositionModel.DeptId != Guid.Empty)
                    {
                        userDeptPosition.DeptId = userDeptPositionModel.DeptId;
                    }
                    if (userDeptPositionModel.PostionId != null && userDeptPositionModel.PostionId != Guid.Empty)
                    {
                        userDeptPosition.PostionId = userDeptPositionModel.PostionId;
                    }
                    //保存到员工与部门表
                    userDeptPositionBll.Insert(userDeptPosition);
                }
                if (deptposition != null)
                {
                    TryUpdateModel<wmfUserDeptPosition>(deptposition);
                }

                //执行更新
                userDeptPositionBll.Update(deptposition);

                //var originalContent = string.Empty;
                //var afterOperateContent = string.Empty;
                //var Errormsg = string.Empty;

                //Errormsg = userinfoModel.Equal(oldUserinfo, out originalContent, out afterOperateContent);
                //if (Errormsg == "true")
                //{
                //    InsertLog(userinfo.ID, "wmfUserInfo", "编辑", originalContent, afterOperateContent);
                //}
                //if (deptposition != null)
                //{
                //    Errormsg = deptposition.aspnet_Users.aspnet_Membership.Equal(oldMembership, out originalContent, out afterOperateContent);
                //}
                //if (Errormsg == "true")
                //{
                //    if (deptposition != null)
                //    {
                //        InsertLog(deptposition.ID, "aspnet_Membership", "编辑", originalContent, afterOperateContent);
                //    }
                //}
                //if (oldDeptPosition != null)
                //{
                //    if (userDeptPositionModel != null)
                //    {
                //        Errormsg = userDeptPositionModel.Equal(oldDeptPosition, out originalContent, out afterOperateContent);
                //    }
                //}
                //if (Errormsg == "true")
                //{
                //    if (deptposition != null)
                //    {
                //        InsertLog(deptposition.ID, "wmfUserDeptPosition", "编辑", originalContent, afterOperateContent);
                //    }
                //}

                #region 64位系统不支持，换种方式
                ////获取用户的角色
                //var userInRoles = aspnetUserBll.All.FirstOrDefault(r => r.UserName == model.UserName).aspnet_Roles.FirstOrDefault();
                //if (userInRoles != null)
                //{
                //    //移除角色 有些服务器不兼容下一句，改用SQL方式
                //    dotNetRoles.RemoveUserFromRole(model.UserName, userInRoles.RoleName);                    
                //}

                //if (!string.IsNullOrEmpty(model.RoleId))
                //{
                //    Guid roleID = Guid.Parse(model.RoleId);
                //    var aspnetroles = new RoleVModel().All.FirstOrDefault(t => t.RoleId == roleID);                   
                //    dotNetRoles.AddUserToRole(model.UserName, aspnetroles.RoleName);
                //}
                #endregion

                #region 64位系统不支持，换种方式
                //获取用户的角色
                var userInRoles = aspnetUserBll.All.FirstOrDefault(r => r.UserName == model.UserName).aspnet_Roles.FirstOrDefault();
                //if (userInRoles != null)
                //{
                //    //移除角色 有些服务器不兼容下一句，改用SQL方式
                //    //dotNetRoles.RemoveUserFromRole(model.UserName, userInRoles.RoleName);
                //    var constr = @"INSERT INTO aspnet_UsersInRoles ([UserId] ,[RoleId])  VALUES  ('" + model.UserId + "'  ,'" + aspnetroles.RoleId + "')";
                //    aspnetUserBll.Db.ExecuteStoreCommand(constr);
                //}

                if (!string.IsNullOrEmpty(model.RoleId))
                {
                    //Guid roleID = Guid.Parse(model.RoleId);
                    //var aspnetroles = new RoleVModel().All.FirstOrDefault(t => t.RoleId == roleID);
                    //var roleName = "工程二部管理组组长";//aspnetroles.RoleName;
                    //添加角色   遇到一些服务器下面这一句无法执行，改用SQL做试试
                    //dotNetRoles.AddUserToRole(model.UserName, roleName);
                    var constr = @"UPDATE aspnet_UsersInRoles SET [RoleId] = '" + model.RoleId + "' WHERE [UserId] = '" + model.UserId + "'";
                    //var constr1 = @"INSERT INTO aspnet_UsersInRoles ([UserId] ,[RoleId])  VALUES  ('" + model.UserId + "'  ,'" + aspnetroles.RoleId + "')";
                    aspnetUserBll.Db.ExecuteStoreCommand(constr);

                }
                #endregion 
                ////更新密码
                //MembershipService.ChangePassword(model.UserName, user.ResetPassword(), model.Password);

                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        /// <summary>
        /// 初使化密码
        /// </summary>
        public virtual String IinitialPassword(aspnet_Users model)
        {
            var newPassword = "123456";
            MembershipUser user = Membership.GetUser(model.UserName);
            if (MembershipService.ChangePassword(model.UserName, user.ResetPassword(), newPassword))
            {
                var userinfobll = new BaseBll<wmfUserInfo>();
                var userinfo = userinfobll.GetModel(model.UserId);
                //密码加密
                userinfo.UserPassword = DESEncrypt.Encrypt(newPassword);
                userinfobll.Update(userinfo);
                //插入操作日志
                InsertLog(UserID, "aspnet_Users", HOHO18.Common.Web.webConfigHelp.GetWebConfigValue("编辑"), "管理员初使化密码", "管理员初使化密码");
                return "true";
            }
            else
            {
                ModelState.AddModelError("", "修改密码失败");
                return "修改密码失败";
            }
        }

        #region 离职
        //离职用户禁用登录
        public virtual String FlagTrashed(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.删除))
            {
                var user = Membership.GetUser(model.UserId);
                if (user == null) return "false";
                user.IsApproved = false;
                Membership.UpdateUser(user);

                var userinfobll = new BaseBll<wmfUserInfo>();
                wmfUserInfo userinfo = null;
                userinfo = userinfobll.GetModel(model.UserId);
                if (userinfo == null) return "false";
                userinfo.FlagTrashed = true;
                TryUpdateModel<wmfUserInfo>(userinfo);
                userinfobll.Update(userinfo);
                //插入操作日志
                InsertLog(userinfo.ID, "wmfUserInfo", "修改", "FlagTrashed:true", "FlagTrashed:false");
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //批量离职用户禁用登录
        [Authorize]
        [ExceptionFilter()]
        public virtual string BatchRecycle(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.删除))
            {
                if (string.IsNullOrEmpty(model.CheckedId))
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量离职的员工"), "") });
                }
                string[] ids = model.CheckedId.Split(',');
                if (ids[0] == "")
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量离职的员工"), "") });
                }
                var userinfobll = new BaseBll<wmfUserInfo>();
                MembershipUser user = null;
                wmfUserInfo userinfo = null;
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        user = Membership.GetUser(ids[i].ToAs<Guid>());
                        if (user == null) return "false";
                        user.IsApproved = false;
                        Membership.UpdateUser(user);

                        userinfo = userinfobll.GetModel(ids[i].ToAs<Guid>());
                        if (userinfo == null) return "false";
                        userinfo.FlagTrashed = true;
                        TryUpdateModel<wmfUserInfo>(userinfo);
                        userinfobll.Update(userinfo);
                    }
                }
                //插入操作日志
                InsertLog(null, "aspnet_Users", "批量删除到回收站", "", "");
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        /// <summary>
        /// 恢复离职用户,启用登录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aspnet_UsersDao"></param>
        /// <returns></returns>
        public virtual String UnFlagTrashed(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.回收站))
            {
                var user = Membership.GetUser(model.UserId);
                if (user == null) return "false";
                user.IsApproved = true;
                Membership.UpdateUser(user);

                var userinfobll = new BaseBll<wmfUserInfo>();
                wmfUserInfo userinfo = null;
                userinfo = userinfobll.GetModel(model.UserId);
                if (userinfo == null) return "true";
                userinfo.FlagTrashed = false;
                TryUpdateModel<wmfUserInfo>(userinfo);
                userinfobll.Update(userinfo);

                //插入操作日志
                InsertLog(userinfo.ID, "wmfUserInfo", "修改", "FlagTrashed:false", "FlagTrashed:true");

                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //批量恢复离职用户，启用登录
        [Authorize]
        [ExceptionFilter()]
        public virtual string BatchRecovery(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.回收站))
            {
                if (string.IsNullOrEmpty(model.CheckedId))
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量恢复的员工"), "") });
                }
                string[] ids = model.CheckedId.Split(',');
                if (ids[0] == "")
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量恢复的员工"), "") });
                }
                var userinfobll = new BaseBll<wmfUserInfo>();
                MembershipUser user = null;
                wmfUserInfo userinfo = null;
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        user = Membership.GetUser(ids[i].ToAs<Guid>());
                        if (user == null) return "false";
                        user.IsApproved = true;
                        Membership.UpdateUser(user);

                        userinfo = userinfobll.GetModel(ids[i].ToAs<Guid>());
                        if (userinfo == null) return "false";
                        userinfo.FlagTrashed = false;
                        TryUpdateModel<wmfUserInfo>(userinfo);
                        userinfobll.Update(userinfo);
                    }
                }
                //插入操作日志
                InsertLog(null, "aspnet_Users", "回收站批量还原", "", "");
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //离职员工
        [Authorize]
        [ExceptionFilter()]
        public ActionResult Recycle(UserVModel model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.回收站))
            {
                return View(model);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }
        #endregion


        #region 退休
        //离职用户禁用登录
        public virtual String FlagDeleted(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.删除))
            {
                var user = Membership.GetUser(model.UserId);
                if (user == null) return "false";
                user.IsApproved = false;
                Membership.UpdateUser(user);

                var userinfobll = new BaseBll<wmfUserInfo>();
                wmfUserInfo userinfo = null;
                userinfo = userinfobll.GetModel(model.UserId);
                if (userinfo == null) return "false";
                userinfo.FlagDeleted = true;
                TryUpdateModel<wmfUserInfo>(userinfo);
                userinfobll.Update(userinfo);
                //插入操作日志
                InsertLog(userinfo.ID, "wmfUserInfo", "修改", "FlagDeleted:true", "FlagDeleted:false");
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //批量离职用户禁用登录
        [Authorize]
        [ExceptionFilter()]
        public virtual string BatchFlagDeleted(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.删除))
            {
                if (string.IsNullOrEmpty(model.CheckedId))
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量退休的员工"), "") });
                }
                string[] ids = model.CheckedId.Split(',');
                if (ids[0] == "")
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量退休的员工"), "") });
                }
                var userinfobll = new BaseBll<wmfUserInfo>();
                MembershipUser user = null;
                wmfUserInfo userinfo = null;
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        user = Membership.GetUser(ids[i].ToAs<Guid>());
                        if (user == null) return "false";
                        user.IsApproved = false;
                        Membership.UpdateUser(user);

                        userinfo = userinfobll.GetModel(ids[i].ToAs<Guid>());
                        if (userinfo == null) return "false";
                        userinfo.FlagDeleted = true;
                        TryUpdateModel<wmfUserInfo>(userinfo);
                        userinfobll.Update(userinfo);
                    }
                }
                //插入操作日志
                InsertLog(null, "aspnet_Users", "批量退休", "", "");
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        /// <summary>
        /// 恢复离职用户,启用登录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aspnet_UsersDao"></param>
        /// <returns></returns>
        public virtual String UnFlagDeleted(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.回收站))
            {
                var user = Membership.GetUser(model.UserId);
                if (user == null) return "false";
                user.IsApproved = true;
                Membership.UpdateUser(user);

                var userinfobll = new BaseBll<wmfUserInfo>();
                wmfUserInfo userinfo = null;
                userinfo = userinfobll.GetModel(model.UserId);
                if (userinfo == null) return "true";
                userinfo.FlagDeleted = false;
                TryUpdateModel<wmfUserInfo>(userinfo);
                userinfobll.Update(userinfo);

                //插入操作日志
                InsertLog(userinfo.ID, "wmfUserInfo", "修改", "FlagTrashed:false", "FlagTrashed:true");

                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //批量恢复退休用户，启用登录
        [Authorize]
        [ExceptionFilter()]
        public virtual string BatchReFlagDeleted(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.回收站))
            {
                if (string.IsNullOrEmpty(model.CheckedId))
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量恢复的员工"), "") });
                }
                string[] ids = model.CheckedId.Split(',');
                if (ids[0] == "")
                {
                    return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("请选择要批量恢复的员工"), "") });
                }
                var userinfobll = new BaseBll<wmfUserInfo>();
                MembershipUser user = null;
                wmfUserInfo userinfo = null;
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        user = Membership.GetUser(ids[i].ToAs<Guid>());
                        if (user == null) return "false";
                        user.IsApproved = true;
                        Membership.UpdateUser(user);

                        userinfo = userinfobll.GetModel(ids[i].ToAs<Guid>());
                        if (userinfo == null) return "false";
                        userinfo.FlagDeleted = false;
                        TryUpdateModel<wmfUserInfo>(userinfo);
                        userinfobll.Update(userinfo);
                    }
                }
                //插入操作日志
                InsertLog(null, "aspnet_Users", "回收站批量还原", "", "");
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        //退休员工
        [Authorize]
        [ExceptionFilter()]
        public ActionResult ReFlagDeleted(UserVModel model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.回收站))
            {
                return View(model);
            }
            else
            {
                return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
            }
        }
        #endregion











        //员工数据导入
        public ActionResult UserInfoImport(aspnet_Users model, int? PIndex)
        {
            model.PIndex = PIndex;
            return View(model);
        }


        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aspnet_UsersDao"></param>
        /// <returns></returns>
        [Authorize]
        [ExceptionFilter()]
        public virtual String UnFlagLock(aspnet_Users model)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(MorSun.Common.Privelege.资源.员工, MorSun.Common.Privelege.操作.修改))
            {
                var user = Membership.GetUser(model.UserId);
                if (user == null) return "false";
                if (user.IsLockedOut)
                {
                    user.UnlockUser();
                }
                return "true";
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }

        /// <summary>
        /// 年假计算
        /// </summary>
        /// <param name="datesEmployed">入职时间</param>
        /// <returns></returns>
        public static double GetYearLeave(DateTime datesEmployed)
        {
            //年假天数
            var yearleaveDay = 0.0;
            if (datesEmployed == null)
            {
                return yearleaveDay;
            }

            //当年入职起始年月份
            var yeardatesEmployed = DateTime.Parse(datesEmployed.ToString("yyyy-01-01"));

            //当前时间
            var nowtime = DateTime.Now;

            //入职几年
            var rzyearNum = nowtime.Year - datesEmployed.Year;

            //总工作时间
            TimeSpan dd = new TimeSpan(nowtime.Ticks).Subtract(new TimeSpan(datesEmployed.Ticks)).Duration();

            //相差天数
            TimeSpan xctsdd = new TimeSpan(datesEmployed.Ticks).Subtract(new TimeSpan(yeardatesEmployed.Ticks)).Duration();

            //入职天数
            var zrzNum = dd.TotalDays;

            //相差天数
            var xctNum = xctsdd.TotalDays;

            //一年的天数
            var yearNum = 365;

            //未满一整年情况
            if (zrzNum < 365)
            {
                //年假0天
                yearleaveDay = 0;
            }
            //如果是第一年入职，考虑未满一整年情况
            else if ((rzyearNum == 1 || rzyearNum == 0) && zrzNum >= 365)
            {
                var sydayNum = yearNum - xctNum;
                //当年可享受的年假
                var yearleave = (sydayNum / 365) * 5.0;
                //实现数据的四舍五入法
                yearleave = System.NumHelp.Round(yearleave, 1);
                //不足1整天的不享受年休假
                if (yearleave < 1)
                {
                    yearleaveDay = 0;
                }
                else
                {
                    yearleaveDay = yearleave;
                }
            }
            else
            {
                //满1-3年
                if (zrzNum < yearNum * 4 && zrzNum >= yearNum)
                {
                    yearleaveDay = 5;
                }
                //满4-6年
                else if (zrzNum < yearNum * 7 && zrzNum >= yearNum * 3)
                {
                    yearleaveDay = 6;
                }
                //满7-9年
                else if (zrzNum < yearNum * 10 && zrzNum >= yearNum * 7)
                {
                    yearleaveDay = 8;
                }
                //满10年不满20年
                else if (zrzNum < yearNum * 20 && zrzNum >= yearNum * 10)
                {
                    yearleaveDay = 10;
                }
                //满20年
                else if (zrzNum >= yearNum * 20)
                {
                    yearleaveDay = 15;
                }
            }
            return yearleaveDay;
        }

        /// <summary>
        /// 学习假计算
        /// </summary>
        /// <param name="datesEmployed">入职时间</param>
        /// <returns></returns>
        public static double GetStudentLeave(DateTime datesEmployed)
        {
            //学习假天数
            var studentleaveDay = 0.0;
            if (datesEmployed == null)
            {
                return studentleaveDay;
            }

            //当前时间
            var nowtime = DateTime.Now;

            //总工作时间
            TimeSpan dd = new TimeSpan(nowtime.Ticks).Subtract(new TimeSpan(datesEmployed.Ticks)).Duration();

            //入职天数
            var zrzNum = dd.TotalDays;

            //未满一整年情况
            if (zrzNum < 365)
            {
                //学习假0天
                studentleaveDay = 0;
            }
            //满一年3天学习假
            else
            {
                studentleaveDay = 3;
            }
            return studentleaveDay;
        }

        /// <summary>
        /// 获取年假和学习假
        /// </summary>
        /// <param name="datesEmployed">入职时间</param>
        /// <returns></returns>
        public string GetYearAndStudentLeave(DateTime datesEmployed)
        {
            return GetYearLeave(datesEmployed) + "," + GetStudentLeave(datesEmployed);
        }
    }
}