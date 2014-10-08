using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace System.Web.Mvc
{
    public static class HtmlHelperExpand
    {

        #region 下拉菜单

        /// <summary>
        /// 生成下拉列表
        /// </summary>
        /// <param name="SelectListName">下拉列表的Name</param>
        /// <param name="SelectListId">下拉列表的Id</param>
        /// <param name="SelectItems">下拉列表的所有数据项</param>
        /// <param name="SelectedValue">默认选中的下拉列表的值</param>
        /// <param name="Attributes">属性设置,可设置多个,如:onchange="void(0)"</param>
        /// <returns></returns>
        public static String DropDownList(this HtmlHelper helper, string SelectListName, string SelectListId, System.Collections.IEnumerable SelectItems, string SelectedValue, string Attributes)
        {
            return HtmlHelperExpand.DropDownList(null, SelectListName, SelectListId, SelectItems, SelectedValue, Attributes, "Value", "Text", "", "");
        }
        /// <summary>
        /// 生成下拉列表
        /// </summary>
        /// <param name="helper">可为空</param>
        /// <param name="SelectListName">下拉列表的Name</param>
        /// <param name="SelectListId">下拉列表的Id</param>
        /// <param name="SelectItems">下拉列表的所有数据项</param>
        /// <param name="SelectedValue">默认选中的下拉列表的值</param>
        /// <param name="Attributes">属性设置,可设置多个,如:onchange="void(0)"</param>
        /// <param name="textFieldName">Text字段名</param>
        /// <param name="valueFieldName">Value字段名</param>
        /// <returns></returns>
        public static String DropDownList(this HtmlHelper helper, string SelectListName, string SelectListId, System.Collections.IEnumerable SelectItems, string SelectedValue, string Attributes,
            string valueFieldName, string textFieldName, string addFirstValue, string addFirstName)
        {
            return DropDownList(helper, SelectListName, SelectListId, SelectItems, SelectedValue, Attributes, valueFieldName, textFieldName, new Hashtable(), addFirstValue, addFirstName);
        }
        /// <summary>
        /// 生成下拉列表
        /// </summary>
        /// <param name="helper">可为空</param>
        /// <param name="SelectListName">下拉列表的Name</param>
        /// <param name="SelectListId">下拉列表的Id</param>
        /// <param name="SelectItems">下拉列表的所有数据项</param>
        /// <param name="SelectedValue">默认选中的下拉列表的值</param>
        /// <param name="Attributes">属性设置,可设置多个,如:onchange="void(0)"</param>
        /// <param name="textFieldName">Text字段名</param>
        /// <param name="valueFieldName">Value字段名</param>
        /// <param name="parentValueFieldName">如果佣有父节点,则需要写存放父节点的Value的字段名</param>
        /// <returns></returns>
        public static String DropDownList(this HtmlHelper helper, string SelectListName, string SelectListId, System.Collections.IEnumerable SelectItems, string SelectedValue, string Attributes,
            string valueFieldName, string textFieldName, string parentValueFieldName, string addFirstValue, string addFirstName)
        {
            Hashtable p = new Hashtable();
            p[parentValueFieldName] = "parentValue";
            return DropDownList(helper, SelectListName, SelectListId, SelectItems, SelectedValue, Attributes, valueFieldName, textFieldName, p, addFirstValue, addFirstName);
        }


        /// <summary>
        /// 生成下拉列表
        /// </summary>
        /// <param name="helper">可为空</param>
        /// <param name="SelectListName">下拉列表的Name</param>
        /// <param name="SelectListId">下拉列表的Id</param>
        /// <param name="SelectItems">下拉列表的所有数据项</param>
        /// <param name="SelectedValue">默认选中的下拉列表的值</param>
        /// <param name="Attributes">属性设置,可设置多个,如:onchange="void(0)"</param>
        /// <param name="textFieldName">Text字段名</param>
        /// <param name="valueFieldName">Value字段名</param>
        /// <param name="attrs">自定义属性,的名(对象字段名)值(生成option的属性名)对应表</param>
        /// <returns></returns>
        public static String DropDownList(this HtmlHelper helper, string SelectListName, string SelectListId, System.Collections.IEnumerable SelectItems, string SelectedValue, string Attributes,
            string valueFieldName, string textFieldName, Hashtable attrs, string addFirstValue, string addFirstName)
        {
            HttpServerUtility Server = HttpContext.Current.Server;
            StringBuilder sb = new StringBuilder();
            sb.Append("<select");
            if (SelectListName.Trim() != "")
            {
                sb.Append(" name=\"" + Server.HtmlEncode(SelectListName) + "\"");
            }
            if (SelectListId.Trim() != "")
            {
                sb.Append(" id=\"" + Server.HtmlEncode(SelectListId) + "\"");
            }
            //sb.Append(" class=\"easyui-combobox\"");
            //sb.Append(" style=\"width:160px;\"");
            if (Attributes.Trim() != "")
            {
                sb.Append(" " + Attributes.Trim());
            }
            sb.Append(">");

            if (!String.IsNullOrEmpty(addFirstValue) || !String.IsNullOrEmpty(addFirstName))
                sb.Append("<option" + " value=\"" + Server.HtmlEncode(addFirstValue) + "\">" + Server.HtmlEncode(addFirstName) + "</option>");

            System.Reflection.PropertyInfo valueP;
            System.Reflection.PropertyInfo textP;
            System.Reflection.PropertyInfo selectedP;
            System.Reflection.PropertyInfo tempP;
            string value, text;
            StringBuilder tempStr = new StringBuilder();
            if (SelectItems != null)
            {
                foreach (object item in SelectItems)
                {
                    valueP = item.GetType().GetProperty(valueFieldName);
                    textP = item.GetType().GetProperty(textFieldName);
                    selectedP = item.GetType().GetProperty("Selected");
                    tempStr = new StringBuilder();
                    foreach (var attr in attrs.Keys)
                    {
                        tempP = item.GetType().GetProperty(attr.ToString());

                        tempStr.Append(" " + attrs[attr] + "=\"" + tempP.GetValue(item, null) + "\"");
                    }

                    value = valueP.GetValue(item, null).ToString();
                    text = textP.GetValue(item, null).ToString();
                    if (value == SelectedValue || (selectedP != null && Convert.ToBoolean(selectedP.GetValue(item, null))))
                    {
                        sb.Append("<option" + tempStr + " value=\"" + Server.HtmlEncode(value) + "\" selected=\"selected\">" + Server.HtmlEncode(text) + "</option>");
                    }
                    else
                    {
                        sb.Append("<option" + tempStr + " value=\"" + Server.HtmlEncode(value) + "\">" + Server.HtmlEncode(text) + "</option>");
                    }
                }
            }
            sb.Append("</select>");

            return sb.ToString();
        }

        /// <summary>
        /// 把一个有层级关系的下拉菜单,按照层级关系缩进,生成新的下拉菜单
        /// </summary>
        /// <param name="helper">本方法会生成树形,外部方法,如果用本类调用,则为null</param>
        /// <param name="items">需要转换成树形菜单的项列表</param>
        /// <param name="valueFieldName">用作值的属性名</param>
        /// <param name="textFieldName">用作显示文本的属性名</param>
        /// <param name="parentIdFieldName">父节点的属性名</param>
        /// <returns></returns>
        public static String DropDownList(this HtmlHelper helper, System.Collections.IEnumerable items, string valueFieldName, string textFieldName, string parentIdFieldName,
            string SelectListName, string SelectListId, string SelectedValue, string Attributes, string addFirstValue, string addFirstName)
        {
            return HtmlHelperExpand.DropDownList(null, SelectListName, SelectListId, HtmlHelperExpand.GetParentSelectList(items, valueFieldName, textFieldName, parentIdFieldName), SelectedValue, Attributes, "Value", "TreeText", "ParentValue", addFirstValue, addFirstName);
        }

        /// <summary>
        /// 把一个有层级关系的下拉菜单,按照层级关系缩进,生成新的下拉菜单
        /// </summary>
        /// <param name="helper">本方法会生成树形,外部方法,如果用本类调用,则为null</param>
        /// <param name="items">需要转换成树形菜单的项列表</param>
        /// <param name="valueFieldName">用作值的属性名</param>
        /// <param name="textFieldName">用作显示文本的属性名</param>
        /// <param name="parentIdFieldName">父节点的属性名</param>
        /// <returns></returns>
        public static String DropDownList<T>(this HtmlHelper helper, List<T> items, string valueFieldName, string textFieldName, string treeTextFieldName, string parentIdFieldName,
            string SelectListName, string SelectListId, string SelectedValue, string Attributes, string addFirstValue, string addFirstName)
        {
            return HtmlHelperExpand.DropDownList(null, SelectListName, SelectListId, HtmlHelperExpand.GetParentSelectList<T>(valueFieldName, textFieldName, treeTextFieldName, parentIdFieldName, items), SelectedValue, Attributes, valueFieldName, treeTextFieldName, parentIdFieldName, addFirstValue, addFirstName);
        }

        /// <summary>
        /// 把一个有层级关系的下拉菜单,按照层级关系缩进,生成新的下拉菜单
        /// </summary>
        /// <param name="helper">本方法会生成树形,外部方法,如果用本类调用,则为null</param>
        /// <param name="items">需要转换成树形菜单的项列表</param>
        /// <param name="valueFieldName">用作值的属性名</param>
        /// <param name="textFieldName">用作显示文本的属性名</param>
        /// <param name="parentIdFieldName">父节点的属性名</param>
        /// <returns></returns>
        public static String DropDownList<T>(this HtmlHelper helper, List<T> items, string valueFieldName, string textFieldName, string treeTextFieldName, string parentIdFieldName, Hashtable attr,
            string SelectListName, string SelectListId, string SelectedValue, string Attributes, string addFirstValue, string addFirstName)
        {
            return HtmlHelperExpand.DropDownList(null, SelectListName, SelectListId, HtmlHelperExpand.GetParentSelectList<T>(valueFieldName, textFieldName, treeTextFieldName, parentIdFieldName, items), SelectedValue, Attributes, valueFieldName, treeTextFieldName, attr, addFirstValue, addFirstName);
        }


        /// <summary>
        /// 把一个有层级关系的下拉菜单,按照层级关系缩进,生成新的下拉菜单
        /// </summary>
        /// <typeparam name="T">项的类型</typeparam>
        /// <param name="items">项列表</param>
        /// <param name="valueFieldName">用作值的属性名</param>
        /// <param name="textFieldName">用作显示文本的属性名</param>
        /// <param name="parentIdFieldName">父节点的属性名</param>
        /// <returns></returns>
        public static List<T> GetParentSelectList<T>(string valueFieldName, string textFieldName, string treeTextFieldName, string parentIdFieldName, List<T> items)
        {
            int i = 0, j = 0, p;
            if (items.Count > 0)
            {
                System.Reflection.PropertyInfo valueP = items[0].GetType().GetProperty(valueFieldName);
                System.Reflection.PropertyInfo textP = items[0].GetType().GetProperty(textFieldName);
                System.Reflection.PropertyInfo treeTextP = items[0].GetType().GetProperty(treeTextFieldName);
                System.Reflection.PropertyInfo parentP = items[0].GetType().GetProperty(parentIdFieldName);
                var tempItems = items.AsQueryable();
                var param = Expression.Parameter(typeof(T), "Sort");
                var pi = typeof(T).GetProperty("Sort");
                if (pi != null)
                {
                    var types = new Type[2];
                    types[0] = typeof(T);
                    types[1] = pi.PropertyType;
                    Expression expr = Expression.Call(typeof(Queryable), "OrderByDescending", types, tempItems.Expression, Expression.Lambda(Expression.Property(param, "Sort"), param));
                    tempItems = tempItems.AsQueryable().Provider.CreateQuery<T>(expr);
                    items = tempItems.ToList();
                }
                string temp, temp1 = Guid.Empty.ToString(), temp2, temp3;
                T sItem, sItem1;
                bool isTempBool;//是否存在父节点
                //先把所有顶层节点拉到最上方
                for (i = 0; i < items.Count; i++)
                {
                    sItem = items[i];
                    var parentPValue = parentP.GetValue(sItem, null);
                    temp = (Convert.ToString(parentPValue) ?? "").ToLower();
                    isTempBool = false;
                    //把父节点不存在的节点做为根节点
                    foreach (var s in items)
                    {
                        if (String.Compare(valueP.GetValue(s, null).ToString().ToLower(), temp) == 0)
                        {
                            isTempBool = true;
                            break;
                        }
                    }
                    if (!isTempBool)
                    {
                        treeTextP.SetValue(sItem, textP.GetValue(sItem, null).ToString(), null);
                        items.RemoveAt(i);
                        items.Insert(0, sItem);
                    }
                    else
                    {
                        treeTextP.SetValue(sItem, "|--" + Convert.ToString(textP.GetValue(sItem, null)), null);
                    }
                }

                //然后开始循环找每个项的子层,
                //规律:当一开始,顶层都在最上方时,循环取子层,子层的位置不会在父层上

                for (i = 0; i < items.Count; i++)
                {
                    sItem = items[i];
                    temp = Convert.ToString((valueP.GetValue(sItem, null)));
                    temp3 = treeTextP.GetValue(sItem, null).ToString();
                    p = temp3.IndexOf("|--");
                    if (p == -1)
                    {
                        temp3 = "　";
                    }
                    else
                    {
                        temp3 = temp3.Substring(0, p) + "　";
                    }
                    for (j = i; j < items.Count; j++)
                    {
                        sItem1 = items[j];

                        temp2 = Convert.ToString(parentP.GetValue(sItem1, null));
                        if (temp == temp2)
                        {
                            treeTextP.SetValue(sItem1, temp3 + treeTextP.GetValue(sItem1, null).ToString(), null);
                            items.RemoveAt(j);
                            items.Insert(i + 1, sItem1);
                        }
                    }
                }

            }
            return items;
        }


        /// <summary>
        /// 把一个有层级关系的下拉菜单,按照层级关系缩进,生成新的下拉菜单
        /// </summary>
        /// <param name="items">项列表</param>
        /// <param name="valueFieldName">用作值的属性名</param>
        /// <param name="textFieldName">用作显示文本的属性名</param>
        /// <param name="parentIdFieldName">父节点的属性名</param>
        /// <returns></returns>
        public static List<SelectListItemParent> GetParentSelectList(System.Collections.IEnumerable items, string valueFieldName, string textFieldName, string parentIdFieldName)
        {
            System.Reflection.PropertyInfo valueP;
            System.Reflection.PropertyInfo textP;
            System.Reflection.PropertyInfo parentP;
            object po, to, vo;

            List<SelectListItemParent> list = new List<SelectListItemParent>();

            //把所有项整理成SelectListItemParent
            foreach (var item in items)
            {
                valueP = item.GetType().GetProperty(valueFieldName);
                textP = item.GetType().GetProperty(textFieldName);
                parentP = item.GetType().GetProperty(parentIdFieldName);
                po = parentP.GetValue(item, null);
                to = textP.GetValue(item, null);
                vo = valueP.GetValue(item, null);
                list.Add(new SelectListItemParent
                {
                    ParentValue = (po != null ? po.ToString().ToLower() : ""),
                    Text = (to != null ? to.ToString().ToLower() : ""),
                    TreeText = (to != null ? to.ToString().ToLower() : ""),
                    Value = (vo != null ? vo.ToString().ToLower() : ""),
                    Depth = 1
                });
            }
            //先把所有顶层节点拉到最上方
            SelectListItemParent sItem;
            string temp;
            for (int i = 0; i < list.Count; i++)
            {
                sItem = list[i];
                temp = sItem.ParentValue;
                //把父节点不存在的节点做为根节点
                if (list.Count(p => p.Value == temp) <= 0)
                {
                    list.RemoveAt(i);
                    list.Insert(0, sItem);
                }
            }



            //开始根据层级关系缩进
            SelectListItemParent jItem;
            string tempStr;
            StringBuilder t = new StringBuilder();
            for (int i = 0; i < list.Count - 1; i++)
            {
                sItem = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    jItem = list[j];
                    if (sItem.Value == jItem.ParentValue && sItem.Value != jItem.Value)
                    {
                        jItem.Depth = sItem.Depth + 1;
                        tempStr = "";
                        for (int d = 1; d < jItem.Depth; d++)
                        {
                            tempStr += "　";
                        }
                        jItem.TreeText = tempStr + "|--" + jItem.TreeText;
                        list.RemoveAt(j);
                        list.Insert(i + 1, jItem);
                    }
                }
            }
            return list;
        }

        //将所有父节点为parentValue的节点缩进,递归调用
        private static void SelectIndentParent(List<SelectListItemParent> list, SelectListItemParent parent)
        {
            SelectListItemParent item;
            int parentIndex;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                if (item.ParentValue == parent.Value && item.Value != parent.Value)
                {
                    if (item.TreeText == item.Text)
                    {
                        item.TreeText = "　|--" + item.TreeText;
                    }
                    else
                    {
                        item.TreeText = "　" + item.TreeText;
                    }
                    //移动到父节点下
                    list.Remove(item);
                    parentIndex = list.IndexOf(parent);
                    list.Insert(parentIndex + 1, item);
                    SelectIndentParent(list, item);
                }
            }
        }
        #endregion

        #region HTML标签操作
        /*
         <%
    string t2 = @"<DIV class=""dd"">12
    3</div>
    <span>1
    23</span>";
         %>
    <%=Html.BackTagHtml(Html.Encode(t2),"br","div") %>

         */
        /// <summary>
        /// 还原被格式化的标签
        /// </summary>
        /// <param name="htmlStr">HTML字符串</param>
        /// <param name="backTagsName">要还原的标签列表 如:"br","p","div"</param>
        /// <returns></returns>
        public static string BackTagHtml(this HtmlHelper helper, string htmlStr, params string[] backTagsName)
        {
            return Pri_BackTagHtml(htmlStr, backTagsName);
        }
        /// <summary>
        /// 还原被格式化的标签
        /// </summary>
        /// <param name="htmlStr">HTML字符串</param>
        /// <param name="backTagsName">要还原的标签列表 如:"br","p","div"</param>
        /// <returns></returns>
        public static string BackTagHtml(string htmlStr, params string[] backTagsName)
        {
            return Pri_BackTagHtml(htmlStr, backTagsName);
        }
        private static string Pri_BackTagHtml(string htmlStr, params string[] backTagsName)
        {
            MatchEvaluator mRe = new MatchEvaluator(MatchReplaceHtmlTag);
            foreach (var tag in backTagsName)
            {
                htmlStr = Regex.Replace(htmlStr, @"(?s)\&lt\;" + tag + @"(.*?)\&gt\;(.*?)\&lt\;\/" + tag + @"\&gt\;", mRe, RegexOptions.IgnoreCase);
                htmlStr = Regex.Replace(htmlStr, @"(?s)\&lt\;" + tag + @"(.*?)\/\&gt\;", mRe, RegexOptions.IgnoreCase);
            }
            return htmlStr;
        }
        private static string MatchReplaceHtmlTag(Match m)
        {
            return HttpUtility.HtmlDecode(m.Groups[0].Value);
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="Htmlstring">过滤内容</param>
        /// <returns></returns>
        public static string RemoveHTMLTag(string Htmlstring) //去除HTML标记
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);

            //不去除换行和连续的空格,换成下面,把连续的空格换成一个空格
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"[\s\t\r\n]{1,}", " ", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");

            return Htmlstring;
        }
        #endregion

        #region 复选框

        /// <summary>
        /// 生成复选框
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="CheckBoxName"></param>
        /// <param name="CheckBoxItems"></param>
        /// <param name="lineQuantity">每行数量</param>
        /// <param name="newline">是否换行</param>
        /// <param name="Attributes"></param>
        /// <returns></returns>
        public static String CheckBox(this HtmlHelper helper, string CheckBoxName, System.Collections.IEnumerable CheckBoxItems, int lineQuantity, bool newline, string Attributes)
        {
            return HtmlHelperExpand.CheckBox(null, CheckBoxName, CheckBoxItems, lineQuantity, newline, Attributes, "Value", "Text");
        }

        /// <summary>
        /// 复选框
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="CheckBoxName"></param>
        /// <param name="CheckBoxItems"></param>
        /// <param name="lineQuantity">每行数量</param>
        /// <param name="newline">是否换行</param>
        /// <param name="Attributes"></param>
        /// <param name="valueFieldName"></param>
        /// <param name="textFieldName"></param>
        /// <returns></returns>
        public static String CheckBox(this HtmlHelper helper, string CheckBoxName, System.Collections.IEnumerable CheckBoxItems, int lineQuantity, bool newline, string Attributes,
            string valueFieldName, string textFieldName)
        {
            return CheckBox(helper, CheckBoxName, CheckBoxItems, lineQuantity, newline, Attributes, valueFieldName, textFieldName, new Hashtable());
        }

        /// <summary>
        /// 生成复选框
        /// </summary>
        /// <param name="helper">可为空</param>
        /// <param name="SelectListName">下拉列表的Name</param>
        /// <param name="SelectListId">下拉列表的Id</param>
        /// <param name="SelectItems">下拉列表的所有数据项</param>
        /// <param name="SelectedValue">默认选中的下拉列表的值</param>
        /// <param name="Attributes">属性设置,可设置多个,如:onchange="void(0)"</param>
        /// <param name="textFieldName">Text字段名</param>
        /// <param name="valueFieldName">Value字段名</param>
        /// <param name="attrs">自定义属性,的名(对象字段名)值(生成option的属性名)对应表</param>
        /// <returns></returns>
        public static String CheckBox(this HtmlHelper helper, string CheckBoxName, System.Collections.IEnumerable CheckBoxItems, int lineQuantity, bool newline, string Attributes,
            string valueFieldName, string textFieldName, Hashtable attrs)
        {
            HttpServerUtility Server = HttpContext.Current.Server;
            StringBuilder sb = new StringBuilder();
            if (newline)
                sb.Append("<p>");
            System.Reflection.PropertyInfo valueP;
            System.Reflection.PropertyInfo textP;
            System.Reflection.PropertyInfo tempP;
            string value, text;
            StringBuilder tempStr = new StringBuilder();

            //添加属性
            tempStr.Append(Attributes);

            int i = 0;
            if (CheckBoxItems != null)
            {
                foreach (object item in CheckBoxItems)
                {
                    i++;
                    valueP = item.GetType().GetProperty(valueFieldName);
                    textP = item.GetType().GetProperty(textFieldName);
                    tempStr = new StringBuilder();
                    foreach (var attr in attrs.Keys)
                    {
                        tempP = item.GetType().GetProperty(attr.ToString());
                        tempStr.Append(" " + attrs[attr].ToString() + "=\"" + tempP.GetValue(item, null).ToString() + "\"");
                    }

                    value = valueP.GetValue(item, null).ToString();
                    text = textP.GetValue(item, null).ToString();
                    if (newline && i % (lineQuantity + 1) == 0 && i > 0)
                        sb.Append("</p><p>");
                    sb.Append("<input type=\"checkbox\" name=\"" + CheckBoxName + "\"" + tempStr.ToString() + " value=\"" + Server.HtmlEncode(value) + "\"/>" + Server.HtmlEncode(text) + "&nbsp;");
                }
            }
            if (newline)
                sb.Append("</p>");
            return sb.ToString();
        }



        /// <summary>
        /// 生成复选框
        /// </summary>
        /// <param name="helper">可为空</param>
        /// <param name="CheckBoxName">复选框的名称Name</param>
        /// <param name="CheckBoxItems">数据项</param>
        /// <param name="lineQuantity">列数</param>
        /// <param name="newline">是否允许新行</param>
        /// <param name="Attributes">属性</param>
        /// <param name="valueFieldName">值字段Name</param>
        /// <param name="textFieldName">显示字段Name</param>
        /// <param name="SelectedValue">选中值</param>
        /// <param name="LableAttributes">lable属性</param>
        /// <param name="ID_Str">ID前缀</param>
        /// <returns></returns>
        public static String CheckBox(this HtmlHelper helper, string CheckBoxName, System.Collections.IEnumerable CheckBoxItems, int lineQuantity, bool newline, string Attributes,
            string valueFieldName, string textFieldName, string SelectedValue, string LableAttributes, string ID_Str)
        {
            HttpServerUtility Server = HttpContext.Current.Server;
            StringBuilder sb = new StringBuilder();
            if (newline)
                sb.Append("<p>");
            System.Reflection.PropertyInfo valueP;
            System.Reflection.PropertyInfo textP;
            string value, text, _checked;

            if (string.IsNullOrEmpty(SelectedValue))
            {
                SelectedValue = "";
            }

            var SelectedValuesArrayList = SelectedValue.Split(',');
            var SelectedCount = SelectedValuesArrayList.Count();

            int i = 0;
            if (CheckBoxItems != null)
            {
                foreach (object item in CheckBoxItems)
                {
                    i++;
                    valueP = item.GetType().GetProperty(valueFieldName);
                    textP = item.GetType().GetProperty(textFieldName);

                    value = valueP.GetValue(item, null).ToString();
                    text = textP.GetValue(item, null).ToString();
                    _checked = "";

                    //选中
                    if (!string.IsNullOrEmpty(SelectedValue))
                    {
                        for (int j = 0; j < SelectedCount; j++)
                        {
                            if (value == SelectedValuesArrayList[j])
                            {
                                _checked = " checked=\"checked\" ";
                            }
                        }
                    }
                    sb.Append("<span class='spanLabelWidth'><input type=\"checkbox\" name=\"" + CheckBoxName + "\"" + Attributes + _checked + " value=\"" + Server.HtmlEncode(value) + "\" id=\"" + ID_Str + value + "\"/><label for=\"" + ID_Str + value + "\"  " + LableAttributes + ">" + Server.HtmlEncode(text) + "</label></span>");
                    if (newline && i % lineQuantity == 0 && i > 0)
                        sb.Append("</p><p>");
                }
            }
            if (newline)
                sb.Append("</p>");
            return sb.ToString();
        }
        #endregion

        #region 分页
        public static MvcHtmlString AjaxPages(this HtmlHelper htmlHelper, int dataCount, string pageIndex, int pageSize, string pagesMod)
        {

            ViewDataDictionary v = new ViewDataDictionary();
            v["DataCount"] = dataCount;
            v["PageIndex"] = pageIndex;
            v["PageSize"] = pageSize;
            if (!String.IsNullOrEmpty(pagesMod))
                v["PagesMod"] = pagesMod;//这边可以改模式
            htmlHelper.RenderPartial("_AjaxPages", v);
            return MvcHtmlString.Empty;
        }
        #endregion

        #region 上传控件
        /// <summary>
        /// 上传文件不需要保存到数据库中
        /// </summary>
        /// <param name="viewPage"></param>
        /// <param name="saveFileName">保存文件名,不包括扩展名</param>
        /// <param name="folder">保存目录</param>
        /// <param name="fileExt">保存扩展名</param>
        /// <param name="fileDesc">出现在上传对话框中的文件类型描述</param>
        /// <param name="queueSizeLimit">当允许多文件生成时，设置选择文件的个数</param>
        /// <param name="multi">是否允许同时上传多文件，true允许多文件上传</param>
        /// <param name="auto">选定文件后是否自动上传，true自动上传</param>
        /// <param name="simUploadLimit">多文件上传时，同时上传文件数目限制</param>
        /// <param name="buttonImg">浏览按钮的文本</param>
        /// <param name="sizeLimit">控制上传文件的大小，单位字节</param>
        /// <param name="operationID">需要显示的操作ID</param>
        /// <param name="operationType">操作ID对应控件的类型：如：img,text</param>
        /// <param name="moreUploadFile">本页面是否是第二次应用此上传控件，是=true,否=false</param>
        public static MvcHtmlString Uploading(this HtmlHelper helper, string saveFileName, string folder, string fileExt, string fileDesc, int queueSizeLimit, bool multi, bool auto, int simUploadLimit, string buttonImg, int sizeLimit, string operationID, string operationType, bool moreUploadFile)
        {
            var v = new ViewDataDictionary();
            v["saveFileName"] = saveFileName;
            v["folder"] = folder;
            v["fileExt"] = fileExt;
            v["fileDesc"] = fileDesc;
            v["queueSizeLimit"] = queueSizeLimit;
            v["multi"] = multi;
            v["auto"] = auto;
            v["simUploadLimit"] = simUploadLimit;
            if (!string.IsNullOrEmpty(buttonImg))
            {
                v["buttonImg"] = buttonImg;
            }
            else
            {
                v["buttonImg"] = "buttonImg".GX();
            }

            v["sizeLimit"] = sizeLimit;
            v["operationID"] = operationID;
            v["moreUploadFile"] = moreUploadFile;
            v["operationType"] = operationType;
            helper.RenderPartial("_AjaxUploadFile", v);
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="viewPage"></param>
        /// <param name="operation">操作类型，create or update</param>
        /// <param name="categoryId">类别ID</param>
        /// <param name="adscriptionId">归属ID（相当于类别）</param>
        /// <param name="linkId1">链接ID(和对象绑定)</param>
        /// <param name="description">描述</param>
        /// <param name="folder">保存目录</param>
        /// <param name="fileExt">保存扩展名</param>
        /// <param name="fileDesc">出现在上传对话框中的文件类型描述</param>
        /// <param name="queueSizeLimit">当允许多文件生成时，设置选择文件的个数</param>
        /// <param name="multi">是否允许同时上传多文件，true允许多文件上传</param>
        /// <param name="auto">选定文件后是否自动上传，true自动上传</param>
        /// <param name="simUploadLimit">多文件上传时，同时上传文件数目限制</param>
        /// <param name="buttonImg">浏览按钮的图片</param>
        /// <param name="sizeLimit">控制上传文件的大小，单位字节</param>
        /// <param name="operationID">需要显示的操作ID</param>
        /// <param name="operationType">操作ID对应控件的类型：如：img,text</param>
        /// <param name="moreUploadFile">本页面是否是第二次应用此上传控件，是=true,否=false</param>
        public static MvcHtmlString Uploading(this HtmlHelper helper, string operation, string categoryId, Guid adscriptionId, Guid linkId1, string description,
            string folder, string fileExt, string fileDesc, int queueSizeLimit, bool multi, bool auto, int simUploadLimit, string buttonImg, int sizeLimit, string operationID, string operationType, bool moreUploadFile)
        {
            var v = new ViewDataDictionary();
            v["operation"] = operation;
            v["categoryId"] = categoryId;
            v["adscriptionId"] = adscriptionId;
            v["linkId1"] = linkId1;
            v["description"] = description;
            v["folder"] = folder;
            v["fileExt"] = fileExt;
            v["fileDesc"] = fileDesc;
            v["queueSizeLimit"] = queueSizeLimit;
            v["multi"] = multi;
            v["auto"] = auto;
            v["simUploadLimit"] = simUploadLimit;
            if (!string.IsNullOrEmpty(buttonImg))
            {
                v["buttonImg"] = buttonImg;
            }
            else
            {
                v["buttonImg"] = "buttonImg".GX();
            }
            v["sizeLimit"] = sizeLimit;
            v["operationID"] = operationID;
            v["moreUploadFile"] = moreUploadFile;
            v["operationType"] = operationType;
            helper.RenderPartial("_AjaxUploadFile", v);
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// 上传控件的拓展，上传材料计划，材料追加等。add by timfeng 2014-6-3
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="TableId">保存材料数据的表格ID值</param>
        /// <param name="isFirst">是否是第二次引用（上传控件）</param>
        /// <returns></returns>
        public static MvcHtmlString LeadingInStoreDetail(this HtmlHelper helper, string tableId, bool isFirst = false)
        {
            var v = new ViewDataDictionary();
            v["TableId"] = tableId;
            v["IsFirst"] = isFirst;
            helper.RenderPartial("_LeadingInStoreDetail", v);
            return MvcHtmlString.Empty;
        }

        #endregion

        #region 多输入，自动补全
        /// <summary>
        /// 多选，拼音输入可自动补全信息（当groupList为null的时候，ToPinYinJsonString的categoryPropetyName无需指定）增加部门树型菜单选项
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="multiSelectDivID">控件id</param>
        /// <param name="outputInputID">隐藏输出的id，也就是所选择的id值</param>
        /// <param name="groupID">类别id，类别下拉框选择的id</param>
        /// <param name="confirmButtonID">类别下拉框选择的确认按钮的id</param>
        /// <param name="allJsonData">生成拼音等所有的json数据</param>
        /// <param name="groupList">可以为null，类别下拉框的数据</param>
        /// <param name="limitSelectNum">限制输入个数</param>
        /// <param name="outInputIDValue">隐藏输出id的内部值</param>
        /// <param name="isSetWidth">是否开启对应限制个数的自适应控件宽度</param>
        /// <param name="isFirst">是否是初次加载</param>
        /// <returns></returns>
        public static MvcHtmlString MultiSelectTextboxWithDept(this HtmlHelper helper,
             string multiSelectDivID, string outputInputID, string groupID, string confirmButtonID, string allJsonData, System.Collections.IEnumerable groupList
            , string outInputIDValue = "", int limitSelectNum = 20, bool isSetWidth = false,bool isFirst=true)
        {
            if (string.IsNullOrEmpty(multiSelectDivID) || string.IsNullOrEmpty(outputInputID)
                || string.IsNullOrEmpty(groupID) || string.IsNullOrEmpty(confirmButtonID))
                throw new ArgumentNullException("未给多选控件的参数为空");
            var dic = new ViewDataDictionary();
            dic.Add("DivID", multiSelectDivID);
            dic.Add("OutputInputID", outputInputID);
            dic.Add("GroupID", groupID);
            dic.Add("ConfirmButtonID", confirmButtonID);
            dic.Add("DeptList", groupList);
            dic.Add("AllData", allJsonData);
            dic.Add("OutInputIDValue", outInputIDValue);
            dic.Add("LimitSelectNum", limitSelectNum);
            dic.Add("IsSetWidth", isSetWidth);
            dic.Add("IsFirst", isFirst);
            helper.RenderPartial("_MultiSelectControl", dic);
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// 多选，拼音输入可自动补全信息（当groupList为null的时候，ToPinYinJsonString的categoryPropetyName无需指定）增加部门树型菜单选项和部门默认值
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="multiSelectDivID">控件id</param>
        /// <param name="outputInputID">隐藏输出的id，也就是所选择的id值</param>
        /// <param name="groupID">类别id，类别下拉框选择的id</param>
        /// <param name="confirmButtonID">类别下拉框选择的确认按钮的id</param>
        /// <param name="allJsonData">生成拼音等所有的json数据</param>
        /// <param name="groupList">可以为null，类别下拉框的数据</param>
        /// <param name="limitSelectNum">限制输入个数</param>
        /// <param name="outInputIDValue">隐藏输出id的内部值</param>
        /// <param name="isSetWidth">是否开启对应限制个数的自适应控件宽度</param>
        /// <param name="isFirst">是否是初次加载</param>
        /// <returns></returns>
        public static MvcHtmlString MultiSelectTextboxWithDropDwon(this HtmlHelper helper,
             string multiSelectDivID, string outputInputID, string groupID, string confirmButtonID, string allJsonData, string dropDownListString
            , string outInputIDValue = "", int limitSelectNum = 20, bool isSetWidth = false, bool isFirst = true)
        {
            if (string.IsNullOrEmpty(multiSelectDivID) || string.IsNullOrEmpty(outputInputID)
                || string.IsNullOrEmpty(groupID) || string.IsNullOrEmpty(confirmButtonID))
                throw new ArgumentNullException("未给多选控件的参数为空");
            var dic = new ViewDataDictionary();
            dic.Add("DivID", multiSelectDivID);
            dic.Add("OutputInputID", outputInputID);
            dic.Add("GroupID", groupID);
            dic.Add("ConfirmButtonID", confirmButtonID);
            dic.Add("DropDownListString", dropDownListString);
            dic.Add("AllData", allJsonData);
            dic.Add("OutInputIDValue", outInputIDValue);
            dic.Add("LimitSelectNum", limitSelectNum);
            dic.Add("IsSetWidth", isSetWidth);
            dic.Add("IsFirst", isFirst);
            helper.RenderPartial("_MultiSelectControl", dic);
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// 多选，拼音输入可自动补全信息（当groupList为null的时候，ToPinYinJsonString的categoryPropetyName无需指定） 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="multiSelectDivID">控件id</param>
        /// <param name="outputInputID">隐藏输出的id，也就是所选择的id值</param>
        /// <param name="groupID">类别id，类别下拉框选择的id</param>
        /// <param name="confirmButtonID">类别下拉框选择的确认按钮的id</param>
        /// <param name="allJsonData">生成拼音等所有的json数据</param>
        /// <param name="groupList">可以为null，类别下拉框的数据</param>
        /// <param name="limitSelectNum">限制输入个数</param>
        /// <param name="outInputIDValue">隐藏输出id的内部值</param>
        /// <param name="isSetWidth">是否开启对应限制个数的自适应控件宽度</param>
        /// <param name="isFirst">是否是初次加载</param>
        /// <returns></returns>
        public static MvcHtmlString MultiSelectTextbox(this HtmlHelper helper,
             string multiSelectDivID, string outputInputID, string groupID, string confirmButtonID, string allJsonData, IEnumerable<SelectListItem> groupList
            , string outInputIDValue = "", int limitSelectNum = 20, bool isSetWidth = false, bool isFirst = true)
        {
            if (string.IsNullOrEmpty(multiSelectDivID) || string.IsNullOrEmpty(outputInputID)
                || string.IsNullOrEmpty(groupID) || string.IsNullOrEmpty(confirmButtonID))
                throw new ArgumentNullException("未给多选控件的参数为空");
            var dic = new ViewDataDictionary();
            dic.Add("DivID", multiSelectDivID);
            dic.Add("OutputInputID", outputInputID);
            dic.Add("GroupID", groupID);
            dic.Add("ConfirmButtonID", confirmButtonID);
            dic.Add("GroupList", groupList);
            dic.Add("AllData", allJsonData);
            dic.Add("OutInputIDValue", outInputIDValue);
            dic.Add("LimitSelectNum", limitSelectNum);
            dic.Add("IsSetWidth", isSetWidth);
            dic.Add("IsFirst", isFirst);
            helper.RenderPartial("_MultiSelectControl", dic);
            return MvcHtmlString.Empty;
        }
        #endregion

        #region 材料表自动补全表格
        public static MvcHtmlString TableMaterial(this HtmlHelper helper, string tableId, string groupId, object detailList = null, object papertypeId = null, bool canAdd = true)
        {
            var data = new ViewDataDictionary();
            data.Add("TableId", tableId);
            data.Add("GroupId", groupId);
            data.Add("DetailList", detailList);
            data.Add("PaperTypeId", papertypeId);
            data.Add("CanAdd", canAdd);
            helper.RenderPartial("_MaterialPlanTable", data);
            return MvcHtmlString.Empty;
        }
        #endregion

        #region 标签labelFor
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
        #endregion 

        #region UEditor
        public static MvcHtmlString HtmlEditor(this HtmlHelper helper, string name, object value, string width = "100%", string height = "200px", bool isFirst = true)
        {
            var v = new ViewDataDictionary();
            v["Name"] = name;
            v["Value"] = value;
            v["Width"] = width;
            v["Height"] = height;
            v["isFirst"] = isFirst;
            helper.RenderPartial("_HtmlEditor", v);
            return MvcHtmlString.Empty;
        }
        #endregion
    }
}
