using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using MorSun.Common;
using MorSun.Model;
using System.Web.Mvc;
//using System.Data.Objects;
using MorSun.Bll;
using FastReflectionLib;
using MorSun.Common.Privelege;
using System.Web.Security;

namespace MorSun.Controllers.ViewModel
{
    public class BaseVModel<T>
        where T : class
    {
        public virtual T t { get; set; }

        public virtual string PIndex { get; set; }

        public int _Psize = "PageSize".GX().ToAs<int>();

        public virtual int PSize
        {
            get
            {
                return _Psize;
            }
            set{
                _Psize = value;
            }
        }

        /// <summary>
        /// 设置初始默认值
        /// </summary>
        /// <param name="psize"></param>
        public virtual void SetPSize(int psize)
        {
            if (PSize < 1)
            {
                PSize = psize;
            }
        }

        /// <summary>
        /// 所有符合条件的页面
        /// </summary>
        public virtual int PCount
        {
            get
            {
                return List.Count();
            }
        }

        BaseBll<T> _dao;
        /// <summary>
        /// 
        /// </summary>
        public virtual BaseBll<T> Dao
        {
            get
            {
                _dao = _dao.Load();
                return _dao;
            }
            set { _dao = value; }
        }

        public virtual IQueryable<T> All
        {
            get
            {
                return Dao.All.Select(t => new { t, F = 1 }).OrderBy(t => t.F).Select(t => t.t);
            }
        }

        /// <summary>
        /// 获取列表 子类再此重写(override)查询条件
        /// </summary>
        public virtual IQueryable<T> List
        {
            get
            {
                return All;
            }
        }
        

        /// <summary>
        /// 获取分页后的集合
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> PLimit(int pSize = 999999999)
        {
            SetPSize(pSize);

            var count = PCount;
            if (String.IsNullOrEmpty(PIndex) || !HOHO18.Common.ModelStateValidate.IsIntege(PIndex.ToString()))
                PIndex = "1";
            int page = Convert.ToInt32(PIndex);
            //调整页面
            var pindex = page < 1 ? 1 : page;
            if (PSize < 1)
            {
                PSize = 999999999;
            }
            var psize = PSize;
            //如果查询范围超过了最大范围
            if ((pindex - 1) * psize >= count)
            {
                //pindex调整到最大值
                pindex = (count - 1) / psize + 1;
                //如果psize=count,pindex=1
                //if (psize == count)
                //    pindex = 1;
            }
            page = pindex;
            PIndex = pindex.ToString();
            return List.PLimit(page, PSize);
        }        

        /// <summary>
        /// 第一条
        /// </summary>
        public virtual T First
        {
            get
            {
                return PLimit().First();
            }
        }


        /// <summary>
        /// 完成动作
        /// </summary>
        public virtual void DoSth()
        {
            if (!string.IsNullOrWhiteSpace(Todo))
            {
                var method = GetType().GetMethods().FirstOrDefault(m => m.Name.Eql(Todo));
                method.FastInvoke(this, null);
            }
        }

        /// <summary>
        /// 要执行的操作
        /// </summary>
        public virtual string Todo { get; set; }


        /// <summary>
        /// 填充树形表格
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public virtual void Each<Tree>(IQueryable<Tree> items, Action<Tree> action, Func<Tree, IQueryable<Tree>> subFuc)
            where Tree : class
        {
            foreach (var item in items)
            {
                action(item);
                var subs = subFuc(item);
                // var subs = Model.All.Where(sub => sub.ParentId == item.Id);
                Each(subs, action, subFuc);
            }
        }

        /// <summary>
        /// 填充树形表格(默认为Id和ParentId)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public virtual void Each<Tree>(IQueryable<Tree> items, Action<Tree> action)
            where Tree : class
        {
            BaseBll<Tree> bll = new BaseBll<Tree>();
            var param = Expression.Parameter(typeof(Tree), "Sort");
            var pi = typeof(Tree).GetProperty("Sort");
            if (pi != null)
            {
                var types = new Type[2];
                types[0] = typeof(Tree);
                types[1] = pi.PropertyType;
                Expression expr = Expression.Call(typeof(Queryable), "OrderBy", types, items.Expression, Expression.Lambda(Expression.Property(param, "Sort"), param));
                items = items.AsQueryable().Provider.CreateQuery<Tree>(expr);
            }
            foreach (var item in items)
            {
                action(item);

                var treeExpr = Expression.Parameter(typeof(Tree));

                var parentProp=typeof(Tree).GetProperty("ParentId");

                Expression<Func<Tree, bool>> test = Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(treeExpr, parentProp),
                        Expression.Convert(Expression.Constant(item.AsDy().ID),parentProp.PropertyType)
                    ),
                    treeExpr);
                var flagTrashed = this.FlagTrashed == "1";
                ConstantExpression constFlagTrashed = Expression.Constant(flagTrashed);
                ParameterExpression paramFlagTrashed = Expression.Parameter(typeof(Tree), "p");
                MemberExpression mex = LambdaExpression.PropertyOrField(paramFlagTrashed, "FlagTrashed");
                BinaryExpression filter = Expression.Equal(mex, constFlagTrashed);
                Expression<Func<Tree, bool>> expression2 = Expression.Lambda<Func<Tree, bool>>(filter, new ParameterExpression[] { paramFlagTrashed });
                var subs = bll.All.Where(test);
                subs = subs.Where(expression2);
                Each(subs, action);
            }

        }

        /// <summary>
        /// 填充树形表格(默认为Id和ParentId)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public virtual void Each<Tree>(IQueryable<Tree> items, Action<Tree> action, bool neadTree)
            where Tree : class
        {
            BaseBll<Tree> bll = new BaseBll<Tree>();
            var param = Expression.Parameter(typeof(Tree), "Sort");
            var pi = typeof(Tree).GetProperty("Sort");
            if (pi != null)
            {
                var types = new Type[2];
                types[0] = typeof(Tree);
                types[1] = pi.PropertyType;
                Expression expr = Expression.Call(typeof(Queryable), "OrderBy", types, items.Expression, Expression.Lambda(Expression.Property(param, "Sort"), param));
                items = items.AsQueryable().Provider.CreateQuery<Tree>(expr);
            }
            foreach (var item in items)
            {
                action(item);

                var treeExpr = Expression.Parameter(typeof(Tree));

                var parentProp = typeof(Tree).GetProperty("ParentId");

                Expression<Func<Tree, bool>> test = Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(treeExpr, parentProp),
                        Expression.Convert(Expression.Constant(item.AsDy().ID), parentProp.PropertyType)
                    ),
                    treeExpr);

                var flagTrashed = this.FlagTrashed == "1";
                ConstantExpression constFlagTrashed = Expression.Constant(flagTrashed);
                ParameterExpression paramFlagTrashed = Expression.Parameter(typeof(Tree), "p");
                MemberExpression mex = LambdaExpression.PropertyOrField(paramFlagTrashed, "FlagTrashed");
                BinaryExpression filter = Expression.Equal(mex, constFlagTrashed);
                Expression<Func<Tree, bool>> expression2 = Expression.Lambda<Func<Tree, bool>>(filter, new ParameterExpression[] { paramFlagTrashed });
                var subs = bll.All.Where(test);
                subs = subs.Where(expression2);
                if (neadTree)
                    Each(subs, action);
            }

        }

        

        /// <summary>
        /// 格式化子项（放在TR标签中）
        /// </summary>
        /// <param name="item"></param>
        public virtual string FormateSubItem(object parentId)
        {
            var parstr = Convert.ToString(parentId);
            var subcss = "";
            if (!parstr.IsWhite())
            {
                subcss = "child-of-node-" + parstr;
            }
            return subcss;
        }

        /// <summary>
        /// 获取树形列表
        /// </summary>
        /// <typeparam name="Tree">类型</typeparam>
        /// <param name="rootItems">顶级项的列表</param>
        /// <param name="valueProp">Value的名称（v=>v.name）</param>
        /// <param name="trAttr">tr的属性（onclick=""）</param>
        /// <param name="idName">Id的属性名</param>
        /// <param name="parentIdName">Parent的属性名</param>
        /// <param name="tbAttr">table的属性</param>
        /// <param name="tdAttr">td的属性</param>
        /// <param name="spanAttr">span的属性</param>
        /// <returns></returns>
        public virtual string MorSunTree<Tree,V>(IQueryable<Tree> rootItems, Expression<Func<Tree,V>> valueProp,string trAttr = "",
            string idName = "Id", string parentIdName = "ParentId",
            string tbAttr = " style=\"border:0px\"",
            string tdAttr = "", string spanAttr = "class=\"TreeIcon\" style=\" font-size:12px;\"")
        where Tree : class
        {
            var treeHtml = new StringBuilder();
            var tb = string.Format("<div id=\"CategoryDiv\"><table id=\"MoreSunList\" width=\"99%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" {0}>", tbAttr);
            treeHtml.Append(tb);
            Action<Tree> treeAction = (item) =>
            {
                var id = item.GetType().GetProperty(idName).FastGetValue(item);
                var pid = item.GetType().GetProperty(parentIdName).FastGetValue(item);
                var itemvalue = true.PropOf(valueProp).FastGetValue(item);
                var tr = string.Format("<tr id=\"node-{0}\" class=\"{1}\" {2}>", id, FormateSubItem(pid), trAttr);
                var td = string.Format("<td {0}>", tdAttr);
                var span = string.Format("<span {0}>{1}", spanAttr, itemvalue);
                var end = "</span></td></tr>";
                treeHtml.Append(tr);
                treeHtml.Append(td);
                treeHtml.Append(span);
                treeHtml.Append(end);
            };
            Each<Tree>(rootItems, treeAction);
            treeHtml.Append("</table><div>");
            return treeHtml.ToString();
        }

        //public virtual string DropDownList<Drop>(string id)
        //    where Drop:class
        //{
        //    var bll = new BaseBll<Drop>();
        //    var dropStr = new StringBuilder();
        //    var select = string.Format("<select id=\"{0}\">", id);
        //    dropStr.Append(select);
        //    foreach (var item in bll.All)
        //    {
        //        var option = string.Format("<option value=\"{0}\">{1}</option>", item.AsDy().Key, item.AsDy().Value);

        //    }
        //}

        #region 属性集合
        /// <summary>
        /// 被选中的编号
        /// </summary>
        public virtual string CheckedId { get; set; }
        private string _FlagDeleted = "0";
        /// <summary>
        /// 删除标记("0"或"1")，默认为"0"
        /// </summary>
        public virtual string FlagDeleted { get { return _FlagDeleted; } set { _FlagDeleted = value; } }

        private string _FlagTrashed = "0";
        /// <summary>
        /// 回收站标记("0"或"1")，默认为"0"
        /// </summary>
        public virtual string FlagTrashed { get { return _FlagTrashed; } set { _FlagTrashed = value; } }
        public string isTree { get; set; }
        #endregion


        #region 基本信息
        /// <summary>
        /// 用户ID
        /// </summary>        
        protected static Guid UserID
        {
            get
            {
                string name = System.Web.HttpContext.Current.User.Identity.Name;
                if (string.IsNullOrEmpty(name))
                    System.Web.HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    System.Web.HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
                return new Guid(user.ProviderUserKey.ToString());
            }
        }
        /// <summary>
        /// 用户ID
        /// </summary>        
        protected static Guid? UserIdOrNull
        {
            get
            {
                string name = System.Web.HttpContext.Current.User.Identity.Name;
                if (string.IsNullOrEmpty(name))
                    return null;
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return null;
                return new Guid(user.ProviderUserKey.ToString());
            }
        }
        /// <summary>
        /// 是否登录
        /// </summary>        
        protected static bool IsLogin
        {
            get
            {
                if (System.Web.HttpContext.Current.User == null || System.Web.HttpContext.Current.User.Identity == null || String.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 当前用户基本信息
        /// </summary>        
        public static aspnet_Users CurrentAspNetUser
        {
            get
            {
                aspnet_Users user = new BaseBll<aspnet_Users>().GetModel(UserID);
                return user;
            }
        }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        protected static MembershipUser CurrentUser
        {
            get
            {
                return Membership.GetUser();
            }
        }
        #endregion

    }
}
