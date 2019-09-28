using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// 商品数据访问类
    /// </summary>
    public class ProductService
    {
        #region 根据商品Id获取商品信息
        /// <summary>
        /// 根据商品Id获取商品信息
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Product GetProductById(string productId)
        {
            string sql = "select ProductId,ProductName,UnitPrice,Discount from Products where ProductId=@ProductId";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ProductId",productId),
            };
            Product product = null;
            try
            {
                SqlDataReader reader = SQLHelper.ExecuteReader(sql, sqlParameters);
                if (reader.Read())
                {
                    product = new Product
                    {
                        ProductId = reader["ProductId"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                        Discount = Convert.ToInt32(reader["Discount"])
                    };
                }
                reader.Close();
                return product;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 启用事务提交商品对象、更新用户积分

        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="objSaleList"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool SaveSaleInfo(SalesListMain objSaleList, SMMembers member)
        {
            List<string> sqlList = new List<string>();
            //【1】组合sql语句（插入主表）
            string mainSql = "insert into SalesList(SerialNum, TotalMoney, RealReceive, ReturnMoney, SalesPersonId) values('{0}',{1},{2},{3},{4})";
            mainSql = string.Format(mainSql, objSaleList.SeriaINum, objSaleList.TotalMoney, objSaleList.RealRecieve, objSaleList.ReturnMoney, objSaleList.SalesPersonId);
            sqlList.Add(mainSql);
            //【2】组合sql语句（插入明细表以及更新库存）
            foreach (SalesListDetail detail in objSaleList.ListDetail)
            {
                //插入明细表
                string detailSql = "insert into SalesListDetail(SerialNum, ProductId, ProductName, UnitPrice, Discount, Quantity, SubTotalMoney)";
                detailSql += " values('{0}','{1}','{2}',{3},{4},{5},{6})";
                detailSql = string.Format(detailSql, detail.SeriaINum, detail.ProductId, detail.ProductName, detail.UnitPrice, detail.Discount, detail.Quantity, detail.SubTotalMoney);
                sqlList.Add(detailSql);
                //跟新库存
                string updateSql = $"update ProductInventory set TotalCount=TotalCount-{detail.Quantity} where ProductId='{detail.ProductId}'";
                sqlList.Add(updateSql);
            }
            //【3】更新客户积分
            if (member != null)
            {
                string pointSql = "update SMMembers set Points=Points+{0} where MemberId={1}";
                pointSql = string.Format(pointSql, member.Points, member.MemeberId);
                sqlList.Add(pointSql);
            }
            try
            {
                //【4】启用事务，返回结果
                return SQLHelper.ExecuteTransaction(sqlList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
