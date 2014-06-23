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

namespace MorSun.Controllers.CommonController
{
    public class ReferenceController : BaseController<wmfReference>
    {
        protected override string ResourceId
        {
            get { return MorSun.Common.Privelege.资源.类别; }
        }

        //public override ActionResult Index()
        //{
        //    if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.查看))
        //    {
        //        var vModel = new ReferenceVModel();
        //        return View(vModel);
        //    }
        //    else
        //    {
        //        return Content(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"));
        //    }
        //}
        /// <summary>
        ///  批量添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override string Create(wmfReference t)
        {
            if (MorSun.Controllers.BasisController.havePrivilege(ResourceId, MorSun.Common.Privelege.操作.添加))
            {
                string msg = "";
                string[] itemInfos = ((t.ItemInfo == null) ? (t.ItemInfo = " ").Split(',') : t.ItemInfo.Replace("\r\n", ",").Split(','));
                for (int i = 0; i < itemInfos.Length; i++)
                {
                    if (itemInfos.Length == 1)
                    {
                        if (string.IsNullOrEmpty(t.ItemInfo))
                            t.ItemInfo = itemInfos[0].Trim();
                        else
                            t.ItemInfo = t.ItemInfo.Trim();
                        if (string.IsNullOrEmpty(t.ItemValue))
                            t.ItemValue = itemInfos[0].Trim();
                        else
                            t.ItemValue = t.ItemValue.Trim();
                        msg = OnPreCreateCK(t);
                        if (msg == "true")
                        {
                            return base.Create(t);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemInfos[i]))
                        {
                            t.ItemInfo = itemInfos[i];
                            msg = OnPreCreateCK(t);
                            if (msg == "true")
                            {
                                if (string.IsNullOrEmpty(t.ItemInfo) || Guid.Equals(t.RefGroupId, Guid.Empty))
                                {
                                    return base.Create(t);
                                }
                                var newRefer = new wmfReference();
                                newRefer.ItemValue = itemInfos[i];
                                newRefer.ItemInfo = itemInfos[i];
                                newRefer.RefGroupId = t.RefGroupId;
                                t.Sort = i + 1;
                                newRefer.Sort = t.Sort;
                                Create(newRefer);
                            }
                        }
                        msg = "true";
                    }
                }

                return msg;
            }
            else
            {
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation("项目提示", "无权限操作"), "") });
            }
        }        

        protected override string OnPreCreateCK(wmfReference t)
        {
            string ret = "true";
            var Refer = Bll.All.FirstOrDefault(r => (r.ItemValue == t.ItemInfo || r.ItemValue == t.ItemValue) && r.RefGroupId == t.RefGroupId);
            if (Refer != null)
            {
                //该类别已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("类别已存在"), "") });
            }

            return ret;
        }

        protected override string OnEditCK(wmfReference t)
        {
            string ret = "true";
            var Refer = Bll.All.FirstOrDefault(r => (r.ItemValue == t.ItemInfo || r.ItemValue == t.ItemValue) && r.RefGroupId == t.RefGroupId);
            if (Refer != null && t.ID != Refer.ID)
            {
                //该类别已经存在，请重新输入！
                return getErrListJson(new[] { new RuleViolation(XmlHelper.GetKeyNameValidation<wmfReference>("类别已存在"), "") });
            }
            if (String.IsNullOrEmpty(t.ItemInfo))
                t.ItemInfo = t.ItemValue;
            else
                t.ItemInfo = t.ItemInfo.Trim();
            return ret;
        }

        [Authorize]
        public virtual ActionResult ReferList(ReferListVModel t)
        {
            return View(t);
        }
        public static string GetRefer()
        {
            StringBuilder str = new StringBuilder();

            var deptList = new RefGroupVModel().All;
            str.Append("{id: '1', pId: 0, name: '" + XmlHelper.GetPagesString<wmfReference>("类别组") + "',open:true },");
            foreach (var item in deptList)
            {
                if (item.ParentId == null)
                {
                    str.Append("{");
                    str.AppendFormat("id:\"{0}\",pId:'1',name:\"{1}\",isDept:true", item.ID, item.RefGroupName);
                    str.Append("}");
                    str.Append(",");
                }
                else
                {
                    str.Append("{");
                    str.AppendFormat("id:'{0}',pId:'{1}',name:'{2}',isDept:true", item.ID, item.ParentId, item.RefGroupName);
                    str.Append("}");
                    str.Append(",");
                }
            }
            return "var zNodes =[" + str.ToString().TrimEnd(',') + "]";
        }

        //public static string GetRefer(string appId)
        //{
        //    StringBuilder str = new StringBuilder();
        //    var ID = Guid.Parse(appId);
        //    var deptList = new RefGroupVModel().All.Where(p => p.ApplicationId == ID);
        //    foreach (var item in deptList)
        //    {
        //        if (item.ParentId == null)
        //        {
        //            str.Append("{");
        //            str.AppendFormat("id:\"{0}\",pId:'1',name:\"{1}\",isDept:true,open:true", item.ID, item.RefGroupName);
        //            str.Append("}");
        //            str.Append(",");
        //        }
        //        else
        //        {
        //            str.Append("{");
        //            str.AppendFormat("id:'{0}',pId:'{1}',name:'{2}',isDept:true", item.ID, item.ParentId, item.RefGroupName);
        //            str.Append("}");
        //            str.Append(",");
        //        }
        //    }
        //    return "var zNodes =[" + str.ToString().TrimEnd(',') + "]";
        //}

        public virtual String SetDefault(wmfReference t)
        {
            var ret = "true";
            var refList = new ReferenceVModel().All.Where(p => p.RefGroupId == t.RefGroupId);
            foreach (var item in refList)
            {
                item.IsDefalut = false;
            }
            var model = Bll.GetModel(t);
            UpdateModel(model);
            model.IsDefalut = true;
            Bll.UpdateChanges();
            return ret;
        }

        public virtual ActionResult Left(ReferenceVModel t)
        {
            return View(t);
        }


        public ActionResult TaskReference(Guid? RefGroup)
        {
            if (RefGroup == null || RefGroup == Guid.Empty)
                RefGroup = Guid.Parse("715bd944-2793-4555-87ec-726fb8cb26c4");
            ViewBag.RefGroupID = RefGroup;
            var list = Bll.All.Where(u => u.RefGroupId == RefGroup && u.FlagTrashed == false).OrderBy(u => u.Sort);
            return View(list);
        }

        public ActionResult EditTaskReference(Guid? id)
        {
            var model = Bll.GetModel(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult EditTaskReference(wmfReference t)
        {
            var originalModel = Bll.GetModel(t.ID);

            if (TryUpdateModel(originalModel))
            {
                Bll.UpdateChanges();
            }
            return RedirectToAction("TaskReference");
        }

        public ActionResult CreateTaskReferenceAdd(wmfReference t)
        {
            t.ID = Guid.NewGuid();
            return View(t);
        }
        [HttpPost]
        public ActionResult CreateTaskReference(wmfReference t)
        {
            var exsit = Bll.All.Any(u => string.Compare(u.ItemValue, t.ItemValue, true) == 0);
            if (exsit)
                ModelState.AddModelError("", "已经存在该ItemValue值得数据，请重新查看");
            t.RegTime = DateTime.Now;
            t.ModTime = DateTime.Now;
            t.FlagTrashed = false;
            t.FlagDeleted = false;
            Bll.Insert(t);
            return RedirectToAction("TaskReference", new { RefGroup = t.RefGroupId });

        }
    }
}
