using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace HOHO18.Common.Web.UBB
{

    /// <summary>
    /// UBB 的摘要说明。
    /// </summary>
    public class UBBTool
    {
        private static string dvHTMLEncode(string fString)
        {
            if (fString != string.Empty)
            {
                fString.Replace("<", "&lt;");
                fString.Replace(">", "&rt;");
                fString.Replace(((char)34).ToString(), "&quot;");
                fString.Replace(((char)39).ToString(), "'");
                fString.Replace(((char)13).ToString(), "");
                fString.Replace(((char)10).ToString(), "<BR> ");
            }
            return (fString);
        }
        private static readonly RegexOptions options = RegexOptions.Compiled | RegexOptions.Singleline; 

        public static string txtMessage(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            string result = str;
            result = HttpUtility.HtmlEncode(result);
            result = DecodeStyle(result);
            result = DecodeFont(result);
            result = DecodeColor(result);
            result = DecodeImage(result);
            result = DecodeLinksNoFollow(result);
            result = DecodeQuote(result);
            result = DecodeAlign(result);
            result = DecodeList(result);
            result = DecodeHeading(result);
            result = DecodeBlank(result);
            return result; 
        }
        private static string DecodeHeading(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[h(\d)\]", "<h$1>", options);
            result = Regex.Replace(result, @"\[/h(\d)\]", "</h$1>", options);
            return result;
        }
        private static string DecodeList(string ubb)
        {
            string sListFormat = "<ol style=\"list-style:{0};\">$1</ol>";
            string result = ubb;
            // Lists
            result = Regex.Replace(result, @"\[ul\]\s*", "<ul>", options);
            result = Regex.Replace(result, @"\[/ul\]", "</ul>", options);
            result = Regex.Replace(result, @"\[ol\]\s*", "<ol>", options);
            result = Regex.Replace(result, @"\[/ol\]", "</ol>", options);            
            result = Regex.Replace(result, @"\[li\]\s*", "<li>", options);
            result = Regex.Replace(result, @"\[/li\]", "</li>", options);
            result = Regex.Replace(result, @"\[\*\]([^\[]*)", "<li>$1</li>", options);
            result = Regex.Replace(result, @"\[list\]\s*(.*?)\[/list\]", "<ul>$1</ul>", options);
            result = Regex.Replace(result, @"\[list=1\]\s*(.*?)\[/list\]", string.Format(sListFormat, "decimal"), options);
            result = Regex.Replace(result, @"\[list=i\]\s*(.*?)\[/list\]", string.Format(sListFormat, "lower-roman"), options);
            result = Regex.Replace(result, @"\[list=I\]\s*(.*?)\[/list\]", string.Format(sListFormat, "upper-roman"), options);
            result = Regex.Replace(result, @"\[list=a\]\s*(.*?)\[/list\]", string.Format(sListFormat, "lower-alpha"), options);
            result = Regex.Replace(result, @"\[list=A\]\s*(.*?)\[/list\]", string.Format(sListFormat, "upper-alpha"), options);
            return result;
        }
        private static string DecodeBlank(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"(?<= ) | (?= )", " ", options);
            result = Regex.Replace(result, @"\r\n", "<br />");
            string[] blockTags = { "h[1-6]", "li", "list", "div", "p", "ul" };
            //clear br before block tags(start or end)
            foreach (string tag in blockTags)
            {
                Regex r = new Regex("<br />(<" + tag + ")", options);
                result = r.Replace(result, "$1");
                r = new Regex("<br />(</" + tag + ")", options);
                result = r.Replace(result, "$1");
            }
            return result;
        }
        private static string DecodeAlign(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[align=left\]", "<div style=\"text-align:left\">",options);            
            result = Regex.Replace(result, @"\[align=right\]", "<div style=\"text-align:right\">", options);           
            result = Regex.Replace(result, @"\[align=center\]", "<div style=\"text-align:center\">", options);
            result = Regex.Replace(result, @"\[/align\]", "</div>", options);            
            return result;
        }
        private static string DecodeQuote(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[quote\]", "<blockquote><div>", options);
            result = Regex.Replace(result, @"\[/quote\]", "</div></blockquote>", options);
            return result;
        }
        private static string DecodeFont(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[size=(\d+)\]", "<font size=\"$1\">", options);
            result = Regex.Replace(result, @"\[/size\]", "</font>", options);
            result = Regex.Replace(result, @"\[font=(.*?)\]", "<font size=\"$1\">", options);
            result = Regex.Replace(result, @"\[/font\]", "</font>", options);
            return result;
        }
        private static string DecodeLinks(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[url\]www\.(.*?)", "<a href=\"http://www.$1\">", options);
            result = Regex.Replace(result, @"\[url\](.*?)", "<a href=\"$1\">", options);            
            //result = Regex.Replace(result, @"\[url=(.*?)\](.*?)\[/url\]", "<a href=\"$1\" title=\"$2\">$2</a>", options);
            result = Regex.Replace(result, @"\[/url\]", "</a>", options);
            result = Regex.Replace(result, @"\[email\](.*?)", "<a href=\"mailto:$1\">", options);
            result = Regex.Replace(result, @"\[/email\]", "</a>", options);
            return result;
        }
        private static string DecodeLinksNoFollow(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[url\]www\.(.*?)", "<a rel=\"nofollow\" href=\"http://www.$1\">", options);            
            result = Regex.Replace(result, @"\[url\](.*?)", "<a rel=\"nofollow\" href=\"$1\">", options);
            result = Regex.Replace(result, @"\[url=(.*?)\](.*?)", "<a rel=\"nofollow\" href=\"$1\" title=\"$2\">", options);
            result = Regex.Replace(result, @"\[/url\]", "</a>", options);
            result = Regex.Replace(result, @"\[email\](.*?)", "<a href=\"mailto:$1\">", options);
            result = Regex.Replace(result, @"\[/email\]", "</a>", options);
            return result;
        }
        private static string DecodeImage(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[hr\]", "<hr />", options);
            result = Regex.Replace(result, @"\n", "<br />", options);
            result = Regex.Replace(result, @"\[br\]", "<br />", options);
            result = Regex.Replace(result, @"\[img\](.+?)\[/img\]", "<img src=\"$1\" alt=\"\" />", options);
            result = Regex.Replace(result, @"\[img=(\d+)x(\d+)\](.+?)\[/img\]", "<img src=\"$3\" style=\"width:$1px;height:$2px\" alt=\"\" />", options);
            return result;
        }
        private static string DecodeColor(string ubb)
        {
            string result = ubb;
            result = Regex.Replace(result, @"\[color=(.*?)\]", "<span style=\"color:$1\">", options);
            result = Regex.Replace(result, @"\[/color\]", "</span>", options);
            result = Regex.Replace(result, @"\[hilitecolor=(.*?)\]", "<span style=\"background-color:$1\">", options);
            result = Regex.Replace(result, @"\[/hilitecolor\]", "</span>", options);
            return result;
        }
        //private static string DMatchEvaluator(Match m)
        //{
        //    return "<span style=\"color:" + m.Captures[0].Value + "\">" + m.Captures[1].Value + "</span>";
        //}
        private static string DecodeStyle(string ubb)
        {
            string result = ubb;
            //we don't need this for perfomance and other consideration:
            //(<table[^>]*>(?><table[^>]*>(?<Depth>)|</table>(?<-Depth>)|.)+(?(Depth)(?!))</table>)
            result = Regex.Replace(result, @"\[[b]\]", "<strong>", options);
            result = Regex.Replace(result, @"\[/[b]\]", "</strong>", options);
            result = Regex.Replace(result, @"\[[u]\]", "<span style=\"text-decoration:underline\">", options);
            result = Regex.Replace(result, @"\[/[u]\]", "</span>", options);
            result = Regex.Replace(result, @"\[[i]\]", "<i>", options);
            result = Regex.Replace(result, @"\[/[i]\]", "</i>", options);
            result = Regex.Replace(result, @"\[[p]\]", "<p>", options);
            result = Regex.Replace(result, @"\[/[p]\]", "</p>", options);
            return result;
        } 
    }
}
