using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Bll;
using System.Collections;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using System.Xml;
using HOHO18.Common;

namespace MorSun.Controllers.SystemController
{
    public class NavigationLinkController : BaseController<wmfNavigationLink>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.导航菜单; }
        }
        
        protected override string OnAddCK(wmfNavigationLink t)
        {
            var Refer = Bll.All.FirstOrDefault(r => r.MenuName == t.MenuName && r.RefId == t.RefId);
            if (Refer != null)
            {
                //该类别已经存在，请重新输入！
                "MenuName".AE("同一类别菜单名称已经存在", ModelState); 
            }
            if (t.RIds != null && t.RIds.Count() > 0)
            {//之前少了个判断，会出错。
                Guid[] Ids = t.RIds;
                var idsCount = Ids.Count();
                if (Ids != null && idsCount > 0)
                {
                    t.ResourcesIds = "";
                    for (int i = 0; i < idsCount; i++)
                    {
                        t.ResourcesIds += Ids[i].ToString();
                        if (i != (idsCount - 1))
                        {
                            t.ResourcesIds += ",";
                        }
                    }
                }
            }

            t.RegTime = DateTime.Now;
            return "";
        }

        protected override string OnEditCK(wmfNavigationLink t)
        {            
            var Refer = Bll.All.FirstOrDefault(r => r.MenuName == t.MenuName && r.RefId == t.RefId);

            if (Refer != null && t.ID != Refer.ID)
            {
                //该类别已经存在，请重新输入！                
                "MenuName".AE("同一类别菜单名称已经存在", ModelState); 
            }

            if (t.RIds != null && t.RIds.Count() > 0)
            {//之前少了个判断，会出错。
                Guid[] Ids = t.RIds;
                var idsCount = Ids.Count();
                if (Ids != null && idsCount > 0)
                {
                    t.ResourcesIds = "";
                    for (int i = 0; i < idsCount; i++)
                    {
                        t.ResourcesIds += Ids[i].ToString();
                        if (i != (idsCount - 1))
                        {
                            t.ResourcesIds += ",";
                        }
                    }
                }
            }
            t.ModTime = DateTime.Now;

            return "";
        }



        /// <summary>
        /// 生成导航菜单
        /// </summary>
        /// <returns></returns>
        //public string GenerationMenu()
        //{
        //    if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.生成))
        //    {
        //        //获取所有未被删除的菜单
        //        var navigationList = new NavigationLinkVModel().All.Where(t => t.FlagTrashed == false && !String.IsNullOrEmpty(t.ResourcesIds)).OrderBy(t => t.Sort);

        //        if (navigationList == null)
        //        {
        //            //是否有导航菜单!
        //            return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("不存在导航菜单"), "") });
        //        }

        //        //读取XML文件中配置的MenuGroupID
        //        Guid MenuGroupID = Guid.Empty;
        //        try
        //        {
        //            MenuGroupID = Guid.Parse(HOHO18.Common.ConfigHelper.GetConfigString("MenuGroupID"));
        //        }
        //        catch
        //        {
        //            return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("配置文件中不存在MenuGroupID定义"), "") });
        //        }

        //        //获取头部导航类型
        //        var basicStr = string.Empty;
        //        var basicName = string.Empty;
        //        var basicIcon = string.Empty;
        //        var MenuReferenceList = new ReferenceVModel().All.Where(t => t.RefGroupId == MenuGroupID && t.FlagTrashed == false).OrderBy(t => t.Sort);
        //        if (MenuReferenceList != null)
        //        {
        //            foreach (var menu in MenuReferenceList)
        //            {
        //                if (menu.wmfNavigationLinks.Count() > 0)
        //                {
        //                    basicStr += menu.ItemInfo + ",";
        //                    basicName += menu.ItemValue + ",";
        //                    basicIcon += menu.Icon + ",";
        //                }
        //            }
        //            //切掉最后一个逗号
        //            if (!string.IsNullOrEmpty(basicStr))
        //            {
        //                basicStr = basicStr.Substring(0, basicStr.Length - 1);
        //                basicName = basicName.Substring(0, basicName.Length - 1);
        //                basicIcon = basicIcon.Substring(0, basicIcon.Length - 1);
        //            }
        //            else
        //            {
        //                basicStr = "basic";
        //                basicName = "系统管理";
        //                basicStr = "sys_nav";
        //            }
        //        }

        //        var ReList = new ResourceVModel().All.OrderBy(t => t.Sort);
        //        string configFilePath = Server.MapPath(HOHO18.Common.ConfigHelper.GetConfigString("XmlMenuPath").Replace("[==Language==]", SessionHelper.GetSessionLanguages()));

        //        XmlDocument xmldoc = new XmlDocument();
        //        XmlDeclaration xmldecl;
        //        XmlElement xmlelem;
        //        xmldecl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
        //        xmldoc.AppendChild(xmldecl);

        //        //加入一个根元素
        //        xmlelem = xmldoc.CreateElement("", "Menu", "");
        //        xmlelem.SetAttribute("Basic", basicStr);
        //        xmlelem.SetAttribute("BasicName", basicName);
        //        xmlelem.SetAttribute("BasicIcon", basicIcon);
        //        xmldoc.AppendChild(xmlelem);

        //        ////将头部导航写入导航XML
        //        //XmlNode root0 = xmldoc.SelectSingleNode("Menu");//查找<Menu>
        //        //XmlElement xe = xmldoc.CreateElement("Basic");//创建一个<Node>节点
        //        //xe.InnerText = basicStr;
        //        //root0.AppendChild(xe);//添加到<Menu>节点中


        //        //菜单
        //        foreach (var link in navigationList)
        //        {
        //            XmlNode root = xmldoc.SelectSingleNode("Menu");//查找<Menu>
        //            XmlElement xe1 = xmldoc.CreateElement("Node");//创建一个<Node>节点
        //            xe1.SetAttribute("ID", link.ID.ToString());//设置该节点ID属性
        //            xe1.SetAttribute("Basic", link.wmfReference.ItemInfo);//设置该节点ID属性


        //            XmlElement xesub2 = xmldoc.CreateElement("MenuID");
        //            xesub2.InnerText = link.ID.ToString();//设置文本节点
        //            xe1.AppendChild(xesub2);//添加到<Node>节点中

        //            XmlElement xesub3 = xmldoc.CreateElement("Icon");
        //            xesub3.InnerText = link.Icon;
        //            xe1.AppendChild(xesub3);

        //            XmlElement xesub4 = xmldoc.CreateElement("MenuName");
        //            xesub4.InnerText = link.MenuName;
        //            xe1.AppendChild(xesub4);

        //            XmlElement xesub5 = xmldoc.CreateElement("Url");
        //            xesub5.InnerText = link.URL;
        //            xe1.AppendChild(xesub5);

        //            XmlElement xesub6 = xmldoc.CreateElement("Menus");



        //            //资源
        //            var resourcesList = ReList.ToList().Where(t => link.ResourcesIds.Contains(t.ID.ToString()));

        //            if (resourcesList != null)
        //            {
        //                foreach (var res in resourcesList)
        //                {
        //                    XmlElement c1 = xmldoc.CreateElement("ChildNode");
        //                    c1.SetAttribute("ID", res.ID.ToString());//设置该节点ID属性

        //                    XmlElement csub1 = xmldoc.CreateElement("MenuID");
        //                    csub1.InnerText = res.ID.ToString();//设置文本节点
        //                    c1.AppendChild(csub1);//添加到<Node>节点中

        //                    XmlElement csub2 = xmldoc.CreateElement("Icon");
        //                    csub2.InnerText = res.Icon;
        //                    c1.AppendChild(csub2);

        //                    XmlElement csub3 = xmldoc.CreateElement("MenuName");
        //                    csub3.InnerText = res.ResourcesCNName;
        //                    c1.AppendChild(csub3);

        //                    XmlElement csub4 = xmldoc.CreateElement("Url");
        //                    csub4.InnerText = res.URL;
        //                    c1.AppendChild(csub4);

        //                    xesub6.AppendChild(c1);
        //                }
        //            }

        //            xe1.AppendChild(xesub6);
        //            root.AppendChild(xe1);//添加到<Menu>节点中
        //        }
        //        try
        //        {
        //            //保存创建好的XML文档
        //            xmldoc.Save(configFilePath);
        //        }
        //        catch
        //        {
        //            //是否有导航菜单!
        //            //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfResource>("文件没有修改的权限"), "") });
        //        }
        //        return "true";
        //    }
        //    else
        //    {
        //        //return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
        //    }
        //}
    }
}
