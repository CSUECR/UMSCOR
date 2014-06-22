using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 数字操作
    /// </summary>
    public static class NumHelp
    {
        /// <summary>
        /// 创建数目为count的数组 foreach(var i in 50.Count())
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] Count(this int count)
        {
            var nums = new int[count];
            for (var i = 0; i < nums.Length; i++)
            {
                nums[i] = i;
            }
            return nums;
        }

        /// <summary>
        /// 获取start到end范围的整数值
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int[] Step(this int start, int end)
        {
            var nums = (end - start).Count();
            for (int i = 0, j = start; i < nums.Length; i++, j++)
            {
                nums[i] = j;
            }
            return nums;
        }

        //操作符号
        #region op

        /// <summary>
        /// 加法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static double Add(this double left, double right)
        {
            return left + right;
        }

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static double Sub(this double left, double right)
        {
            return left - right;
        }

        /// <summary>
        /// 乘法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static double Mul(this double left, double right)
        {
            return left * right;
        }



        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static double Div(this double left, double right)
        {
            return left / right;
        }

        /// <summary>
        /// 数字 x 的 y 次幂
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Pow(this double x, double y = 2)
        {
            return Math.Pow(x, y);
        }

        #endregion

        //转换
        #region cast

        /// <summary>
        /// 装换为double
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double ToNum(this int num)
        {
            return num;
        }

        /// <summary>
        /// 转换为int
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int ToInt(this double num)
        {
            return (int)num;
        }

        /// <summary>
        /// 装化为decimal
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDec(this double num)
        {
            return (decimal)num;
        }

        /// <summary>
        /// 装化为double
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double ToNum(this decimal num)
        {
            return (double)num;
        }

        /// <summary>
        /// 装化为int
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        #endregion



        //数字文本操作
        #region str


        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNum(this string str)
        {
            double num;
            var result = double.TryParse(str ?? "", out num);
            return result;
        }

        /// <summary>
        /// 判定是否是整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(this string str)
        {
            int num;
            var result = int.TryParse(str ?? "", out num);
            return result;
        }

        /// <summary>
        /// 转换为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double? ToNumN(this string str, bool err = false)
        {
            double? result = null;

            double num;
            var isNum = double.TryParse(str ?? "", out num);

            if (isNum)
            {
                result = num;
            }
            else if (err)
            {
                var ex = new InvalidCastException();
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 强制转换为整数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static int? ToIntN(this string str, bool err = false)
        {
            int? result = null;
            var num = str.ToNumN(err);
            if (num != null)
            {
                result = (int?)num.Value;
            }
            return result;
        }

        /// <summary>
        /// 强制转换为int
        /// </summary>
        /// <param name="str"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static int ToInt(this string str, bool err = false)
        {
            int num = 0;
            var isNum = int.TryParse(str, out num);
            if (!isNum && err)
            {
                throw new InvalidCastException();
            }
            return num;
        }

        /// <summary>
        /// 强制转换为数字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static double ToNum(this string str, bool err = false)
        {
            double num = 0;
            var isNum = double.TryParse(str, out num);
            if (!isNum && err)
            {
                throw new InvalidCastException();
            }
            return num;
        }

        #endregion



        /// <summary>
        /// 实现数据的四舍五入法
        /// </summary>
        /// <param name="v">要进行处理的数据</param>
        /// <param name="x">保留的小数位数</param>
        /// <returns>四舍五入后的结果</returns>
        public static double Round(double v, int x)
        {
            bool isNegative = false;
            //如果是负数
            if (v < 0)
            {
                isNegative = true;
                v = -v;
            }

            int IValue = 1;
            for (int i = 1; i <= x; i++)
            {
                IValue = IValue * 10;
            }
            double Int = Math.Round(v * IValue + 0.5, 0);
            v = Int / IValue;

            if (isNegative)
            {
                v = -v;
            }
            return v;
        }
    }
}
