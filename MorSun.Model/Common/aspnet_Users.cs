using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    //[Bind(Include = "UserName,OldPassword,Password,Password2,Email,PwdQuestion,PwdAnswer,IsApproved,IsLockedOut,formCreateUser_DeptId,formCreateUser_PositionId")]
    public partial class aspnet_Users : IModel
    {

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string Email { get; set; }
        public string PwdQuestion { get; set; }
        public string PwdAnswer { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsLockedOut { get; set; }
        public wmfUserInfo userinfoModel { get; set; }
        //public wmfUserDeptPosition userDeptPosition { get; set; }
        public string CheckedId { get; set; }
        //员工管理编辑，state=manage
        public string State { get; set; }
        //返回员工管理的页码
        public int? PIndex { get; set; }


        /// <summary>
        /// 临时存放用户真实姓名
        /// </summary>
        public string UserTrueName { set; get; }   

        //传过来的部门ID
        public string Dep { get; set; }

        public string Depts { get; set; }

        public string scrollTo { get; set; }


        //角色
        public string RoleId { get; set; }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }


        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<aspnet_Users>(this);

            if (String.IsNullOrEmpty(UserName) || UserName.Trim() == "")
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("用户名不能为空"), "UserName");
            else
            {
                if (!String.IsNullOrEmpty(UserName) && !ModelStateValidate.IsUsername(UserName.Trim()))
                    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Users>("用户名长度"), "UserName");
            }
            if (String.IsNullOrEmpty(Password) || Password.Trim() == "")
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码不能为空"), "Password");
            else
            {
                if (Password.Length < 6)
                    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码长度"), "Password");
            }
            if (String.IsNullOrEmpty(Password2) || Password2.Trim() == "")
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Membership>("确认密码不能为空"), "Password2");
            else
            {
                if (!String.IsNullOrEmpty(Password) && !String.IsNullOrEmpty(Password2) && Password != Password2)
                    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Membership>("密码不一致"), "Password2");
            }
            if (String.IsNullOrEmpty(UserTrueName) || UserTrueName.Trim() == "")
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfUserInfo>("真实姓名不能为空"), "UserTrueName");
            if (String.IsNullOrEmpty(Email) || Email.Trim() == "")
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Membership>("电子邮箱不能为空"), "UserName");
            if (!String.IsNullOrEmpty(Email) && !ModelStateValidate.IsEmail(Email.Trim()))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<aspnet_Membership>("电子邮箱格式不正确"), "Email");
            //if (String.IsNullOrEmpty(PwdQuestion) || PwdQuestion.Trim() == "")
            //    yield return new RuleViolation("密码提示问题不能为空!", "PwdQuestion");
            //if (String.IsNullOrEmpty(PwdAnswer) || PwdAnswer.Trim() == "")
            //    yield return new RuleViolation("密码提示问题答案不能为空!", "PwdAnswer");


            //if (String.IsNullOrEmpty(formCreateUser_DeptId) || !ModelStateValidate.IsGuid(formCreateUser_DeptId))
            //    yield return new RuleViolation("请选择网点", "formCreateUser_DeptId");
            //if (String.IsNullOrEmpty(formCreateUser_PositionId) || !ModelStateValidate.IsGuid(formCreateUser_PositionId))
            //    yield return new RuleViolation("请选择职位", "formCreateUser_PositionId");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

}