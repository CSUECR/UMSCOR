using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOHO18.Common
{
    public interface IModel
    {
        IEnumerable<RuleViolation> GetRuleViolations();
        //bool IsValid;
        //public string Verifycode;
        //public string VerifycodeRandom;
    }
}
