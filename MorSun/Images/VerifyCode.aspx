<%@ Page Language="C#" Inherits="System.Web.UI.Page" %>

<%
    
    HOHO18.Common.Web.VerifyCodeType type = HOHO18.Common.Web.VerifyCodeType.Login;
    try
    {
        string typeStr = Request["type"];
        if (String.IsNullOrEmpty(typeStr))
            type = HOHO18.Common.Web.VerifyCodeType.Login;
        else
            type = (HOHO18.Common.Web.VerifyCodeType)Enum.Parse(typeof(HOHO18.Common.Web.VerifyCodeType), typeStr, true);
    }
    catch { }
    HOHO18.Common.Web.VerifyCode.ShowImg(type);
%>