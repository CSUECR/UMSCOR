using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using HOHO18.Common;
using MorSun.Bll;
using System.Web.Mvc;
using MorSun.Controllers.ViewModel;
using System.Xml;
using MorSun.Common.Privelege;

namespace MorSun.Controllers.SystemController
{
    [HandleError]
    [Authorize]
    public class RefGroupController : BaseController<wmfRefGroup>
    {
        protected override string ResourceId
        {
            get { return 资源.类别组; }
        }

        /// <summary>
        /// 可批量添加类别组
        /// </summary>
        /// <param name="t"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Create(wmfRefGroup t, string returnUrl)
        {
            if (ResourceId.HP(操作.添加))
            {
                var oper = new OperationResult(OperationResultType.Error, "添加失败");                
                string[] refGroupNames = ((t.RefGroupName == null) ? (t.RefGroupName = " ").Split(',') : t.RefGroupName.Split(','));
                for (int i = 0; i < refGroupNames.Length; i++)
                {
                    if (refGroupNames.Length == 1)
                    {
                        t.RefGroupName = refGroupNames[0];
                        OnAddCK(t);
                        if(ModelState.IsValid)
                        { 
                            //添加初始化字段
                            CreateInitObject(t);
                            var result = Bll.Insert(t,false);
                            if (result == null)
                            {
                                "RefGroupName".AE(refGroupNames[0] + "添加失败", ModelState);                            
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(refGroupNames[i]))
                        {                              
                            var model = new wmfRefGroup();
                            model.RefGroupName = refGroupNames[i];
                            model.ParentId = t.ParentId;
                            OnAddCK(t);
                            if(ModelState.IsValid)
                            {
                                CreateInitObject(model);
                                var result = Bll.Insert(model, false);
                                if (result == null)
                                {
                                    "RefGroupName".AE(refGroupNames[i] + "添加失败", ModelState);
                                }
                            }                                                       
                        }                        
                    }
                }
                if (ModelState.IsValid)
                {
                    fillOperationResult(returnUrl, oper, "添加成功");
                    Bll.UpdateChanges();
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oper.AppendData = ModelState.GE();
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                "RefGroupName".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TreeTableMove(string id, string pid, string returnUrl)
        {
            if (ResourceId.HP(操作.修改))
            {                
                var p1 = Guid.Parse(id);
                //父ID
                var p2 = Guid.Parse(pid);
                var errms = "";
                //不能将自己当做父节点
                if (p1 == p2)
                {
                    //移动失败，类别组A不能移动到类别组A下！
                    errms = "移动位置错误";
                    "RefGroupName".AE("移动位置错误", ModelState);
                }
                ///判断ID与父级ID相同
                if (SearchDep(p1, p2))
                {
                    //上级部门不能往自己的下级部门移动！
                    errms = "上级类别组不能移到下级类别组";
                    "RefGroupName".AE("上级类别组不能移到下级类别组", ModelState);
                }                
                
                var model = Bll.GetModel(p1);
                var pmodel = Bll.GetModel(p2);
                if(model == null || pmodel == null)
                {
                    errms = "数据提交错误";
                    "RefGroupName".AE("数据提交错误", ModelState);
                }
                var oper = new OperationResult(OperationResultType.Error, "移动失败 " + errms);
                if(ModelState.IsValid)
                {                    
                    model.ParentId = p2;
                    Bll.Update(model);      
                    fillOperationResult(returnUrl, oper, "移动成功");
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oper.AppendData = ModelState.GE();
                    return Json(oper, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                "RefGroupName".AE("无权限", ModelState);
                var oper = new OperationResult(OperationResultType.Error, "无权限");
                oper.AppendData = ModelState.GE();
                return Json(oper, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 判断上下级关系
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private bool SearchDep(Guid p1, Guid p2)
        {
            var dept = Bll.All.FirstOrDefault(r => r.ID == p2);
            if (dept != null)
            {
                Guid parentId = dept.ParentId.ToAs<Guid>();
                if (parentId == p1)
                {
                    return true;
                }
                else
                {
                    return SearchDep(p1, parentId);
                }
            }
            else
            {
                return false;
            }
        }


        protected override string OnAddCK(wmfRefGroup t)
        {            
            var ReferGrop = Bll.All.FirstOrDefault(r => r.RefGroupName == t.RefGroupName);
            if (ReferGrop != null)
            {
                //该类别组已经存在，请重新输入！
                "RefGroupName".AE("类别组已存在",ModelState);
            }
            if(t.ParentId != null)
            {
                var pReferGrop = Bll.All.FirstOrDefault(r => r.ID == t.ParentId);
                if(pReferGrop == null)
                    "ParentId".AE("请正确选择类别组", ModelState);
            }
            if(t.RefGroupName.Length > 50)
            {
                "RefGroupName".AE("类别组名长度不可超过50", ModelState);
            }
            return "";
        }

        protected override string OnEditCK(wmfRefGroup t)
        {            
            var ReferGrop = Bll.All.FirstOrDefault(r => r.RefGroupName == t.RefGroupName);
            if (ReferGrop != null && ReferGrop.ID != t.ID)
            {
                //该类别组已经存在，请重新输入！
                "RefGroupName".AE("类别组已存在",ModelState);
            }
            var p1 = t.ID;
            //父ID
            var p2 = t.ParentId.ToAs<Guid>();

            //不能将自己当做父节点
            if (p1 == p2)
            {
                //移动失败，类别组A不能移动到类别组A下！
                "RefGroupName".AE("移动位置错误",ModelState);
            }

            ///判断ID与父级ID相同
            if (SearchDep(p1, p2))
            {
                //上级类别组不能往自己的下级类别组移动！
                "RefGroupName".AE("上级类别组不能移到下级类别组",ModelState);
            }
            if (t.RefGroupName.Length > 50)
            {
                "RefGroupName".AE("类别组名长度不可超过50", ModelState);
            }
            return "";
        }

        protected override string OnDelCk(wmfRefGroup t)
        {
            var s = "";
            var rg = Bll.GetModel(t);
            var refBll = new BaseBll<wmfReference>();
            var model = refBll.All.FirstOrDefault(r => r.RefGroupId == t.ID);
            var childList = Bll.All.Where(r => r.ParentId == t.ID);
            if (model != null)
            {
                //该类别组类别存在，请重新输入！
                "RefGroupName".AE("该类别组还存在类别", ModelState);
                s += rg.RefGroupName + "还存在类别";
            }
            if (childList.Count() > 0)
            {
                "RefGroupName".AE("该类别组存在子类别组", ModelState);
                s += rg.RefGroupName + "还存在子类别组";
            }
            
            return s;
        }

        public ActionResult GetP()
        {
            return View();
        }


    }
}
