using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace HOHO18.Common.Helper
{
    public static class TreeViewHtmlHelper
    {
        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        public static string TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            return html.TreeView(treeId, rootItems, childrenProperty, itemContent, true, null);
        } 

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>    
        public static string TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript)
        {
            return html.TreeView(treeId, rootItems, childrenProperty, itemContent, includeJavaScript, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>
        /// <param name="emptyContent">Content to be rendered when the tree is empty</param>
        /// <param name="includeJavaScript">If true, output will automatically into the JavaScript to turn the ul into the treeview</param>    
        public static string TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent, bool includeJavaScript, string emptyContent)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<ul id='{0}'>\r\n", treeId);

            if (rootItems.Count() == 0)
            {
                sb.AppendFormat("<li>{0}</li>", emptyContent);
            }

            foreach (T item in rootItems)
            {
                RenderLi(sb, item, itemContent);
                AppendChildren(sb, item, childrenProperty, itemContent);
            }

            sb.AppendLine("</ul>");

            if (includeJavaScript)
            {
                sb.AppendFormat(
                    @"<script type='text/javascript'>
                    $(document).ready(function() {{
                        $('#{0}').treeview({{ animated: 'fast' }});
                    }});
                </script>", treeId);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 递归调用生成树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="root"></param>
        /// <param name="childrenProperty"></param>
        /// <param name="itemContent"></param>
        private static void AppendChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemContent)
        {
            var children = childrenProperty(root);
            if (children.Count() == 0)
            {
                sb.AppendLine("</li>");
                return;
            }

            sb.AppendLine("\r\n<ul>");
            foreach (T item in children)
            {
                RenderLi(sb, item, itemContent);
                AppendChildren(sb, item, childrenProperty, itemContent);
            }

            sb.AppendLine("</ul></li>");
        }

        private static void RenderLi<T>(StringBuilder sb, T item, Func<T, string> itemContent)
        {
            sb.AppendFormat("<li>{0}", itemContent(item));
        }
    }

}
