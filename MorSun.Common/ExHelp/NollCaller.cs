using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;



namespace System
{
    using BodyFunc = Func<Expression, Expression>;

    /// <summary>
    /// 非空判定调用器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Noller<T>
    {
        /// <summary>
        /// 编译后的函数委托
        /// </summary>
        public virtual Func<object[], NollResult<T>> Func { get; set; }

        /// <summary>
        /// 分离表达式中的参数
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public virtual object[] Pars(Expression<Func<T>> expr)
        {
            return null;
        }

        /// <summary>
        /// 缓存调用的委托
        /// </summary>
        static Dictionary<string, Noller<T>> dict =
            new Dictionary<string, Noller<T>>();

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <returns></returns>
        public virtual NollResult<T> Call(Expression<Func<T>> expr)
        {
            
            //false.Noll(()=>student.Class.Name,()=>student.Name,()=>"nimei");

            var pars = Pars(expr);

            var result = Func(pars);

            return result;
        }

        /// <summary>
        /// 获取表达式对应的调用器
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Noller<T> Get(Expression<Func<T>> expr)
        {
            var key = expr.ToString();
            var caller = dict.Load(key, () => Make(expr));
            return caller;
        }

        /// <summary>
        /// 制作表达式对应的非空调用器
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Noller<T> Make(Expression<Func<T>> expr)
        {
            var caller = new Noller<T>();

            //
            Func<string> src = () => "".Trim().ToLower();
            //
            Func<object[], NollResult<string>> rs = (pars) =>
            {
                var ret = new NollResult<string>();

                var par0 = (string)pars[0];

                if (par0 != null)
                {
                    var v0 = par0.Trim();
                    if (v0 != null)
                    {
                        var v1 = v0.ToLower();
                        ret = new NollResult<string> { NoNull = true, Value = v1 };
                    }
                }

                return ret;
            };

            return caller;
        }

#if !NoExHelp2010

        

        public static Expression NollExpr(Expression expr, BodyFunc func)
        {
            Expression result = null;
            if (expr is MemberExpression)
            {
                var memberExpr = expr as MemberExpression;
                var objExpr = memberExpr.Expression;

                var resultVar = Expression.Variable(memberExpr.Type);



                BodyFunc bodyFunc = objVar =>
                    //if(obj==null)
                    Expression.IfThen(
                        Expression.Equal(objVar, Expression.Constant(null)),
                    Expression.Block(
                        //var v=obj.Prop;
                        Expression.Assign(
                            resultVar,
                            Expression.MakeMemberAccess(objVar, memberExpr.Member)
                        ),
                        //if(v!=null){...if(v1!=null){...return new v3
                        func(resultVar)
                    ));
                result = NollExpr(objExpr, bodyFunc);
            }
            else if (expr is MethodCallExpression)
            {
                
            }
            return result;
        }
#endif

    }
   

    /// <summary>
    /// 非空判定调用结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NollResult<T>
    {
        /// <summary>
        /// 判定表达式链中是否存在null值
        /// </summary>
        public virtual bool NoNull { get; set; }

        /// <summary>
        /// 调用的结果值
        /// </summary>
        public virtual T Value { get; set; }
    }
}
