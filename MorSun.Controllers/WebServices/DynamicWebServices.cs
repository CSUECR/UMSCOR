using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.CodeDom.Compiler;
using System.Net;
using System.CodeDom;
using Microsoft.CSharp;
using System.IO;
using System.Web.Services.Description;
using System.Reflection;

namespace MorSun.Controllers.WebServices
{
    public class DynamicWebServices
    {
        static SortedList<string, Type> _typeList = new SortedList<string, Type>();

        #region InvokeWebService
        static string GetCacheKey(string url, string className)
        {
            return url.ToLower() + className;
        }
        static Type GetTypeFromCache(string url, string className)
        {
            string key = GetCacheKey(url, className);
            foreach (KeyValuePair<string, Type> pair in _typeList)
            {
                if (key == pair.Key)
                {
                    return pair.Value;
                }
            }

            return null;
        }
        static Type GetTypeFromWebService(string url, string className)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((className == null) || (className == ""))
            {
                className = GetWsClassName(url);
            }


            //获取WSDL
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(url + "?WSDL");
            ServiceDescription sd = ServiceDescription.Read(stream);
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(@namespace);

            //生成客户端代理类代码
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();
            ICodeCompiler icc = csc.CreateCompiler();

            //设定编译参数
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");

            //编译代理类
            CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
            if (true == cr.Errors.HasErrors)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }

            //生成代理实例，并调用方法
            System.Reflection.Assembly assembly = cr.CompiledAssembly;
            Type t = assembly.GetType(@namespace + "." + className, true, true);

            return t;
        }

        //动态调用web服务
        public static object InvokeWebService(string url, string methodName, object[] args)
        {
            return InvokeWebService(url, null, methodName, args);
        }

        public static object InvokeWebService(string url, string className, string methodName, object[] args)
        {
            try
            {
                Type t = GetTypeFromCache(url, className);
                if (t == null)
                {
                    t = GetTypeFromWebService(url, className);

                    //添加到缓冲中

                    string key = GetCacheKey(url, className);
                    _typeList.Add(key, t);
                }

                object obj = Activator.CreateInstance(t);
                MethodInfo mi = t.GetMethod(methodName);

                return mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        }
        #endregion

    }
}