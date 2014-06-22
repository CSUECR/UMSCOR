using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Data.Objects;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using FastReflectionLib;

namespace MorSun.Common
{
    public class GenericEFDao<Context, T>
        where Context : ObjectContext
        where T : class
    {
        Context _db;

        /// <summary>
        /// 实体上下文
        /// </summary>
        public virtual Context Db
        {
            get
            {
                _db = _db.Load(() => TheEf.Entities as Context);
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        #region CRUD


        /// <summary>
        /// 通过id查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetModel(object id)
        {

            //s => s.Id=id

            id = id.ToAsV(PK.PropertyType);

            var sExpr = Expression.Parameter(typeof(T));

            var expr = Expression.Lambda<Func<T, bool>>(
                //s.Id==id
                Expression.Equal(
                //s.Id
                    Expression.Property(sExpr, PK),
                //(PKID)id
                    Expression.Constant(id)),
                //(s)
                    sExpr);

            return All.FirstOrDefault(expr);
        }

        /// <summary>
        /// 查询与该对象相匹配的数据库中的对象
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual T GetModel(T t)
        {
            //s => s.Id=id
            var id = PK.FastGetValue(t);
            return GetModel(id);
        }

        /// <summary>
        /// 获取全部实体的集合
        /// </summary>
        public virtual ObjectSet<T> All
        {
            get
            {
                var tabName = typeof(Context).GetProperties().
                    FirstOrDefault(v => v.PropertyType == typeof(ObjectSet<T>));
                return tabName.FastGetValue(Db).AsDy();
                //return Db.CreateQuery<T>("[" + typeof(T).Name + "]");
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="submitChanges">是否提交变更</param>
        /// <returns></returns>
        public virtual T Insert(T item, bool submitChanges = true)
        {

            if (PK.FastGetValue(item) == null || (PK.FastGetValue(item) != null && PK.FastGetValue(item).ToString() == Guid.Empty.ToString()))
            {
                if (PK.PropertyType == typeof(Guid) ||
                    PK.PropertyType == typeof(Guid?))
                {
                    PK.FastSetValue(item, Guid.NewGuid());
                }
            }

            Db.AddObject(EntityName, item);
            if (submitChanges)
            {
                UpdateChanges();
            }
            return item;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="submitChanges">是否提交</param>
        public virtual void Delete(T item, bool submitChanges = true)
        {
            Db.DeleteObject(item);
            if (submitChanges)
            {
                UpdateChanges();
            }
        }

        /// <summary>
        /// 删除集合
        /// </summary>
        /// <param name="items"></param>
        /// <param name="submitChanges"></param>
        public virtual void Delete(IEnumerable<T> items, bool submitChanges = true)
        {
            items = items.ToList();
            foreach (var item in items)
            {
                Db.DeleteObject(item);
            }
            if (submitChanges)
            {
                UpdateChanges();
            }
        }


        /// <summary>
        /// 更新
        /// </summary>
        public virtual void UpdateChanges()
        {
            try
            {
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新调用用这个方法
        /// </summary>
        /// <param name="item"></param>
        public virtual void Update(T item)
        {
            UpdateChanges();
        }

        #endregion

        #region help


        PropertyInfo _PK;
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        public virtual PropertyInfo PK
        {
            get
            {
                _PK = _PK.Load(() =>
                    //
                    typeof(T).GetProperties().FirstOrDefault(pi =>
                        //获取标注 pi.attrs
                        pi.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false).
                        Cast<EdmScalarPropertyAttribute>().
                            //如果存在主键标注则说明成功
                            Any(attr => attr.EntityKeyProperty))
                );
                return _PK;
            }
            set
            {
                _PK = value;
            }
        }

        /// <summary>
        /// 获取实体名称
        /// </summary>
        public virtual string EntityName
        {
            get
            {
                return All.EntitySet.Name;
            }
        }

        #endregion


    }
}
