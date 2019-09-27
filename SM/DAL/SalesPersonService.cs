using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Models;

namespace DAL
{
    /// <summary>
    /// 销售员数据问类
    /// </summary>
    public class SalesPersonService
    {
        #region 日志记录
        /// <summary>
        /// 添加登录日志，返回日志编号
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public int WriteLoginlog(LoginLogs logInfo)
        {
            string sql = "insert LoginLogs(LoginId, SPName, ServerName) values (@LoginId,@SPName,@ServerName);select @@identity";
            SqlParameter[] salParameters = new SqlParameter[]
            {
                new SqlParameter("@LoginId",logInfo.LoginId),
                new SqlParameter("@SPName",logInfo.SPName),
                new SqlParameter("@ServerName",logInfo.ServerName),
            };

            try
            {
                return Convert.ToInt32(SQLHelper.ExecuteScalar(sql, salParameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 用户退出日志记录
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int WriteExitLog(int logId,DateTime dt)
        {
            string sql = "update LoginLogs set ExitTime=@ExitTime where LogId=@LogId";
            SqlParameter[] salParameters = new SqlParameter[]
            {
                new SqlParameter("@ExitTime",dt),
                new SqlParameter("@LogId",logId),
            };

            try
            {
                return Convert.ToInt32(SQLHelper.ExecuteNonQuery(sql, salParameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 销售人员登录
        /// <summary>
        /// 销售人员登录
        /// </summary>
        /// <param name="salesPerson"></param>
        /// <returns></returns>
        public SalesPerson UserLogin(SalesPerson salesPerson)
        {
            string sql ="select SalesPersonId,SPName,LoginPwd from SalesPerson where SPName=@SPName and LoginPwd=@LoginPwd";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@SPName",salesPerson.SPName),
                new SqlParameter("@LoginPwd",salesPerson.LoginPwd),
            };
            try
            {
                SqlDataReader reader = SQLHelper.ExecuteReader(sql, sqlParameters);
                if (reader.Read())
                {
                    salesPerson.SalesPersonId = Convert.ToInt32(reader["SalesPersonId"]);
                }
                else
                {
                    salesPerson = null;
                }
                reader.Close();
                return salesPerson;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取服务器时间
        public DateTime GetDBServerTime()
        {
            try
            {
                return SQLHelper.GetServerTime();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

        }
        #endregion
    }
}
