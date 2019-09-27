using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    /// <summary>
    /// 通用数据访问类
    /// </summary>
    class SQLHelper
    {
        private static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();
        #region 简化版
        /// <summary>
        /// 执行通用的增、删、改操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql,SqlParameter[] sqlParameters = null)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql,conn);
            conn.Open();
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 返回单一结果查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, SqlParameter[] sqlParameters = null)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }
            try
            {
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 返回查询结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sql, SqlParameter[] sqlParameters = null)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }
            try
            {
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 完整版
        /// <summary>
        /// 执行通用的增、删、改操作
        /// </summary>
        /// <param name="cmdText">sql语句存储过程名称</param>
        /// <param name="sqlParameters">参数数组</param>
        /// <param name="isProcedure">是否是存储过程</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, SqlParameter[] sqlParameters = null, bool isProcedure = false)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }
            if (isProcedure)
            {
                //表示执行的是存储过程
                cmd.CommandType = CommandType.StoredProcedure;
            }
            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 返回单一结果集
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="sqlParameters"></param>
        /// <param name="isProcedure"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, SqlParameter[] sqlParameters = null, bool isProcedure = false)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }
            if (isProcedure)
            {
                cmd.CommandType = CommandType.StoredProcedure;
            }
            try
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 返回一个只读结果集查询方法
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="sqlParameters"></param>
        /// <param name="isProcedure"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string cmdText, SqlParameter[] sqlParameters = null, bool isProcedure = false)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }
            if (isProcedure)
            {
                cmd.CommandType = CommandType.StoredProcedure;//表示执行的是存储过程
            }
            try
            {
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                throw ex;
            }
        }
        #endregion

        #region 通用方法：事务、DataSet、以及获取服务器时间
        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerTime()
        {
            DateTime serverTime = Convert.ToDateTime(ExecuteScalar("select getdate()",null));
            return serverTime;
        }

        /// <summary>
        /// 执行返回数据集的查询（针对一张数据表）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <param name="tableName"></param>
        /// <param name="isProcedure"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql, SqlParameter[] sqlParameters = null,string tableName = null, bool isProcedure = false)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }
            if (isProcedure)
            {
                //表示执行的是存储过程
                cmd.CommandType = CommandType.StoredProcedure;
            }
            //创建数据适配器
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //创建数据集
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                if (tableName != null)
                {
                    adapter.Fill(ds, tableName);
                }
                else
                {
                    adapter.Fill(ds);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 使用DataSe存储查询结果（针对多张数据表）
        /// </summary>
        /// <param name="sqlAndTableName"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(Dictionary<string,string> sqlAndTableName)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            //创建数据适配器
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //创建数据集
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                foreach (var tableName in sqlAndTableName.Keys)
                {
                    cmd.CommandText = sqlAndTableName["tableName"];
                    adapter.Fill(ds, tableName);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 通过事务提交数据：多条sql语句
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        public static bool ExecuteTransaction(List<string> sqlList)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            try
            {
                conn.Open();
                //开启事务
                cmd.Transaction = conn.BeginTransaction();
                foreach (var sql in sqlList)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();//提交事务：将数据保存到数据库，提交成功则事务会自动清空
                return true;
            }
            catch (Exception ex)
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction.Rollback();// 回滚事务，回滚成功事务也会自动清空
                }
                throw ex;
            }
            finally
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction = null;
                }
                conn.Close();
            }
        }

        /// <summary>
        /// 启用事务，提交多条带参数的sql语句，适合主从表的关系
        /// </summary>
        /// <returns></returns>
        public static bool ExecuteTransaction(string mainSql, SqlParameter[] mainParam, string detailSql,List<SqlParameter[]> detailParam)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            try
            {
                cmd.Transaction = conn.BeginTransaction();
                //执行主表操作
                cmd.Parameters.AddRange(mainParam);
                cmd.CommandText = mainSql;
                cmd.ExecuteNonQuery();
                //执行明细表操作
                cmd.CommandText = detailSql;
                foreach (var param in detailParam)
                {
                    //必须先情况以及添加的参数数组
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                }
                //提交事务
                cmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction = null;
                }
                conn.Close();
            }
        }
        #endregion
    }
}
