using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    /// <summary>
    /// 时间测试组建
    /// </summary>
    public static class TimeTest
    {
        /// <summary>
        /// 测试循环count次所调用action操作所用的时间
        /// </summary>
        /// <param name="count"></param>
        /// <param name="action">要测试的调用操作，参数用于获取但千测试的次数</param>
        /// <returns></returns>
        public static TimeSpan Each(
            this int count, Action<int> action)
        {
            var t = DateTime.Now;

            for (var i = 0; i < count; i++)
            {
                action(i);
            }

            return DateTime.Now - t;
        }

        /// <summary>
        /// 测试循环调用count次调用动作
        /// </summary>
        /// <param name="count"></param>
        /// <param name="timeActions"></param>
        public static void Each(
            this int count, params TimeAction[] timeActions)
        {
            foreach (var timeAction in timeActions)
            {
                var tt = Each(count, timeAction.Action);
                timeAction.ResultAction(tt);
            }
        }
    }

    /// <summary>
    /// 测试动作
    /// </summary>
    public class TimeAction
    {

        /// <summary>
        /// 被调用的动作。调用过程中将传入当前调用的次数i，从0开始。
        /// </summary>
        public virtual Action<int> Action
        { get; set; }

        /// <summary>
        /// 完成测试时将调用该操作。
        /// 可在该操作中显示调用结果。
        /// </summary>
        public virtual Action<TimeSpan> ResultAction
        { get; set; }
    }
}