using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;
using System.Reflection;
using HOHO18.Common.Helper;
using HOHO18.Common;
using System.IO;

namespace MorSun.Model
{
    //[Bind(Include = "ParentId,Category,Virtual,DeptName,Domain,Description,Sort,Province,City,Town,Area,Address,Tel,WYDTLon,WYDTLat,WYDTZoom,WYDTTitle,WYDTContent,WYDTImage,WYDTImgWide,WYDTImgHigh,WYDTImgTopLeftHorizontal,WYDTImgTopLeftVertical")]
    //[Bind]
    public partial class wmfDept : IPPCModel
    {

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnParentIdChanging(Guid value);
        #endregion

        public bool IsValid
        {
            get { return (GetRuleViolations().Count() == 0); }
        }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domain
        {
            get;
            set;
        }

        //public string Category
        //{
        //    get;
        //    set;
        //}

        public string Virtual
        {
            get;
            set;
        }

        public string formProvince
        {
            get;
            set;
        }

        public string formCity
        {
            get;
            set;
        }

        public string formTown
        {
            get;
            set;
        }

        public string formArea
        {
            get;
            set;
        }

        //public wmfDept parentDept
        //{
        //    get;
        //    set;
        //}


        public string DeptNameTree
        {
            get;
            set;
        }

        public Guid? OldParentID
        {
            get;
            set;
        }

        //public IEnumerable<wmfDept> Childrens
        //{
        //    get;
        //    set;
        //}



        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfDept>(this);
            if (String.IsNullOrEmpty(DeptName))
                yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("部门名为空"), "DeptName");


            //if (String.IsNullOrEmpty(Tel))
            //    yield return new RuleViolation(XmlHelper.GetKeyNameValidation<wmfDept>("电话为空"), "Tel");

            yield break;
        }


        //partial void OnValidate(ChangeAction action)
        //{
        //    if (!IsValid)
        //        throw new ApplicationException("Rule violations prevent saving");
        //}

        public string CheckedId { get; set; }

        public string isTree { get; set; }
    }

}