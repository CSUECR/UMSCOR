using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace HOHO18.Common.Base
{
    /// <summary>
    /// 抽象基类,实现三个基本功能
    /// </summary>
    public abstract class BaseDalMSSQL<T,TType>
        where T:class
    {
        /// <summary>
        /// 规则数据源连接词
        /// </summary>
        protected string connectionString = "";//规则数据源
        /// <summary>
        /// 项目ID(从appsettings配置中的ProjectApplication读取)
        /// </summary>
        protected Guid ProjectApplication;//项目ID
        /// <summary>
        /// 是否去掉主键信息
        /// </summary>
        public abstract bool removePK { get; }//添加Model时是否去掉主键信息，有些主键为自增长的时候，就要去掉了。
        /// <summary>
        /// 数据库中的表名
        /// </summary>
        public abstract String dbTableName { get; }
        /// <summary>
        /// 数据库表中的主键名
        /// </summary>
        public abstract String pkColumnName { get; }

        /// <summary>
        /// 构造函数,自动初始化,获取appID和连接词配置,实例化db
        /// </summary>
        public BaseDalMSSQL()
        {
            if (String.IsNullOrEmpty(WebConfigHelper.GetWebConfig("ConnectionString"))) throw new Exception("未在AppSettings中配置ConnectionString节点");
            if (String.IsNullOrEmpty(WebConfigHelper.GetWebConfig("ProjectApplication"))) throw new Exception("未在AppSettings中配置ProjectApplication节点");
            connectionString = PubConstant.ConnectionString;//System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            ProjectApplication = new Guid(WebConfigHelper.GetWebConfig("ProjectApplication"));//(System.Configuration.ConfigurationManager.AppSettings["ProjectApplication"]);
        }

        #region 需要根据不同的表实现
        /// <summary>
        /// 将DataRow转换为model
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public abstract T DataRowToModel(DataRow dr);

        /// <summary>
        /// 根据字段名获取数据库参数
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParameter GetParameterMatching(String propertyName, Object value)
        {
            return GetParameterMatching(propertyName, value, "@" + propertyName);
        }

        /// <summary>
        /// 根据字段名获取数据库参数
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="parameterName">参数名称,必须包含'@'符号</param>
        /// <returns></returns>
        public abstract SqlParameter GetParameterMatching(String propertyName, Object value, String parameterName);

        /// <summary>
        /// 根据mode获得字段名-字段值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract Dictionary<String, Object> GetPaValueByModel(T model);
        #endregion

        /// <summary>
        /// 是否存在对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public bool Exists(TType id)
        {
            DataTable dt = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select count(1) from [" + dbTableName + "] where [" + pkColumnName + "]=@" + pkColumnName, new SqlParameter("@" + pkColumnName, id)).Tables[0];
            return dt.Rows.Count > 0 ? true : false;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public T GetModel(TType id)
        {
            DataTable dt = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select * from [" + dbTableName + "] where [" + pkColumnName + "]=@" + pkColumnName, new SqlParameter("@" + pkColumnName, id)).Tables[0];
            return dt.Rows.Count > 0 ? DataRowToModel(dt.Rows[0]) : default(T);
        }

        /// <summary>
        /// DataTable转换成对象集形势
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> dataTableToList(DataTable dt)
        {
            List<T> modelList = new List<T>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int n = 0; n < rowsCount; n++)
                {
                    modelList.Add(DataRowToModel(dt.Rows[n]));
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获得数据列表 调用的时候要把where子句前后的空格去掉。
        /// </summary>
        public List<T> GetModelList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM [");            
            strSql.Append(dbTableName);
            strSql.Append("]");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            } 
            return dataTableToList(SqlHelper.ExecuteDataset(connectionString, CommandType.Text, strSql.ToString()).Tables[0]);
        }

        /// <summary>
        /// 获得全部数据列表
        /// </summary>
        public List<T> GetAllModelList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM [");
            strSql.Append(dbTableName);
            strSql.Append("]");
            return dataTableToList(SqlHelper.ExecuteDataset(connectionString, CommandType.Text, strSql.ToString()).Tables[0]);
        }

        /// <summary>
        /// 添加对象,返回是否添加成功
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Insert(T model)
        {
            //if (model.ApplicationId.Equals(Guid.Empty)) model.ApplicationId = ProjectApplication;
            //if (model.AdvisoryConfigId.Equals(Guid.Empty)) model.AdvisoryConfigId = Guid.NewGuid();

            Dictionary<String, Object> c = GetPaValueByModel(model);
            //主键为自增长ID的时候，要去掉主键信息参数
            if (removePK)
                c.Remove(pkColumnName);
            //去掉主键信息参数结束
            StringBuilder pas1 = new StringBuilder(), pas2 = new StringBuilder();
            System.Collections.ArrayList paArr = new System.Collections.ArrayList();
            foreach (String col in c.Keys)
            {
                object val = c[col];
                pas1.Append("[" + col + "],");
                pas2.Append("@" + col + ",");
                paArr.Add(GetParameterMatching(col, c[col]));
            }
            if (pas1.Length > 0)
            {
                pas1.Remove(pas1.Length - 1, 1);
                pas2.Remove(pas2.Length - 1, 1);
                SqlParameter[] pa = new SqlParameter[paArr.Count];
                paArr.CopyTo(pa);                
                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "insert into [" + dbTableName + "](" + pas1.ToString() + ") values(" + pas2.ToString() + ")", pa) > 0 ? true : false;
            }

            return false;
        }

        /// <summary>
        /// 更新对象,返回是否更新成功
        /// </summary>
        /// <param name="model">要更新的对象,是修改后的对象,主键ID不能修改!</param>
        /// <returns></returns>
        public virtual bool Update(T model)
        {
            Dictionary<String, Object> c = GetPaValueByModel(model);
            StringBuilder pas1 = new StringBuilder();
            System.Collections.ArrayList paArr = new System.Collections.ArrayList();
            foreach (String col in c.Keys)
            {
                object val = c[col];
                if (!col.Equals(pkColumnName))
                {
                    pas1.Append("[" + col + "]=@" + col + ",");
                }
                paArr.Add(GetParameterMatching(col, val));
            }
            if (pas1.Length > 0)
            {
                pas1.Remove(pas1.Length - 1, 1);
                SqlParameter[] pa = new SqlParameter[paArr.Count];
                paArr.CopyTo(pa);
                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "update [" + dbTableName + "] set " + pas1.ToString() + " where [" + pkColumnName + "]=@" + pkColumnName, pa) > 0 ? true : false;
            }

            return false;
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(TType id)
        {
            return SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "delete from [" + dbTableName + "] where [" + pkColumnName + "]=@" + pkColumnName, GetParameterMatching(pkColumnName, id)) > 0 ? true : false;
        }

        /// <summary>
        /// 得到数据总数
        /// </summary>
        /// <param name="subsql">where 子语句传进来前去掉首尾的空格，并且过滤掉敏感字符</param>
        /// <returns>数量</returns>
        public virtual int Count(string subsql)
        {           
            SqlParameter[] parameters = {
					new SqlParameter("@subsql", SqlDbType.VarChar, 4000),
                    new SqlParameter("@table", SqlDbType.VarChar, 250),
					};
            parameters[0].Value = subsql;
            parameters[1].Value = dbTableName;
            return Convert.ToInt32(SqlHelper.ExecuteScalar(connectionString, "UP_CountData", parameters));
        }

        /// <summary>
        /// 批量删除数据        
        /// </summary>
        public virtual bool BatchDeleteData(string ids)
        {  
            SqlParameter[] parameters = {
					new SqlParameter("@ids", SqlDbType.VarChar, 4000),
					new SqlParameter("@table", SqlDbType.VarChar, 250),
					new SqlParameter("@idcolom", SqlDbType.VarChar, 250)};
            parameters[0].Value = ids;
            parameters[1].Value = dbTableName;
            parameters[2].Value = pkColumnName;
            return SqlHelper.ExecuteNonQuery(connectionString, "UP_DeleteData", parameters) > 0 ? true : false;            
        }

        /// <summary>
        /// 批量设置状态
        ///<param name="ids">传进来的ID要加单引号。多个中间要加逗号</param>
        /// </summary>
        public virtual bool BatchStateData(string ids, string state, string colom)
        {   
            SqlParameter[] parameters = {
					new SqlParameter("@ids", SqlDbType.VarChar, 4000),
					new SqlParameter("@state", SqlDbType.VarChar,250),
					new SqlParameter("@colom", SqlDbType.VarChar, 250),
					new SqlParameter("@table", SqlDbType.VarChar, 250),
					new SqlParameter("@idcolom", SqlDbType.VarChar, 250)};
            parameters[0].Value = ids;
            parameters[1].Value = state;
            parameters[2].Value = colom;
            parameters[3].Value = dbTableName;
            parameters[4].Value = pkColumnName;
            //return SqlHelper.ExecuteNonQuery(connectionString, "UP_StateData", parameters) > 0 ? true : false;
            return Convert.ToInt32(SqlHelper.ExecuteScalar(connectionString, "UP_StateData", parameters)) > 0 ? true : false;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="tables">表名,多张表联结是请使用 tA a inner join tB b On a.AID = b.AID</param>
        /// <param name="pkColumn">主键，可以带表头 a.AID</param>
        /// <param name="sortColumn">排序字段</param>
        /// <param name="sortType">设置排序类型, 非 0 值则降序  1 true 0 false</param>
        /// <param name="selectFields">读取字段</param>
        /// <param name="whereFilter">Where条件</param>
        /// <param name="group">分组</param>
        /// <param name="pageNumber">开始页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="isCount">是否获得总记录数</param>
        /// <param name="outTotalRecord">输出参数，总记录数，-1 是还没有执行数量统计的值</param>
        /// <param name="outTotalPage">输出参数，总页数，-1是还没有统计的值</param>
        /// <param name="outErrorMessage">错误信息，输出参数</param>
        /// <returns>表的对象集</returns>
        public virtual List<T> Paging(string tables, string pkColumn, string sortColumn, bool sortType, string selectFields, string whereFilter, string group, int pageNumber, int pageSize, bool isCount, out int outTotalRecord, out int outTotalPage, out string outErrorMessage)
        {
            if (String.IsNullOrEmpty(tables))
                tables = dbTableName;
            if (String.IsNullOrEmpty(pkColumn))
                pkColumn = pkColumnName;
            if (String.IsNullOrEmpty(selectFields))
                selectFields = "*";
            if (String.IsNullOrEmpty(whereFilter))
                whereFilter = "";
            SqlParameter[] parameters ={
                    new SqlParameter("@Tables",SqlDbType.VarChar, 1000), 
                    new SqlParameter("@PK",SqlDbType.VarChar, 100),
                    new SqlParameter("@Sort",SqlDbType.VarChar, 200),
                    new SqlParameter("@SortType",SqlDbType.Bit),
                    new SqlParameter("@PageNumber",SqlDbType.Int),
                    new SqlParameter("@PageSize",SqlDbType.Int),
                    new SqlParameter("@Fields",SqlDbType.VarChar, 1000),
                    new SqlParameter("@Filter",SqlDbType.VarChar, 1000),
                    new SqlParameter("@Group",SqlDbType.VarChar, 1000),
                    new SqlParameter("@isCount",SqlDbType.Bit),
                    new SqlParameter("@outTotalRecord",SqlDbType.Int),
                    new SqlParameter("@outTotalPage",SqlDbType.Int),
                    new SqlParameter("@outErrorMessage",SqlDbType.VarChar, 2000)
                                       };
            parameters[0].Value = tables;
            parameters[1].Value = pkColumn;
            parameters[2].Value = sortColumn;
            parameters[3].Value = sortType;
            parameters[4].Value = pageNumber;
            parameters[5].Value = pageSize;
            parameters[6].Value = selectFields;
            parameters[7].Value = whereFilter;
            parameters[8].Value = group;
            parameters[9].Value = isCount;
            parameters[10].Direction = ParameterDirection.Output;
            parameters[11].Direction = ParameterDirection.Output; 
            parameters[12].Direction = ParameterDirection.Output;
            //需要返回记录值的要加上这句话
            int i = isCount ? 1 : 0;            
            //返回记录值结束
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "UP_Paging", parameters);
            DataTable dt = ds.Tables[i];
            //SqlHelper.ExecuteDataset无法获取输出参数值，以后搜索注意
            if (isCount)
            {
                outTotalRecord = Convert.ToInt32(ds.Tables[0].Rows[0][0]);                
            }
            else
            {
                outTotalRecord=0;
            }
            outTotalPage = Convert.ToInt32(Math.Ceiling((outTotalRecord + 0.0) / pageSize));
            //outTotalRecord = Convert.ToInt32(parameters[10].Value);
            //outTotalPage = Convert.ToInt32(parameters[11].Value);
            outErrorMessage = "";// parameters[12].Value == null ? "" : parameters[12].Value.ToString();
            return dataTableToList(dt);
        }

        ///// <summary>
        ///// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        ///// </summary>
        ///// <param name="connection">数据库连接</param>
        ///// <param name="storedProcName">存储过程名</param>
        ///// <param name="parameters">存储过程参数</param>
        ///// <returns>SqlCommand</returns>
        //private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        //{
        //    SqlCommand command = new SqlCommand(storedProcName, connection);
        //    command.CommandType = CommandType.StoredProcedure;
        //    foreach (SqlParameter parameter in parameters)
        //    {
        //        if (parameter != null)
        //        {
        //            // 检查未分配值的输出参数,将其分配以DBNull.Value.
        //            if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
        //                (parameter.Value == null))
        //            {
        //                parameter.Value = DBNull.Value;
        //            }
        //            command.Parameters.Add(parameter);
        //        }
        //    }

        //    return command;
        //}

        ///// <summary>
        ///// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        ///// </summary>
        ///// <param name="storedProcName">存储过程名</param>
        ///// <param name="parameters">存储过程参数</param>
        ///// <returns>SqlCommand 对象实例</returns>
        //private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        //{
        //    SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
        //    command.Parameters.Add(new SqlParameter("ReturnValue",
        //        SqlDbType.Int, 4, ParameterDirection.ReturnValue,
        //        false, 0, 0, string.Empty, DataRowVersion.Default, null));
        //    return command;
        //}        
    }
}
