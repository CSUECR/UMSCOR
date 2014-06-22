using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Reflection;
using System.Data.Common;
using FastReflectionLib;
using System.Data;
using System.Data.SqlClient;
using HOHO18.Common.Web;
using Microsoft.ApplicationBlocks.Data;
using System.Configuration;

namespace HOHO18.Common.Base
{
    public class BaseSQL<Entity> where Entity : class ,new ()
    {
        protected CacheDependency fileDependency = new CacheDependency(HttpContext.Current.Server.MapPath("~/Web.Config"));

         /// <summary>
        /// 规则数据源连接词
        /// </summary>
        protected string connectionString = string.Empty;//规则数据源

        public BaseSQL()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        }

        /// <summary>
        /// 将DataRow转换为model
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public Entity DataRowToModel(DataSet ds)
        {
            PropertyInfo[] allPis = typeof(Entity).GetProperties();
            Entity entity = new Entity();
            var columnNames = GetDataSetColumnNames(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < allPis.Length; i++)
                {
                    if (allPis[i].PropertyType.IsEnum)
                    {
                        if (columnNames.Contains(allPis[i].Name))
                        {
                            allPis[i].SetValue(entity, Enum.ToObject(allPis[i].PropertyType, ds.Tables[0].Rows[0][allPis[i].Name]), null);
                        }
                    }
                    else
                    {
                        if (columnNames.Contains(allPis[i].Name))
                        {
                            allPis[i].SetValue(entity, ds.Tables[0].Rows[0][allPis[i].Name].ToString() == "" ? null : ds.Tables[0].Rows[0][allPis[i].Name], null);
                        }

                    }

                }
            }
           

            return entity;
        }

        public string BackupDataBase(string path,string database)
        {
            string ret = string.Empty;
            string backupFileName = System.IO.Path.Combine(path, DateTime.Today.ToString("yyyyMMdd.bak"));

            string sqlstr = string.Format("BACKUP DATABASE [" + database + "] TO DISK='{0}'", backupFileName);
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlstr);
                ret = "true";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        /// <summary>
        ///  根据ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Entity GetModelByID(string id,string tableName,string key)
        {
            string sqlstr = string.Format("select * from {0} where {2}='{1}'",tableName,id,key);
            SqlConnection conn = new SqlConnection(connectionString);
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sqlstr);
            return ds.Tables[0].Rows.Count > 0 ? DataRowToModel(ds) : default(Entity);
        }

        public string GetTableColumnName(string table)
        {
            string sqlstr = string.Format("select name from syscolumns where id=object_id('{0}')",table);
            SqlConnection conn = new SqlConnection(connectionString);
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sqlstr);
            string names = string.Empty;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    names += ds.Tables[0].Rows[i][0].ToString() + ",";
                }
            }
            return names;
        }

        private string GetDataSetColumnNames(DataSet ds)
        {
            string names = string.Empty;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                {
                    names += ds.Tables[0].Columns[i].ColumnName + ",";
                }
            }
            return names;
        }
    }
}
