using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

#if !NoExHelp2010
using System.Collections.Concurrent;
#endif

namespace System
{
    /// <summary>
    /// 动态类型和匿名类型
    /// </summary>
    public static class DyHelp
    {

#if !NoExHelp2010

        /// <summary>
        /// 声明为动态类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static dynamic AsDy<T>(this T t)
        {
            return t;
        }

        /// <summary>
        /// 设置属性，重p中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static T SetProp<T, P>(this T t, P p)
        {
            p.CopyProp(t);
            return t;
        }

        
        

        /// <summary>
        /// 拷贝一个对象，其属性值将从obj中拷贝，如果属性名和类型对应的话
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T CopyProp<T>(this object obj, T toObj = default(T))
        {
            if (toObj == null)
            {
                toObj = typeof(T).New<T>();
            }

            var mapPis = GetMapPis(obj.GetType(), typeof(T));

            foreach (var mapPi in mapPis)
            {
                var value = mapPi.OldPi.GetValue(obj, null);
                mapPi.NewPi.SetValue(toObj, value, null);
            }

            return toObj;
        }


        /// <summary>
        /// 获取到拷贝属性的集合
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IEnumerable<T> SelectProp<S, T>(this IEnumerable<S> src)
            where T : new()
        {
            var list = new List<T>();
            foreach (var item in src)
            {
                var value = item.CopyProp<T>();
                list.Add(value);
            }
            return list;
        }

        #region varToDy

        /// <summary>
        /// 将匿名函数转化为动态类型
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static dynamic VarToDy(this object entity)
        {
            var entityType = entity.GetType();
            var dynamicType = s_dynamicTypes.GetOrAdd(entityType, s_dynamicTypeCreator);

            var dynamicObject = Activator.CreateInstance(dynamicType);
            foreach (var entityProperty in entityType.GetProperties())
            {
                var value = entityProperty.GetValue(entity, null);
                dynamicType.GetField(entityProperty.Name).SetValue(dynamicObject, value);
            }

            return dynamicObject;
        }

        private static ConcurrentDictionary<Type, Type> s_dynamicTypes = new ConcurrentDictionary<Type, Type>();

        private static Func<Type, Type> s_dynamicTypeCreator = new Func<Type, Type>(CreateDynamicType);

        /// <summary>
        /// 创建类型
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static Type CreateDynamicType(Type entityType)
        {
            var asmName = new AssemblyName("DynamicAssembly_" + Guid.NewGuid());
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var moduleBuilder = asmBuilder.DefineDynamicModule("DynamicModule_" + Guid.NewGuid());

            var typeBuilder = moduleBuilder.DefineType(
                entityType.GetType() + "$DynamicType",
                TypeAttributes.Public);

            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            foreach (var entityProperty in entityType.GetProperties())
            {
                typeBuilder.DefineField(entityProperty.Name, entityProperty.PropertyType, FieldAttributes.Public);
            }

            return typeBuilder.CreateType();
        }

        static Dictionary<ListHashK, List<MapPi>> maps;

        /// <summary>
        /// 获取属性映射关系
        /// </summary>
        /// <param name="oldType"></param>
        /// <param name="newType"></param>
        /// <returns></returns>
        public static List<MapPi> GetMapPis(Type oldType, Type newType)
        {
            List<MapPi> result = null;
            var key = new ListHashK
            {
                Keys = new[]
                {
                    oldType,
                    newType
                }
            };

            maps = LoadHelp.Load(maps);
            //(0.0d) .ToAs("");            

            if (!maps.TryGetValue(key, out result))
            {
                //查找相同的属性
                #region findMap

                result = new List<MapPi>();
                var oldPis = oldType.GetProperties();
                var newPis = newType.GetProperties();
                for (var i = 0; i < oldPis.Length; i++)
                {
                    var oldPi = oldPis[i];
                    PropertyInfo newPi = null;
                    for (var j = 0; j < newPis.Length; j++)
                    {
                        var pi = newPis[j];
                        //名称相同 && 类型匹配
                        if (oldPi.Name.Eql(pi.Name) &&
                            (oldPi.PropertyType == pi.PropertyType ||
                            oldPi.PropertyType.IsSubclassOf(pi.PropertyType)))
                        {
                            newPi = pi;
                        }
                    }

                    if (newPi != null)
                    {
                        result.Add(new MapPi
                        {
                            OldPi = oldPi,
                            NewPi = newPi,
                        });
                    }
                }
                maps[key] = result;

                #endregion
            }
            return result;
        }

        #endregion

#endif
    }

    /// <summary>
    /// 描述一个属性映射
    /// </summary>
    public class MapPi
    {

        /// <summary>
        /// 原始属性
        /// </summary>
        public PropertyInfo OldPi { get; set; }

        /// <summary>
        /// 映射属性
        /// </summary>
        public PropertyInfo NewPi { get; set; }

        Action<object, object> _setter;

        public virtual Action<object, object> Setter
        {
            get
            {
                _setter = _setter.Load(() =>
                {

                    //(oldObj,newObj)
                    var oldObjPar = Expression.Parameter(typeof(object), "oldObj");
                    var newObjPar = Expression.Parameter(typeof(object), "newObj");

                    //(S)oldObj
                    var oldObj = Expression.Convert(oldObjPar, OldPi.DeclaringType);

                    //((S)oldObj).Prop
                    var oldObjValue = Expression.Property(oldObj, OldPi);

                    //(T)newObj
                    var newObj = Expression.Convert(newObjPar, NewPi.DeclaringType);

                    //((T)newObj).Prop=((S)oldObj).Prop;
                    var newObjSetter = Expression.Call(newObj, NewPi.GetSetMethod(), oldObjValue);



                    return null;
                });
                return _setter;
            }
            set { _setter = value; }
        }
    }
}
