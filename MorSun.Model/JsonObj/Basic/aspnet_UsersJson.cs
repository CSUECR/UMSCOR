using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{    
    public class aspnet_UsersJson
    {
        public Guid ApplicationId { get; set; }

        public Guid UserId { get; set; }

        public String UserName { get; set; }

        public String LoweredUserName { get; set; }

        public String MobileAlias { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime LastActivityDate { get; set; }
    }

}