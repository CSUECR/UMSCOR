using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOHO18.Common;
using System.Data.Linq;

namespace MorSun.Model
{
    public partial class wmfMailRecord:IModel
    {
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public string CheckedId { get; set; }

        public wmfMailRecord wmfMailRecord2(string mailTo, string body, string title, string mailFromName, string mailToName,Guid mailRef)
        {
            var model = new wmfMailRecord();
            model.ID = Guid.NewGuid();
            model.MailTo = mailTo;
            model.Body = body;
            model.Title = title;
            model.MailFromName = mailFromName;
            model.MailToName = mailToName;
            model.MailRef = MailRef;
            model.RegTime = DateTime.Now;
            model.FlagTrashed = false;
            model.FlagDeleted = false;
            return model;
        }

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfMailRecord>(this);

            //if (string.IsNullOrEmpty(ReceiveUser) && MailCategory != Guid.Parse("ca615b28-3219-4483-a5e1-4e191c4d8a32"))
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfMail>("收件人不能为空"), "ReceiveUser");

            yield break;
        }

        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }
}
