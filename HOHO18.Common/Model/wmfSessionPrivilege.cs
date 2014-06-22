using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HOHO18.Common
{
    public class wmfSessionPrivilege
    {
        public string resourceId { get; set; }
        public string operationId { get; set; }
        public ArrayList privilegeValues { get; set; }
        public String[] privilegeValuesArray { get; set; }
    }
}
