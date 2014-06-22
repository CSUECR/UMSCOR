using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;

namespace MorSun.Model
{
    public partial class wmfMail:IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string CheckedId { get; set; }

        //选择收件人的ID
        public string checkId { get; set; }

        //抄送选择
        public string copyStr { get; set; }
        //密送选择
        public string secretStr { get; set; }

        public string sentOrDraftFlag { get; set; }
        //回复Reply或转发Forward
        public string operateType { get; set; }

        public int? PIndex { get; set; }

        public string PreUrl { get; set; }
        //下一封
        public Guid nextLetter{get;set;}
        //上一封
        public Guid preLetter { get; set; }

        /// <summary>
        /// 邮件通知时的调用内容，格式如下：
        /// window.parent.send('MorSun.Common.类别.Reference.上传文件_邮件',
        /// 'FMailID','标题','内容','要返回的页面','要发生的用户，放Guid用逗号隔开');
        /// </summary>
        public string sendContent { get; set; }

        public string recevierStr { get; set; }
        //提醒标识
        public string remindFlag { get; set; }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfMail>(this);

            if (string.IsNullOrEmpty(ReceiveUser) && MailCategory != Guid.Parse("ca615b28-3219-4483-a5e1-4e191c4d8a32"))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfMail>("收件人不能为空"), "ReceiveUser");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
