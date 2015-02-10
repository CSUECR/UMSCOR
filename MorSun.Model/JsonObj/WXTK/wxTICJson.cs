using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;

namespace MorSun.Model
{
    public class wxTICJson
    {
        /// <summary>
        /// 
        /// </summary>
        public string ticket
        { get; set; }  
        /// <summary>
        /// 
        /// </summary>
        public string expires_in
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errcode
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errmsg
        { get; set; }        
    }
}