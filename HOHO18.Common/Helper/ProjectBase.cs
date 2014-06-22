using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;


namespace HOHO18.Common.Helper
{
    /// <summary>
    /// ProjectBase是工程实体基类.工程实体基类--该类是工程实体类的基类,在工程对象包里有两种类型的类,一种是实体类,例如分项工程类,要素价格类等等,
    /// 还有一种就是链表类,链表类的出项主要是为了实现强类型的数据集链表.而工程实体基类主要是抽象出这些类的共同点
    /// 例如对象标识,对象父标识等等信息.
    /// </summary>
    [Serializable]
    public abstract class ProjectBase
    {
        public void Calculate()
        {
            //throw new NotImplementedException();
        }

        public void ReCalculate()
        {
            //throw new NotImplementedException();
        }

        public void AllCalculate()
        {
            //throw new NotImplementedException();
        }

        //public IBase GetParentRef(string parentName)
        //{
        //    throw new NotImplementedException();
        //}

        public string GetPropertyValue(string PropertyName)
        {
            //throw new NotImplementedException();
            return "";
        }

        /// <summary>
        /// 将本工程对象序列化成XML文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Serialize(string path)
        {
            try
            {
                XmlSerializer xls = new XmlSerializer(this.GetType());
                FileStream fs = new FileStream(path, FileMode.Create);
                xls.Serialize(fs, this);
                fs.Close();
                fs = null;
                xls = null;
            }
            catch (Exception ee)
            {
                Exception e = new Exception("ERROR-010004 无法将对象写入文件");
                throw ee;
            }
        }

        public string SerializeToString()
        {
            try
            {
                XmlSerializer xls = new XmlSerializer(this.GetType());
                MemoryStream ms = new MemoryStream();
                xls.Serialize(ms, this);
                ms.Position = 0;
                byte[] b = ms.ToArray();
                ms.Close();
                xls = null;
                return Convert.ToBase64String(b);
            }
            catch
            {
                Exception e = new Exception("ERROR-010104 无法将对象写入内存");
                throw e;
            }
        }

        /// <summary>
        /// 将XML文件反序列化成工程对象
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回工程对象</returns>
        public ProjectBase Deserialize(string path)
        {
            try
            {
                XmlSerializer xls = new XmlSerializer(this.GetType());
                FileStream fs = new FileStream(path, FileMode.Open,FileAccess.Read);
                ProjectBase pb = (ProjectBase)xls.Deserialize(fs);
                fs.Close();
                fs = null;
                xls = null;
                //Project.Manage.ProjectManage.SetID(pb, null);
                //pb.AllCalculate();
                return pb;
            }
            catch
            {
                Exception e = new Exception("ERROR-010005 无法从文件中读取对象");
                throw e;
            }
        }


        /// <summary>
        /// 判断字符串是否为空(当为null和空串时都为空.如果为空返回true,否则返回false)
        /// </summary>
        /// <param name="str">需要验证的字符串</param>
        /// <returns>返回bool值</returns>
        protected bool IsBlankString(string str)
        {
            if (str != null && str != "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}



