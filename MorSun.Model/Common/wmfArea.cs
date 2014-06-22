using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;
namespace MorSun.Model
{
    //[Bind(Include = "AreaId,formTownId,Sort,AreaName,WYDTCode,formWYDTLon,formWYDTLat,formWYDTZoom,WYDTTitle,WYDTContent,WYDTImage,formWYDTImgWide,formWYDTImgHigh,formWYDTImgTopLeftHorizontal,formWYDTImgTopLeftVertical,ImgIds")]
    public partial class wmfArea : IModel
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

        public string formTownId//县ID
        {
            get;
            set;
        }
        public string formWYDTLon//我要地图经度
        {
            get;
            set;
        }
        public string formWYDTLat//我要地图纬度
        {
            get;
            set;
        }
        public string formWYDTZoom//我要地图比例
        {
            get;
            set;
        }
        public string formWYDTImgWide//我要地图自定义图标宽
        {
            get;
            set;
        }
        public string formWYDTImgHigh//我要地图自定义图标高
        {
            get;
            set;
        }
        public string formWYDTImgTopLeftHorizontal//我要地图自定义图标左上角定位横向距离
        {
            get;
            set;
        }
        public string formWYDTImgTopLeftVertical//我要地图自定义图标左上角定位纵向距离
        {
            get;
            set;
        } 

        /// <summary>
        /// 提交上来的连接字符串
        /// </summary>
        public string formDataBaseCon { get; set; }

        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            ParameterProcess.TrimParameter<wmfArea>(this);
            if (String.IsNullOrEmpty(formTownId) || !ModelStateValidate.IsGuid(formTownId))
                yield return new RuleViolation("请选择县", "formTownId");
            if (String.IsNullOrEmpty(AreaName))
                yield return new RuleViolation("区域名不能为空", "AreaName");            
            if (AreaName != "undefined" && !String.IsNullOrEmpty(AreaName) && AreaName.Length > 20)
                yield return new RuleViolation("区域名长度不能超过20个字符", "AreaName");
            if (Sort <= 0)
                yield return new RuleViolation("排序错误,必须是整数且最小为1", "Sort");
            if ((formWYDTLon != "undefined" && !String.IsNullOrEmpty(formWYDTLon)) && (!(ModelStateValidate.IsIntege(formWYDTLon))))
                yield return new RuleViolation("地图经度输入格式有误", "formWYDTLon");
            if ((formWYDTLat != "undefined" && !String.IsNullOrEmpty(formWYDTLat)) && (!(ModelStateValidate.IsIntege(formWYDTLat))))
                yield return new RuleViolation("地图纬度输入格式有误", "formWYDTLat");
            if ((formWYDTZoom != "undefined" && !String.IsNullOrEmpty(formWYDTZoom)) && (!(ModelStateValidate.IsIntege(formWYDTZoom))))
                yield return new RuleViolation("地图比例输入格式有误", "formWYDTZoom");
            if ((formWYDTImgWide != "undefined" && !String.IsNullOrEmpty(formWYDTImgWide)) && (!(ModelStateValidate.IsIntege(formWYDTImgWide))))
                yield return new RuleViolation("自定义图标宽输入格式有误", "formWYDTImgWide");
            if ((formWYDTImgHigh != "undefined" && !String.IsNullOrEmpty(formWYDTImgHigh)) && (!(ModelStateValidate.IsIntege(formWYDTImgHigh))))
                yield return new RuleViolation("自定义图标高输入格式有误", "formWYDTImgHigh");
            if ((formWYDTImgTopLeftHorizontal != "undefined" && !String.IsNullOrEmpty(formWYDTImgTopLeftHorizontal)) && (!(ModelStateValidate.IsIntege(formWYDTImgTopLeftHorizontal))))
                yield return new RuleViolation("自定义图标左上角定位横向距离输入格式有误", "formWYDTImgTopLeftHorizontal");
            if ((formWYDTImgTopLeftVertical != "undefined" && !String.IsNullOrEmpty(formWYDTImgTopLeftVertical)) && (!(ModelStateValidate.IsIntege(formWYDTImgTopLeftVertical))))
                yield return new RuleViolation("自定义图标左上角定位纵向距离输入格式有误", "formWYDTImgTopLeftVertical");
            yield break;
        }                        

         
        partial void OnValidate(ChangeAction action)
        {
            if (!IsValid)
                throw new ApplicationException("Rule violations prevent saving");
        }
    }

}