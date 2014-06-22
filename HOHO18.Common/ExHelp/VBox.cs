using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace System
{
    /// <summary>
    /// 存放T类型对象的盒子，可以用于懒加载和缓存
    /// </summary>
    /// <typeparam name="V">类型</typeparam>
    [Serializable]
    public class VBox<V> : IConvertible, IComparable, ICloneable, IComparable<VBox<V>>, IComparable<V>, IEquatable<VBox<V>>, IEquatable<V>, IFormattable
    {

        //创建
        #region Create

        /// <summary>
        /// 创建
        /// </summary>
        public VBox()
        {

        }

        /// <summary>
        /// 创建值为value的OBox
        /// </summary>
        /// <param name="value"></param>
        public VBox(V value)
        {
            Value = value;
        }

        /// <summary>
        /// 创建狼加载的盒子
        /// </summary>
        /// <param name="expr"></param>
        public VBox(Func<V> expr)
        {
            Lazy = expr;

        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static implicit operator V(VBox<V> box)
        {
            return box == null ? default(V) : box.Value;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator VBox<V>(V value)
        {
            return new VBox<V>(value);
        }

        /// <summary>
        /// 从懒加载表达式创建
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static implicit operator VBox<V>(Func<V> expr)
        {
            return new VBox<V>(expr);
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(VBox<V> v1, VBox<V> v2)
        {
            return object.ReferenceEquals(v1, v2) ||
                v1 != null && v1.Equals(v2);
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(VBox<V> v1, VBox<V> v2)
        {
            return !(v1 == v2);
        }

        ///// <summary>
        ///// 真假
        ///// </summary>
        ///// <param name="v"></param>
        ///// <returns></returns>
        //public static bool operator true(VBox<V> v)
        //{
        //    return v.Equals(true);
        //}

        ///// <summary>
        ///// 真假
        ///// </summary>
        ///// <param name="v"></param>
        ///// <returns></returns>
        //public static bool operator false(VBox<V> v)
        //{
        //    return v.Equals(false);
        //}



        #endregion


        //数据
        #region Data

        /// <summary>
        /// 懒加载表达式
        /// </summary>
        public virtual Func<V> Lazy { get; set; }

        /// <summary>
        /// 判定是否已经加载
        /// </summary>
        public virtual bool Loaded { get; set; }

        /// <summary>
        /// 缓存值
        /// </summary>
        V _value;

        /// <summary>
        /// 值
        /// </summary>
        public virtual V Value
        {
            get
            {
                if (!Loaded && Lazy != null)
                {
                    //防止多线程同时加载
                    lock (this)
                    {
                        if (!Loaded && Lazy != null)
                        {
                            Value = Lazy();
                        }
                    }
                }
                return _value;
            }
            set
            {
                if (!Loaded)
                {
                    Loaded = true;
                }
                _value = value;
            }
        }

        #endregion

        //重写继承自object的方法
        #region obj

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var result = object.ReferenceEquals(this, obj);
            if (!result)
            {
                if (obj is V)
                {
                    result = Value.Equals(obj);
                }
                else if (obj is VBox<V>)
                {
                    result = Equals(obj as VBox<V>);
                }
            }
            return result;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Equals(V obj)
        {
            return Value.Equals(obj);
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(VBox<V> other)
        {
            var result = false;
            if (other != null)
            {
                result = Equals(other.Value);
            }
            return result;
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// 文本表示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        public virtual new Type GetType()
        {
            return Value.GetType();
        }

        #endregion

        //转换
        #region Convert

        /// <summary>
        /// 强制转换为type类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual object ConvertTo(Type type)
        {
            return Convert.ChangeType(Value, type);
        }

        /// <summary>
        /// 强制转换为T类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T ConvertTo<T>()
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        /// <summary>
        /// 获取类型码
        /// </summary>
        /// <returns></returns>
        public virtual TypeCode GetTypeCode()
        {
            return Value is IConvertible ?
                ((IConvertible)Value).GetTypeCode() :
                Convert.GetTypeCode(Value);
        }

        /// <summary>
        /// bool
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToBoolean(provider);
        }
        /// <summary>
        /// byte
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToByte(provider);
        }
        /// <summary>
        /// char
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToChar(provider);
        }

        /// <summary>
        /// datetime
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDateTime(provider);
        }
        /// <summary>
        /// decimal
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDecimal(provider);
        }
        /// <summary>
        /// double
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDouble(provider);
        }
        /// <summary>
        /// int16
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt16(provider);
        }
        /// <summary>
        /// int
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt32(provider);
        }
        /// <summary>
        /// long
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt64(provider);
        }
        /// <summary>
        /// sbyte
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToSByte(provider);
        }
        /// <summary>
        /// float
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToSingle(provider);
        }
        /// <summary>
        /// string
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual string ToString(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToString(provider);
        }
        /// <summary>
        /// totype
        /// </summary>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)Value).ToType(conversionType, provider);
        }
        /// <summary>
        /// ushort
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt16(provider);
        }
        /// <summary>
        /// uint
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt32(provider);
        }
        /// <summary>
        /// ulong
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt64(provider);
        }

        #endregion

        //常用接口
        #region Inerface

        /// <summary>
        /// 比较器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int CompareTo(object obj)
        {
            var Comparable = (IComparable)Value;
            return Comparable.CompareTo(obj);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var ICloneable = (ICloneable)Value;
            var newValue = ICloneable.Clone();
            return new VBox<V>((V)newValue);
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(VBox<V> other)
        {
            return CompareTo(other.Value);
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(V other)
        {
            var IComparable = (IComparable<V>)Value;
            return IComparable.CompareTo(other);
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            var IFormattable = (IFormattable)Value;
            return IFormattable.ToString(format, formatProvider);
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public virtual string ToString(string format)
        {
            var formatText = "{0:" + format + "}";
            return string.Format(formatText, Value);
        }


        #endregion

    }
    
}
