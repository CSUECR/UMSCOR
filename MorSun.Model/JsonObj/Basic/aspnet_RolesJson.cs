using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{    
    public class aspnet_RolesJson
    {
        public Guid ApplicationId 
        { get; set; }

        public Guid RoleId
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RoleName
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LoweredRoleName
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Sort
        { get; set; }
    }

}