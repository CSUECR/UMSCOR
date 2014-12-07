using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    public class aspnet_UsersInRolesJson
    {
        public Guid UserId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid RoleId
        { get; set; }
    }

}