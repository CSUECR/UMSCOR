using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;


    public static class HtmlExHelper
    {

        /// <summary>
        /// 工程关联的上传功能
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="projectId">工程ID,必要属性</param>
        /// <param name="dataRef">附件的REF，必要属性</param>
        /// <param name="linkId">附件的对象ID值，可选。NULL时不做判定</param>
        /// <param name="canAdd">可选，是否显示上传按钮</param>
        /// <param name="width">可选，默认为800</param>
        /// <param name="height">可选：默认为600</param>
        /// <returns></returns>
        public static MvcHtmlString UploadProjectData(this HtmlHelper helper, Guid? projectId,Guid? dataRef, Guid? linkId, bool canAdd=true ,int width=800,int height=600)
        {
             var v = new ViewDataDictionary();
             v["ProjectId"] = projectId;
             v["DataRef"] = dataRef;
             v["LinkId"] = linkId;
             v["CanAdd"] = canAdd;
             v["Width"] = width;
             v["Height"] = height;
            helper.RenderPartial("_UploadProjectData", v);
            return MvcHtmlString.Empty;
        }
    }