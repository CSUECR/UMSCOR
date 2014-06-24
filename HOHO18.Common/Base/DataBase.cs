using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace HOHO18.Common.Base
{
    /// <summary>
    /// 连接数据库的操作类
    /// </summary>
    public class DataBase
    {

        #region 私有变量
        private System.Data.SqlClient.SqlConnection conn = null;
        private string connStr = "";
        #endregion

        #region 构造函数
        /// <summary>
        /// 实例化数据库操作类
        /// </summary>
        public DataBase()
        {
            connStr = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
            conn = getConn();
        }
        /// <summary>
        /// 实例化数据库操作类
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public DataBase(string connectionString)
        {
            connStr = connectionString;
            conn = getConn();
        }
        #endregion

        #region 获取连接词
        /// <summary>
        /// 获取连接词
        /// </summary>
        /// <returns>连接词</returns>
        private string getConnStr()
        {
            return connStr;
        }
        #endregion

        #region 获取连接
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns>连接</returns>
        private System.Data.SqlClient.SqlConnection getConn()
        {
            try
            {
                return new System.Data.SqlClient.SqlConnection(getConnStr());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 打开连接
        /// <summary>
        /// 打开连接
        /// </summary>
        public void OpenConn()
        {
            if (ConnectionState.Open != conn.State)
            {
                conn.Open();
            }
        }
        #endregion

        #region 关闭连接
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConn()
        {
            if (ConnectionState.Open == conn.State)
            {
                try
                {
                    conn.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 制造存储过程参数
        /// <summary>
        /// 制造存储过程的参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <param name="Value">参数值</param>
        /// <returns>创建的参数</returns>
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int size, object Value)
        {
            return MakeParam(ParamName, DbType, size, ParameterDirection.Input, Value);
        }
        /// <summary>
        /// 制造存储过程的参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <returns>创建的参数</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int size)
        {
            return MakeParam(ParamName, DbType, size, ParameterDirection.Output, null);
        }
        /// <summary>
        /// 制造存储过程的参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <returns>创建的参数</returns>
        public SqlParameter MakeReturnParam(string ParamName, SqlDbType DbType, int size)
        {
            return MakeParam(ParamName, DbType, size, ParameterDirection.ReturnValue, null);
        }
        private SqlParameter MakeParam(string ParamName, SqlDbType DbType, int size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;
            try
            {
                if (size > 0)
                    param = new SqlParameter(ParamName, DbType, size);
                else
                {
                    param = new SqlParameter(ParamName, DbType);
                }
                param.Direction = Direction;
                if (!(Direction == ParameterDirection.Output && Value == null))
                    param.Value = Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return param;
        }

        #endregion

        #region 执行非查询操作
        /// <summary>
        /// 执行非查询操作
        /// </summary>
        /// <param name="cmdtxt">命令文本</param>
        /// <returns>所影响的行数</returns>
        public int ExecuteNonQuery(string cmdtxt)
        {
            SqlCommand cmd = new SqlCommand(cmdtxt);
            try
            {
                cmd.Connection = conn;
                OpenConn();
                int val = cmd.ExecuteNonQuery();
                cmd.Dispose();
                CloseConn();
                return val;
            }
            catch (Exception ex)
            {
                CloseConn();
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 执行多个命令的非查询操作
        /// </summary>
        /// <param name="cmdtxtes">命令字符串数组</param>
        /// <returns>所有命令影响行数的总和</returns>
        public int ExecuteNonQuery(CommandType[] cmdType, string[] cmdtxtes, SqlParameter[][] cmdparms)
        {
            int val = 0;
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction myTrans;
            OpenConn();
            myTrans = conn.BeginTransaction();
            try
            {
                cmd.Connection = conn;
                cmd.Transaction = myTrans;
                for (int i = 0; i < cmdparms.Length; i++)
                {
                    cmd.Parameters.Clear();
                    PrepareCommand(cmd, conn, null, (cmdType == null ? CommandType.Text : cmdType[i]), cmdtxtes[i], cmdparms[i]);
                    OpenConn();
                    val += cmd.ExecuteNonQuery();
                }
                myTrans.Commit();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                CloseConn();
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (myTrans != null)
                {
                    myTrans.Dispose();
                }
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConn();
            }
            return val;
        }
        /// <summary>
        /// 执行非查询操作,命令字符串或存储过程
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdtxt">命令文本</param>
        /// <param name="cmdparms">命令参数</param>
        /// <returns>所影响的行数字</returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdtxt, params SqlParameter[] cmdparms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdtxt, cmdparms);
            try
            {
                OpenConn();
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                cmd.Dispose();
                conn.Close();
                return val;
            }
            catch (Exception ex)
            {
                CloseConn();
                if (cmd != null)
                {
                    if (cmd.Parameters.Count > 0)
                    {
                        cmd.Parameters.Clear();
                    }
                    cmd.Dispose();
                }
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 执行带有事物的非查询操作的命令字符串或存储过程
        /// </summary>
        /// <param name="trans">事物实例</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdtxt">命令文本</param>
        /// <param name="cmdparms">命令参数</param>
        /// <returns>所影响的行数</returns>
        public int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdtxt, params SqlParameter[] cmdparms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdtxt, cmdparms);
            try
            {
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                cmd.Dispose();
                CloseConn();
                return val;
            }
            catch (Exception ex)
            {
                CloseConn();
                if (cmd != null)
                {
                    if (cmd.Parameters.Count > 0)
                    {
                        cmd.Parameters.Clear();
                    }
                    cmd.Dispose();
                }
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 执行查询Scalar
        /// <summary>
        /// 返回命令执行后首行首列的值
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdtxt">命令文本</param>
        /// <param name="cmdparms">命令参数</param>
        /// <returns>首行首列的值</returns>
        public object ExecuteScalar(CommandType cmdType, string cmdtxt, params SqlParameter[] cmdparms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdtxt, cmdparms);
            try
            {
                OpenConn();
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                cmd.Dispose();
                CloseConn();
                return val;
            }
            catch (Exception ex)
            {
                CloseConn();
                if (cmd != null)
                {
                    if (cmd.Parameters.Count > 0)
                    {
                        cmd.Parameters.Clear();
                    }
                    cmd.Dispose();
                }
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 执行sql命令文本，并返回数据读取器
        /// <summary>
        /// 执行sql命令文本，并返回数据读取器
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdtxt">命令文本</param>
        /// <param name="cmdparms">命令参数</param>
        /// <returns>sqldatareader 数据读取器</returns>
        public SqlDataReader ExecuteReader(CommandType cmdType, string cmdtxt, params SqlParameter[] cmdparms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr = null;
            PrepareCommand(cmd, conn, null, cmdType, cmdtxt, cmdparms);
            try
            {
                OpenConn();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                cmd.Dispose();
                CloseConn();
                return dr;
            }
            catch (Exception ex)
            {
                CloseConn();
                if (cmd != null)
                {
                    if (cmd.Parameters.Count > 0)
                    {
                        cmd.Parameters.Clear();
                    }
                    cmd.Dispose();
                }
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 执行查询获得DataSet
        /// <summary>
        /// 得到数据集
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdtxt">命令文本或存储过程名字</param>
        /// <param name="cmdparms">命令参数</param>
        /// <returns>操作过的数据集</returns>
        public DataSet GetDs(CommandType cmdType, string cmdtxt, params SqlParameter[] cmdparms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdtxt, cmdparms);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                OpenConn();
                da.Fill(ds);
                da.Dispose();
                cmd.Parameters.Clear();
                cmd.Dispose();
                CloseConn();
                return ds;
            }
            catch (Exception ex)
            {
                CloseConn();
                if (cmd != null)
                {
                    if (cmd.Parameters.Count > 0)
                    {
                        cmd.Parameters.Clear();
                    }
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 根据搜索条件得到数据集
        /// </summary>
        /// <param name="cmdtxt">SQL语句(未附加条件时的)</param>
        /// <param name="superPrimaryKey">SQL语句中主要表的主键名称(如关联到多表,主键请用完整标识符)</param>
        /// <param name="tableNames">搜索条件的表名(可多表)</param>
        /// <param name="primaryKey">搜索子表的主键名称,请用完整标识符(表名+字段名)</param>
        /// <param name="searchKey">搜索字符串(搜索多个字符串请用空格分隔)</param>
        /// <param name="colNameArr">搜索字段数组,请用完整标识符(表名+字段名)</param>
        /// <returns></returns>
        public DataSet GetDsBySearch(string cmdtxt, string superPrimaryKey, string tableNames, string primaryKey, string searchKey, params string[] colNameArr)
        {
            //本方法用查询子句实现select * from table where id in(select id from table where colNameArr[0] like '' or colNameArr[1] like '' ...)
            searchKey = searchKey.Trim();
            if (searchKey != "")
            {
                StringBuilder sql = new StringBuilder(cmdtxt);//最终SQL语句
                while (searchKey.IndexOf("  ") != -1)//把所有连续的空格替换成一个空格
                {
                    searchKey = searchKey.Replace("  ", " ");
                }
                searchKey = searchKey.Replace("%", "\\%");
                searchKey = searchKey.Replace("_", "\\_");
                string[] searchKeyArr = searchKey.Split(' ');
                int orderIndex = cmdtxt.ToLower().IndexOf("order");
                string orderStr = "";
                string temp = sql.ToString();
                if (orderIndex != -1)
                {
                    orderStr = temp.Substring(orderIndex, temp.Length - orderIndex);
                    sql.Remove(orderIndex, sql.Length - orderIndex);
                }
                if (cmdtxt.ToLower().IndexOf("where") == -1) sql.Append(" where 1=1 ");
                ArrayList preParameters = new ArrayList();//参数的个数是由搜索条件和搜索字符串定的,该对象临时存放参数列表
                SqlParameter[] finallyParameters;//最终存放参数列表,带入执行查询方法
                sql.Append(" and " + superPrimaryKey + " in (select " + primaryKey + " from " + tableNames + " where 1=2 ");
                string selectTemp;
                for (int i = 0; i < colNameArr.Length; i++)
                {
                    for (int j = 0; j < searchKeyArr.Length; j++)
                    {
                        selectTemp = searchKeyArr[j].Replace("%", "\\%");
                        selectTemp = selectTemp.Replace("_", "\\_");
                        sql.Append(" or " + colNameArr[i] + " like '%'+@" + colNameArr[i] + "_" + j + "+'%' escape '\\' ");
                        preParameters.Add(this.MakeInParam("@" + colNameArr[i] + "_" + j, SqlDbType.VarChar, 100, selectTemp));
                    }
                }
                sql.Append(")");
                if (orderStr != "") sql.Append(" " + orderStr);
                finallyParameters = new SqlParameter[preParameters.Count];
                preParameters.CopyTo(finallyParameters);
                try { return this.GetDs(CommandType.Text, sql.ToString(), finallyParameters); }
                catch (Exception ex) { throw new Exception(ex.Message); }
            }
            return null;
        }
        #endregion

        #region 配置Command命令
        /// <summary>
        /// 制造命令
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <param name="conn">SqlConnection</param>
        /// <param name="trans">SqlTransaction</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdtxt">命令或存储过程名字</param>
        /// <param name="cmdparms">命令或存储过程参数</param>
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdtxt, SqlParameter[] cmdparms)
        {
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = cmdtxt;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }
                cmd.CommandType = cmdType;

                if (cmdparms != null)
                {
                    foreach (SqlParameter parm in cmdparms)
                    {
                        cmd.Parameters.Add(parm);
                    }

                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }

        }
        #endregion

    }
}